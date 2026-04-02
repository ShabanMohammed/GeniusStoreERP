using AutoMapper;
using AutoMapper.QueryableExtensions;
using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Transactions.Queries.GetProductTransactions;

public class GetProductTransactionsQueryHandler : IRequestHandler<GetProductTransactionsQuery, PagedResponse<ProductTransactionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductTransactionsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResponse<ProductTransactionDto>> Handle(GetProductTransactionsQuery request, CancellationToken cancellationToken)
    {
var query = _context.StockTransactions
    .AsNoTracking()
    .Where(x => x.ProductId == request.ProductId);

if (request.StartDate.HasValue)
{
    query = query.Where(x => x.TransactionDate >= request.StartDate.Value);
}

if (request.EndDate.HasValue)
{
    query = query.Where(x => x.TransactionDate <= request.EndDate.Value);
}

        var count = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.TransactionDate)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<ProductTransactionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResponse<ProductTransactionDto>(items, count, request.CurrentPage, request.PageSize);
    }
}