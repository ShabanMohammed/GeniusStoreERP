using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.Transactions.Queries.GetInvoiceByIdQuery;

public record GetInvoiceByIdQuery(int Id) : IRequest<InvoiceDto>;