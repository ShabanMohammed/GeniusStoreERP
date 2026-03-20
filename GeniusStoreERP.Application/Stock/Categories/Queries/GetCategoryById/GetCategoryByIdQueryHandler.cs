using AutoMapper;
using AutoMapper.QueryableExtensions;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Stock.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly IApplicationDbContext dbContext;
    private readonly IMapper mapper;

    public GetCategoryByIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<CategoryDto> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var category = await dbContext
            .Categories.AsNoTracking()
            .Where(c => !c.IsDeleted && c.Id == request.Id)
            .ProjectTo<CategoryDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
        if (category == null)
        {
            throw new NotFoundException();
        }
        return category;
    }
}
