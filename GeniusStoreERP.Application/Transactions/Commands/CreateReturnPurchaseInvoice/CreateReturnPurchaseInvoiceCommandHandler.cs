using AutoMapper;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Domain.Entities.Finances;
using GeniusStoreERP.Domain.Entities.Stock;
using GeniusStoreERP.Domain.Entities.Transactions;
using GeniusStoreERP.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Transactions.Commands.CreateReturnPurchaseInvoice;

public class CreateReturnPurchaseInvoiceCommandHandler : IRequestHandler<CreateReturnPurchaseInvoiceCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateReturnPurchaseInvoiceCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateReturnPurchaseInvoiceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _context.BeginTransactionAsync(cancellationToken);

            if (request.InvoiceItems == null || !request.InvoiceItems.Any())
                throw new EmptyInoiceException();

            // 1. جلب المورد مرة واحدة لتجنب التكرار
            var partner = await _context.Partners
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PartnerId, cancellationToken);

            var lastNumber = await _context.Invoices
                .Where(i => i.InvoiceTypeId == (int)InvoiceTypeEnum.ReturnPurchase)
                .MaxAsync(i => (int?)i.InvoiceNumber, cancellationToken) ?? 0;

            var invoice = new Invoice
            {
                InvoiceNumber = lastNumber + 1,
                InvoiceDate = request.InvoiceDate,
                TotalItemsAmount = request.TotalItemsAmount,
                TotalItemsDiscount = request.TotalItemsDiscount,
                TotalItemsTax = request.TotalItemsTax,
                FinalAmount = request.FinalAmount,
                Notes = request.Notes,
                PartnerId = request.PartnerId,
                InvoiceStatusId = request.InvoiceStatusId,
                InvoiceTypeId = (int)InvoiceTypeEnum.ReturnPurchase,
                InvoiceItems = _mapper.Map<List<InvoiceItem>>(request.InvoiceItems)
            };

            await _context.Invoices.AddAsync(invoice, cancellationToken);

            // اضافة الحركة الى حسابات المورد
            var partnerTransaction = new PartnerTransaction
            {
                Partner = partner,
                Invoice = invoice, // سيتم ربطه تلقائياً بعد SaveChanges
                TransactionDate = invoice.InvoiceDate,
                TransactionTypeId = (int)PartnerTransactionTypeEnum.PurchaseReturn,
                ReferenceNumber = invoice.InvoiceNumber.ToString(),
                Debit = invoice.FinalAmount, // مدين بقيمة المرتجع لأنها مشتريات تنقص المديونية
                Remarks = $"مرتجع مشتريات رقم {invoice.InvoiceNumber}"
            };
            await _context.PartnerTransactions.AddAsync(partnerTransaction, cancellationToken);

            // استخراج معرفات المنتجات لجلبها دفعة واحدة (Eager Loading) لتحسين الأداء
            var productIds = invoice.InvoiceItems.Select(i => i.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            foreach (var item in invoice.InvoiceItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                {
                    // التحقق من توفر الكمية الكافية للمرتجع للمورد
                    if (product.StockQuantity < item.Quantity)
                        throw new InsufficientStockException(product.Name);

                    product.StockQuantity -= item.Quantity; // نقص المخزون لمرتجع المشتريات

                    var stockMovement = new StockTransaction
                    {
                        ProductId = item.ProductId,
                        Invoice = invoice, // سيتم ربطه تلقائياً بعد SaveChanges
                        Quantity = -item.Quantity, // سالب للمشتريات
                        TransactionDate = invoice.InvoiceDate,
                        StockTransactionTypeId = (int)StockTransactionTypeEnum.Invoice,
                        InvoiceReference = invoice.InvoiceNumber.ToString(),
                        Remarks = $"مرتجع مشتريات رقم {invoice.InvoiceNumber} - المورد: {partner?.Name ?? "مورد عام"}"
                    };
                    await _context.StockTransactions.AddAsync(stockMovement, cancellationToken);
                }
            }

            // SaveChanges واحدة كافية قبل عمل Commit لضمان وحدة العمل (Unit of Work)
            await _context.SaveChangesAsync(cancellationToken);
            await _context.CommitTransactionAsync(cancellationToken);

            return invoice.Id;
        }
        catch (BusinessException)
        {
            await _context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await _context.RollbackTransactionAsync(cancellationToken);
            throw new BusinessException("فشل في إنشاء مرتجع مشتريات", ex);
        }
    }
}
