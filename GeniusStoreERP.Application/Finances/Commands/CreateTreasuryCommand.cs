using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Domain.Entities.Finances;
using MediatR;

namespace GeniusStoreERP.Application.Finances.Commands;

public record CreateTreasuryCommand(string Name, string Code, string? Description) : IRequest<int>;

public class CreateTreasuryCommandHandler : IRequestHandler<CreateTreasuryCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTreasuryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTreasuryCommand request, CancellationToken cancellationToken)
    {
        var entity = new Treasury
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            Balance = 0
        };

        _context.Treasuries.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
