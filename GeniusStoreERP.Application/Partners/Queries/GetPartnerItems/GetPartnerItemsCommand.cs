using AutoMapper;
using AutoMapper.QueryableExtensions;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos.ListItemDto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Partners.Queries.GetPartnerItems;

public record GetPartnerItemsCommand(bool IsCustomer = false, bool IsSupplier = false, bool IsShareholder = false) : IRequest<List<PartnerListItemDto>>;

public class GetPartnerItemsCommandHandler : IRequestHandler<GetPartnerItemsCommand, List<PartnerListItemDto>>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;

    public GetPartnerItemsCommandHandler(IMapper mapper, IApplicationDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }
    public async Task<List<PartnerListItemDto>> Handle(GetPartnerItemsCommand request, CancellationToken cancellationToken)
    {
        var query = _context.Partners.AsQueryable();

        if (request.IsCustomer || request.IsSupplier || request.IsShareholder)
        {
            query = query.Where(x => 
                (request.IsCustomer && x.IsCustomer) || 
                (request.IsSupplier && x.IsSupplier) ||
                (request.IsShareholder && x.IsShareholder));
        }

        return await query
             .ProjectTo<PartnerListItemDto>(_mapper.ConfigurationProvider)
             .ToListAsync(cancellationToken);
    }
}