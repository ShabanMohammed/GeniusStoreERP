using AutoMapper;
using AutoMapper.QueryableExtensions;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos.ListItemDto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Stock.Products.Queries.GetProductItems;

public class GetProductItemsCommandHandler : IRequestHandler<GetProductItemsCommand, List<ProductListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductItemsCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ProductListItemDto>> Handle(GetProductItemsCommand request, CancellationToken cancellationToken)
    {
        return await _context.Products.AsNoTracking()
                    .Where(p => !p.IsDeleted)
                    .ProjectTo<ProductListItemDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

    }
}