using AutoMapper;
using AutoMapper.QueryableExtensions;
using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeniusStoreERP.Application.Stock.Adjustments.Queries.GetStockAdjustments;

public record GetStockAdjustmentsQuery(
    int CurrentPage = 1,
    int PageSize = 10,
    string? SearchText = null) : IRequest<PagedResponse<StockAdjustmentDto>>;

public class GetStockAdjustmentsQueryHandler : IRequestHandler<GetStockAdjustmentsQuery, PagedResponse<StockAdjustmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetStockAdjustmentsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResponse<StockAdjustmentDto>> Handle(GetStockAdjustmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StockAdjustments
            .AsNoTracking()
            .Include(x => x.Items)
            .OrderByDescending(x => x.AdjustmentDate)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchText))
        {
            query = query.Where(x => x.ReferenceNumber.Contains(request.SearchText) || 
                                     (x.Remarks != null && x.Remarks.Contains(request.SearchText)));
        }

        var count = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<StockAdjustmentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResponse<StockAdjustmentDto>(items, count, request.CurrentPage, request.PageSize);
    }
}
