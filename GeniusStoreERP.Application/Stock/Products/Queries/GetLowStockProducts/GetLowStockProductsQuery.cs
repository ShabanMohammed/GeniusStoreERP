using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Products.Queries.GetProductById;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace GeniusStoreERP.Application.Stock.Products.Queries.GetLowStockProducts;

public record GetLowStockProductsQuery : IRequest<List<ProductDto>>;

public class GetLowStockProductsQueryHandler : IRequestHandler<GetLowStockProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetLowStockProductsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Where(p => p.StockQuantity <= p.ReorderLevel)
            .OrderBy(p => p.StockQuantity)
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
