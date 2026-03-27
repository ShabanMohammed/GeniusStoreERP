using AutoMapper;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Domain.Entities.Stock;
using GeniusStoreERP.Domain.Entities.Transactions;
using GeniusStoreERP.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Transactions.Commands.CreateReturnSalesInvoice;

public class CreateReturnSalesInvoiceCommandHandler : IRequestHandler<CreateReturnSalesInvoiceCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateReturnSalesInvoiceCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateReturnSalesInvoiceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _context.BeginTransactionAsync(cancellationToken);
            if (request.InvoiceItems == null || !request.InvoiceItems.Any())
                throw new EmptyInoiceException();

            var lastNumber = await _context.Invoices
                .Where(i => i.InvoiceTypeId == (int)InvoiceTypeEnum.ReturnSales)
                .Select(i => (int?)i.InvoiceNumber)
                .MaxAsync<int?>(cancellationToken) ?? 0;

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
                InvoiceTypeId = (int)InvoiceTypeEnum.ReturnSales,
                InvoiceItems = _mapper.Map<List<InvoiceItem>>(request.InvoiceItems)
            };

            await _context.Invoices.AddAsync(invoice, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            foreach (var item in invoice.InvoiceItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity; // زيادة المخزون لمرتجع المبيعات
                    _context.Products.Update(product);

                    var stockMovement = new StockTransaction
                    {

                        ProductId = item.ProductId,
                        InvoiceId = invoice.Id,
                        Quantity = item.Quantity, //مرتجع المبيعات موجب لزيادة المخزون
                        TransactionDate = invoice.InvoiceDate,
                        StockTransactionTypeId = (int)StockTransactionTypeEnum.Invoice,
                        InvoiceReference = invoice.InvoiceNumber.ToString(),
                        Remarks = "مرتجع المبيعات",
                    };
                    await _context.StockTransactions.AddAsync(stockMovement, cancellationToken);
                }
            }

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
            throw new BusinessException("فشل في إنشاء مرتجع مبيعات", ex);
        }
    }
}
