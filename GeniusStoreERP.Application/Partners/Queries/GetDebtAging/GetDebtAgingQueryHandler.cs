using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Partners.Queries.GetDebtAging
{
    public class GetDebtAgingQueryHandler : IRequestHandler<GetDebtAgingQuery, List<DebtAgingDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetDebtAgingQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DebtAgingDto>> Handle(GetDebtAgingQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var thirtyDaysAgo = now.AddDays(-30);
            var sixtyDaysAgo = now.AddDays(-60);
            var ninetyDaysAgo = now.AddDays(-90);

            var partners = await _context.Partners
                .Where(p => !p.IsDeleted)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var result = new List<DebtAgingDto>();

            foreach (var partner in partners)
            {
                var transactions = await _context.PartnerTransactions
                    .Where(t => t.PartnerId == partner.Id)
                    .ToListAsync(cancellationToken);

                var totalBalance = transactions.Sum(t => t.Debit - t.Credit);

                // We only show partners with a debit balance (they owe money)
                if (totalBalance <= 0) continue;

                var current = transactions
                    .Where(t => t.TransactionDate >= thirtyDaysAgo)
                    .Sum(t => t.Debit - t.Credit);

                var thirtyToSixty = transactions
                    .Where(t => t.TransactionDate < thirtyDaysAgo && t.TransactionDate >= sixtyDaysAgo)
                    .Sum(t => t.Debit - t.Credit);

                var sixtyToNinety = transactions
                    .Where(t => t.TransactionDate < sixtyDaysAgo && t.TransactionDate >= ninetyDaysAgo)
                    .Sum(t => t.Debit - t.Credit);

                var overNinety = transactions
                    .Where(t => t.TransactionDate < ninetyDaysAgo)
                    .Sum(t => t.Debit - t.Credit);

                result.Add(new DebtAgingDto(
                    partner.Id,
                    partner.Name,
                    partner.PhoneNumber,
                    totalBalance,
                    Math.Max(0, current),
                    Math.Max(0, thirtyToSixty),
                    Math.Max(0, sixtyToNinety),
                    Math.Max(0, overNinety)
                ));
            }

            return result.OrderByDescending(r => r.TotalBalance).ToList();
        }
    }
}
