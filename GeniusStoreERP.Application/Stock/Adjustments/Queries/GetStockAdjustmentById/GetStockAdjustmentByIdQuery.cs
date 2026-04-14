using AutoMapper;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GeniusStoreERP.Application.Stock.Adjustments.Queries.GetStockAdjustmentById;

public record GetStockAdjustmentByIdQuery(int Id) : IRequest<StockAdjustmentDto?>;

public class GetStockAdjustmentByIdQueryHandler : IRequestHandler<GetStockAdjustmentByIdQuery, StockAdjustmentDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetStockAdjustmentByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<StockAdjustmentDto?> Handle(GetStockAdjustmentByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.StockAdjustments
            .AsNoTracking()
            .Include(x => x.Items)
                .ThenInclude(i => i.Product)
            .Include(x => x.Items)
                .ThenInclude(i => i.StockTransactionType)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return entity != null ? _mapper.Map<StockAdjustmentDto>(entity) : null;
    }
}
