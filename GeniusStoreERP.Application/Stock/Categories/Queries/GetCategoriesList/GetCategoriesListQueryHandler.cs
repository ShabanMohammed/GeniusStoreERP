using AutoMapper;
using AutoMapper.QueryableExtensions;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos.ListItemDto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Stock.Categories.Queries.GetCategoriesList;

public class GetCategoriesListQueryHandler
    : IRequestHandler<GetCategoriesListQuery, List<CategoryListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCategoriesListQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        this._context = context;
        this._mapper = mapper;
    }

    public async Task<List<CategoryListItemDto>> Handle(
        GetCategoriesListQuery request,
        CancellationToken cancellationToken
    )
    {
        var categories = await _context
            .Categories.AsNoTracking()
            .Where(c => c.IsDeleted == false)
            .ProjectTo<CategoryListItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return categories;
    }
}
