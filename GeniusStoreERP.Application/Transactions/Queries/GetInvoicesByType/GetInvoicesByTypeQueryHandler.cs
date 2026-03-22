using AutoMapper;
using AutoMapper.QueryableExtensions;
using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Transactions.Queries.GetInvoicesByType;

public class GetInvoicesByTypeQueryHandler : IRequestHandler<GetInvoicesByTypeQuery, PagedResponse<InvoiceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetInvoicesByTypeQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }



    public async Task<PagedResponse<InvoiceDto>> Handle(GetInvoicesByTypeQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Invoices.AsNoTracking().Where(x => x.InvoiceTypeId == request.InvoiceTypeId);

        var count = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.InvoiceDate)
            .Skip((request.currentPage - 1) * request.pageSize)
            .Take(request.pageSize)
            .ProjectTo<InvoiceDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResponse<InvoiceDto>(items, count, request.currentPage, request.pageSize);
    }

}
