using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Partners.Queries.GetPartnerAccounts
{
    public class GetPartnerAccountsQueryHandler : IRequestHandler<GetPartnerAccountsQuery, PagedResponse<PartnerAccountDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPartnerAccountsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<PartnerAccountDto>> Handle(GetPartnerAccountsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Partners
                .Where(p => !p.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                query = query.Where(p => p.Name.Contains(request.SearchText) || p.PhoneNumber.Contains(request.SearchText));
            }

            if (request.IsSupplier.HasValue && request.IsSupplier.Value)
            {
                query = query.Where(p => p.IsSupplier);
            }

            if (request.IsCustomer.HasValue && request.IsCustomer.Value)
            {
                query = query.Where(p => p.IsCustomer);
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var partners = await query
                .OrderBy(p => p.Name)
                .Skip((request.CurrentPage - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new PartnerAccountDto(
                    p.Id,
                    p.Name,
                    p.PhoneNumber,
                    p.IsSupplier,
                    p.IsCustomer,
                    _context.PartnerTransactions.Where(t => t.PartnerId == p.Id).Sum(t => t.Debit),
                    _context.PartnerTransactions.Where(t => t.PartnerId == p.Id).Sum(t => t.Credit),
                    _context.PartnerTransactions.Where(t => t.PartnerId == p.Id).Sum(t => t.Debit - t.Credit)
                ))
                .ToListAsync(cancellationToken);

            return new PagedResponse<PartnerAccountDto>(partners, totalItems, request.CurrentPage, request.PageSize);
        }
    }
}
