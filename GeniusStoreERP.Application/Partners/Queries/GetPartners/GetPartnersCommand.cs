using AutoMapper;
using AutoMapper.QueryableExtensions;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Partners.Queries.GetPartners;

public record GetPartnersCommand(
    string searchText,
    bool IsSupplier,
    bool IsCustomer,
    int pageSize = 10,
    int currentPage = 1
    ) : IRequest<List<PartnerDto>>;

public class GetPartnersCommandHandler : IRequestHandler<GetPartnersCommand, List<PartnerDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPartnersCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<List<PartnerDto>> Handle(GetPartnersCommand request, CancellationToken cancellationToken)
    {
        var query = _context.Partners.AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.searchText))
        {

            query = query.Where(x =>
                x.Name.Contains(request.searchText)
                || x.Email.Contains(request.searchText)
                || x.PhoneNumber.Contains(request.searchText)
                || x.Address.Contains(request.searchText)
            );
        }
        if (request.IsSupplier)
        {
            query = query.Where(x => x.IsSupplier);
        }
        if (request.IsCustomer)
        {
            query = query.Where(x => !x.IsCustomer);
        }

        return await query
            .OrderBy(x => x.Name)
            .Skip((request.currentPage - 1) * request.pageSize)
            .Take(request.pageSize)
            .ProjectTo<PartnerDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);


    }
}

