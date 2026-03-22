using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.Transactions.Queries.GetInvoicesByType;

public record GetInvoicesByTypeQuery(int InvoiceTypeId, int currentPage = 1, int pageSize = 10) : IRequest<PagedResponse<InvoiceDto>>;
