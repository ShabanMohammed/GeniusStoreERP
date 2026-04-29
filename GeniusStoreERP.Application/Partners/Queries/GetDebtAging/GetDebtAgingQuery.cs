using System.Collections.Generic;
using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.Partners.Queries.GetDebtAging
{
    public record GetDebtAgingQuery : IRequest<List<DebtAgingDto>>;
}
