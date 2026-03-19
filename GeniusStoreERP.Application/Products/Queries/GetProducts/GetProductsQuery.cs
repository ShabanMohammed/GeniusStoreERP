using AutoMapper;
using AutoMapper.QueryableExtensions;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Products.Queries.GetProductById;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Products.Queries.GetProducts;

public record GetProductsQuery(string? search, int? CategoryId) : IRequest<List<ProductDto>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        var query = _context.Products.AsNoTracking().Where(p => !p.IsDeleted).AsQueryable();

        if (!string.IsNullOrEmpty(request.search))
        {
            query = query.Where(p =>
                p.Name.Contains(request.search)
                || (p.Description != null && p.Description.Contains(request.search))
                || (p.SKU != null && p.SKU.Contains(request.search))
                || (p.Barcode != null && p.Barcode.Contains(request.search))
            );
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        return await query.ProjectTo<ProductDto>(_mapper.ConfigurationProvider).ToListAsync();
    }
}
