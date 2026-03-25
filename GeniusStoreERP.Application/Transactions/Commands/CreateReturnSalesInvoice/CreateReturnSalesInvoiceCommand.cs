using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.Transactions.Commands.CreateReturnSalesInvoice;

public record CreateReturnSalesInvoiceCommand(
    DateTime InvoiceDate,
    decimal TotalItemsAmount,
    decimal TotalItemsDiscount,
    decimal TotalItemsTax,
    decimal FinalAmount,
    string? Notes,
    int PartnerId,
    int InvoiceStatusId,
    ICollection<InvoiceItemDto> InvoiceItems
) : IRequest<int>;
