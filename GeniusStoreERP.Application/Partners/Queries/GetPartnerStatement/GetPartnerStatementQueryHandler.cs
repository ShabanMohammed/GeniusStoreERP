using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Partners.Queries.GetPartnerStatement
{
    public class GetPartnerStatementQueryHandler : IRequestHandler<GetPartnerStatementQuery, PartnerStatementDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetPartnerStatementQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PartnerStatementDto> Handle(GetPartnerStatementQuery request, CancellationToken cancellationToken)
        {
            var partner = await _context.Partners
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PartnerId, cancellationToken);

            if (partner == null)
            {
                throw new Exception("الشريك غير موجود");
            }

            var partnerDto = _mapper.Map<PartnerDto>(partner);

            // 1. Calculate Opening Balance (Transactions before FromDate)
            decimal openingBalance = 0;
            if (request.FromDate.HasValue)
            {
                openingBalance = await _context.PartnerTransactions
                    .Where(t => t.PartnerId == request.PartnerId && t.TransactionDate < request.FromDate.Value)
                    .SumAsync(t => t.Debit - t.Credit, cancellationToken);
            }

            // 2. Fetch Transactions in range
            var query = _context.PartnerTransactions
                .Include(t => t.Type)
                .Where(t => t.PartnerId == request.PartnerId)
                .AsNoTracking();

            if (request.FromDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= request.ToDate.Value);
            }

            var transactions = await query
                .OrderBy(t => t.TransactionDate)
                .ThenBy(t => t.Id)
                .ToListAsync(cancellationToken);

            // 3. Transform to DTO and Calculate Running Balance
            var items = new List<PartnerStatementItemDto>();
            decimal currentRunningBalance = openingBalance;

            foreach (var t in transactions)
            {
                currentRunningBalance += (t.Debit - t.Credit);
                items.Add(new PartnerStatementItemDto(
                    t.Id,
                    t.TransactionDate,
                    t.Type?.Name ?? "غير محدد",
                    t.ReferenceNumber,
                    t.Remarks,
                    t.Debit,
                    t.Credit,
                    currentRunningBalance
                ));
            }

            decimal closingBalance = currentRunningBalance;

            return new PartnerStatementDto(partnerDto, openingBalance, items, closingBalance);
        }
    }
}
