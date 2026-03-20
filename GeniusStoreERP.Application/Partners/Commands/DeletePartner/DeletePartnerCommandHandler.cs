using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Exceptions;
using MediatR;

namespace GeniusStoreERP.Application.Partners.Commands.DeletePartner;

public class DeletePartnerCommandHandler : IRequestHandler<DeletePartnerCommand>
{
    IApplicationDbContext _context;

    public DeletePartnerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeletePartnerCommand request, CancellationToken cancellationToken)
    {
        var partner = await _context.Partners.FindAsync(request.Id);
        if (partner == null)
        {
            throw new NotFoundException();
        }
        if (partner.IsDeleted)
        {
            throw new EntityDeletedException();
        }

        _context.Partners.Remove(partner);
        await _context.SaveChangesAsync();

    }
}