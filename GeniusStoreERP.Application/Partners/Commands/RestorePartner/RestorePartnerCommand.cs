using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Partners.Commands.RestorePartner;

public record RestorePartnerCommand(int PartnerId) : IRequest;

public class RestorePartnerCommandHandler : IRequestHandler<RestorePartnerCommand>
{
    private readonly IApplicationDbContext _context;

    public RestorePartnerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RestorePartnerCommand request, CancellationToken cancellationToken)
    {
        var partner = await _context.Partners
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == request.PartnerId, cancellationToken);

        if (partner == null)
        {
            throw new NotFoundException();
        }

        partner.IsDeleted = false;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
