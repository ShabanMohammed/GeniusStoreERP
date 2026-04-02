using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Dtos;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GeniusStoreERP.Application.Transactions.Queries.GetProductTransactions;

public record GetProductTransactionsQuery(
    int ProductId,
    int CurrentPage = 1,
    int PageSize = 10,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IRequest<PagedResponse<ProductTransactionDto>>;

