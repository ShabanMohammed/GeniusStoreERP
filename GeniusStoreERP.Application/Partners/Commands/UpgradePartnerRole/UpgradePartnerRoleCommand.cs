namespace GeniusStoreERP.Application.Partners.Commands.UpgradePartnerRole;

using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Domain.Entities.Partners;
using MediatR;

public record UpgradePartnerRoleCommand(int PartnerId) : IRequest;

public class UpgradePartnerRoleCommandHandler : IRequestHandler<UpgradePartnerRoleCommand>
{
    private readonly IApplicationDbContext _context;

    public UpgradePartnerRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    async Task IRequestHandler<UpgradePartnerRoleCommand>.Handle(UpgradePartnerRoleCommand request, CancellationToken cancellationToken)
    {
        var partner = await _context.Partners.FindAsync(request.PartnerId, cancellationToken);
        if (partner == null)
        {
            throw new NotFoundException();
        }
        if (partner.IsCustomer && partner.IsSupplier)
        {
            throw new BusinessException();
        }
        partner.IsCustomer = true;
        partner.IsSupplier = true;
        await _context.SaveChangesAsync(cancellationToken);
    }
}