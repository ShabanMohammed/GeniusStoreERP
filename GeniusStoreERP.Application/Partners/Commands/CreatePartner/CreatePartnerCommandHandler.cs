using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Domain.Entities.Partners;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Partners.Commands.CreatePartner;

public class CreatePartnerCommandHandler : IRequestHandler<CreatePartnerCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreatePartnerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<int> Handle(CreatePartnerCommand request, CancellationToken cancellationToken)
    {
        var cleanName = request.Name.Sanitize() ?? string.Empty;
        var existingPartner = await _context.Partners.FirstOrDefaultAsync(p => p.Name == cleanName, cancellationToken);
        if (existingPartner != null)
        {
            if (existingPartner.IsCustomer && request.IsCustomer)
            {
                throw new BusinessException();
            }
            if (existingPartner.IsSupplier && request.IsSupplier)
            {
                throw new BusinessException();
            }
            if (!existingPartner.IsCustomer && request.IsCustomer)
            {
                throw new EntityConflictException();
            }
            if (!existingPartner.IsSupplier && request.IsSupplier)
            {
                throw new EntityConflictException();
            }

            throw new BusinessException();
        }


        var partner = new Partner
        {
            Name = cleanName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            IsSupplier = request.IsSupplier,
            IsCustomer = request.IsCustomer
        };

        _context.Partners.Add(partner);
        await _context.SaveChangesAsync();

        return partner.Id;
    }
}