using MediatR;
using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using GeniusStoreERP.Application.Exceptions;
using AutoMapper.QueryableExtensions;

namespace GeniusStoreERP.Application.Transactions.Queries.GetInvoiceByIdQuery;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetInvoiceByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<InvoiceDto> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .AsNoTracking().ProjectTo<InvoiceDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (invoice == null)
        {
            throw new NotFoundException();
        }

        return invoice;
    }
}