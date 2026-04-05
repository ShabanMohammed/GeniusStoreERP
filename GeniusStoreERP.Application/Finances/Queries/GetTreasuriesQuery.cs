using AutoMapper;
using AutoMapper.QueryableExtensions;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Finances.Queries;

public record GetTreasuriesQuery : IRequest<List<TreasuryDto>>;

public class GetTreasuriesQueryHandler : IRequestHandler<GetTreasuriesQuery, List<TreasuryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTreasuriesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TreasuryDto>> Handle(GetTreasuriesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Treasuries
            .AsNoTracking()
            .ProjectTo<TreasuryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
