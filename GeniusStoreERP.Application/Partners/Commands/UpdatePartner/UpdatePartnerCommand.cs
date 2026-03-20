using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Partners.Commands.UpgradePartnerRole;

public record UpdatePartnerCommand
(
     int Id,
     string Name,
     string Email,
     string PhoneNumber,
     string Address,
     bool IsCustomer,
     bool IsSupplier
)
 : IRequest;

public class UpdatePartnerCommandHandler : IRequestHandler<UpdatePartnerCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdatePartnerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdatePartnerCommand request, CancellationToken cancellationToken)
    {
        var exitePartner = await _context.Partners.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);


        if (exitePartner == null)
        {
            throw new NotFoundException();
        }

        var ExiteNamePartner = await _context.Partners.FirstOrDefaultAsync(p => p.Name == request.Name.Sanitize() && p.Id != request.Id, cancellationToken);
        if (ExiteNamePartner != null)
        {
            if (ExiteNamePartner.IsDeleted)
                throw new EntityDeletedException();
            throw new EntityConflictException();

        }


        exitePartner.Name = request.Name.Sanitize() ?? string.Empty;
        exitePartner.Email = request.Email;
        exitePartner.PhoneNumber = request.PhoneNumber;
        exitePartner.Address = request.Address;
        exitePartner.IsCustomer = request.IsCustomer;
        exitePartner.IsSupplier = request.IsSupplier;
        await _context.SaveChangesAsync(cancellationToken);

    }
}