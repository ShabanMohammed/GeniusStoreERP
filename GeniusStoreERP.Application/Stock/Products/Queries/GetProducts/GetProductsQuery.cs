using AutoMapper;
using AutoMapper.QueryableExtensions;
using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Products.Queries.GetProductById;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Stock.Products.Queries.GetProducts;

public record GetProductsQuery(
    string? search,
    int? CategoryId,
    int pageNumber = 1,
    int pageSize = 10
) : IRequest<PagedResponse<ProductDto>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResponse<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResponse<ProductDto>> Handle(
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

        // Get total count before pagination
        int totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((request.pageNumber - 1) * request.pageSize)
            .Take(request.pageSize)
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResponse<ProductDto>(
            items,
            totalCount,
            request.pageNumber,
            request.pageSize
        );
    }
}
