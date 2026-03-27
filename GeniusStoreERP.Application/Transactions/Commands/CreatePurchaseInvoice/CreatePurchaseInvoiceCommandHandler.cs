using AutoMapper;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Domain.Entities.Stock;
using GeniusStoreERP.Domain.Entities.Transactions;
using GeniusStoreERP.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Transactions.Commands.CreatePurchaseInvoice;

public class CreatePurchaseInvoiceCommandHandler : IRequestHandler<CreatePurchaseInvoiceCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreatePurchaseInvoiceCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreatePurchaseInvoiceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _context.BeginTransactionAsync(cancellationToken);

            if (request.InvoiceItems == null || !request.InvoiceItems.Any())
                throw new EmptyInoiceException();

            // 1. جلب المورد مرة واحدة لتجنب التكرار داخل الحلقة
            var partner = await _context.Partners
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PartnerId, cancellationToken);

            // 2. حساب رقم الفاتورة التالي
            var lastNumber = await _context.Invoices
                .Where(i => i.InvoiceTypeId == (int)InvoiceTypeEnum.Purchase)
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
                InvoiceTypeId = (int)InvoiceTypeEnum.Purchase,
                InvoiceItems = _mapper.Map<List<InvoiceItem>>(request.InvoiceItems)
            };

            await _context.Invoices.AddAsync(invoice, cancellationToken);

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
                    // تحديث المخزون
                    product.StockQuantity += item.Quantity;

                    // إضافة حركة المخزن
                    var stockMovement = new StockTransaction
                    {
                        ProductId = item.ProductId,
                        Invoice = invoice, // سيتم ربطه تلقائياً بعد SaveChanges
                        Quantity = item.Quantity,
                        TransactionDate = invoice.InvoiceDate,
                        StockTransactionTypeId = (int)StockTransactionTypeEnum.Invoice,
                        InvoiceReference = invoice.InvoiceNumber.ToString(),
                        Remarks = $"وارد فاتورة مشتريات رقم {invoice.InvoiceNumber} - المورد: {partner?.Name ?? "مورد عام"}"
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
            throw new BusinessException("حدث خطأ تقني أثناء إنشاء فاتورة المشتريات", ex);
        }
    }
}