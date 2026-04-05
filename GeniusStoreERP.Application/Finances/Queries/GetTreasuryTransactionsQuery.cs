using AutoMapper;
using AutoMapper.QueryableExtensions;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Finances.Queries;

public record GetTreasuryTransactionsQuery(int TreasuryId) : IRequest<List<TreasuryTransactionDto>>;

public class GetTreasuryTransactionsQueryHandler : IRequestHandler<GetTreasuryTransactionsQuery, List<TreasuryTransactionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTreasuryTransactionsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TreasuryTransactionDto>> Handle(GetTreasuryTransactionsQuery request, CancellationToken cancellationToken)
    {
        return await _context.TreasuryTransactions
            .AsNoTracking()
            .Where(t => t.TreasuryId == request.TreasuryId)
            .Include(t => t.Treasury)
            .Include(t => t.Partner)
            .ProjectTo<TreasuryTransactionDto>(_mapper.ConfigurationProvider)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }
}
