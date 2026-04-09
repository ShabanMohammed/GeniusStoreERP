using System;
using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.Partners.Queries.GetPartnerStatement
{
    public record GetPartnerStatementQuery(
        int PartnerId,
        DateTime? FromDate = null,
        DateTime? ToDate = null,
        int CurrentPage = 1,
        int PageSize = 50
    ) : IRequest<PartnerStatementDto>;
}
