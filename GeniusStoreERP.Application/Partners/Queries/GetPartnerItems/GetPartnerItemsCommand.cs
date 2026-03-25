using AutoMapper;
using AutoMapper.QueryableExtensions;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos.ListItemDto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Partners.Queries.GetPartnerItems;

public record GetPartnerItemsCommand(bool IsCustomer = false, bool IsSupplier = false) : IRequest<List<PartnerListItemDto>>;

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
        var partnerListItemDto = await _context.Partners.Where(x => x.IsCustomer == request.IsCustomer && x.IsSupplier == request.IsSupplier)
             .ProjectTo<PartnerListItemDto>(_mapper.ConfigurationProvider).ToListAsync();
        return partnerListItemDto;
    }
}