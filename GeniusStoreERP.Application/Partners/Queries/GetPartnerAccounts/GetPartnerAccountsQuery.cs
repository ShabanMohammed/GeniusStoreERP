using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.Partners.Queries.GetPartnerAccounts
{
    public record GetPartnerAccountsQuery(
        string? SearchText = null,
        bool? IsSupplier = null,
        bool? IsCustomer = null,
        int CurrentPage = 1,
        int PageSize = 10
    ) : IRequest<PagedResponse<PartnerAccountDto>>;
}
