using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Domain.Entities.Finances;
using GeniusStoreERP.Domain.Entities.Stock;
using GeniusStoreERP.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Transactions.Commands.VoidInvoiceByReverse;

public record VoidInvoiceByReverseCommand(int Id) : IRequest;

public class VoidInvoiceByReverseCommandHendler : IRequestHandler<VoidInvoiceByReverseCommand>
{
    private readonly IApplicationDbContext _context;
    public VoidInvoiceByReverseCommandHendler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task Handle(VoidInvoiceByReverseCommand request, CancellationToken cancellationToken)
    {
        try
        {


            await _context.BeginTransactionAsync();
            var invoice = await _context.Invoices
                                    .Include(i => i.InvoiceItems)
                                    .Where(i => i.Id == request.Id)
                                    .FirstOrDefaultAsync();
            if (invoice == null)
                throw new NotFoundException();
            if (invoice.IsDeleted)
                throw new BusinessException("الفاتورة ملغاة");

            invoice.InvoiceStatusId = (int)InvioceStatusEnum.Cancelled;
            invoice.Notes += $" - تم إلغاء الفاتورة بتاريخ {DateTime.Now:yyyy-MM-dd}";
            invoice.IsDeleted = true;

            // 2. Determine Reverse Amount (Debit vs Credit)
            decimal debit = 0;
            decimal credit = 0;
            int stockReverse = 0;



            var invoiceType = await _context.InvoiceTypes.FindAsync(invoice.InvoiceTypeId);
            string typeName = "";
            if (invoiceType != null)
                typeName = invoiceType.Name;
            else
                typeName = "فاتورة ملغاة";


            switch (invoice.InvoiceTypeId)
            {
                case 1: // مبيعات (Sales: Originally Debits the Customer)
                case 4: // مرتجع مشتريات (Purchase Return: Originally Debits the Vendor)
                    credit = invoice.FinalAmount; // Reverse with Credit
                    stockReverse = 1;
                    break;

                case 2: // مشتريات (Purchases: Originally Credits the Vendor)
                case 3: // مرتجع مبيعات (Sales Return: Originally Credits the Customer)
                    debit = invoice.FinalAmount; // Reverse with Debit
                    stockReverse = -1;
                    break;
            }

            // 3. Create the Reversing Transaction
            var reverseTransaction = new PartnerTransaction
            {
                PartnerId = invoice.PartnerId,
                InvoiceId = invoice.Id,
                TransactionDate = DateTime.UtcNow,
                TransactionTypeId = (int)PartnerTransactionTypeEnum.CancelledInvoice,
                ReferenceNumber = invoice.InvoiceNumber.ToString(),
                Debit = debit,
                Credit = credit,
                Remarks = $"إلغاء تلقائي لـ {typeName} رقم {invoice.InvoiceNumber}"
            };
            await _context.PartnerTransactions.AddAsync(reverseTransaction);
            if (invoice.InvoiceItems != null && invoice.InvoiceItems.Any())
            {
                var productIds = invoice.InvoiceItems.Select(i => i.ProductId).ToList();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync(cancellationToken);

                foreach (var item in invoice.InvoiceItems)
                {
                    item.IsDeleted = true;
                    var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        // تحديث المخزون
                        product.StockQuantity += item.Quantity * stockReverse;

                        // إضافة حركة المخزن
                        var stockMovement = new StockTransaction
                        {
                            ProductId = item.ProductId,
                            InvoiceId = invoice.Id,
                            Quantity = item.Quantity * stockReverse,
                            TransactionDate = invoice.InvoiceDate,
                            StockTransactionTypeId = (int)StockTransactionTypeEnum.Invoice,
                            InvoiceReference = invoice.InvoiceNumber.ToString(),
                            Remarks = $"إلغاء تلقائي لـ {typeName} رقم {invoice.InvoiceNumber}"
                        };

                        await _context.StockTransactions.AddAsync(stockMovement, cancellationToken);
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
            await _context.CommitTransactionAsync();
        }
        catch (Exception ex)
        {

            await _context.RollbackTransactionAsync();
            throw new BusinessException("خطاء", ex);
        }

    }
}