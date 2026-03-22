using AutoMapper;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Exceptions;
using MediatR;

namespace GeniusStoreERP.Application.Partners.Queries.GetPartnerById;

public class GetPartnerByIdQueryHandler : IRequestHandler<GetPartnerByIdQuery, PartnerDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetPartnerByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PartnerDto> Handle(GetPartnerByIdQuery request, CancellationToken cancellationToken)
    {
        var partner = await _context.Partners.FindAsync(request.id, cancellationToken);
        if (partner == null)
        {
            throw new NotFoundException();
        }
        if (partner.IsDeleted)
        {
            throw new EntityDeletedException(partner);
        }
        return _mapper.Map<PartnerDto>(partner);

    }

}
