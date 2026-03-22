using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.Partners.Queries.GetPartners;

public record GetPartnersCommand(
    string? searchText = null,
    bool IsSupplier = false,
    bool IsCustomer = false,
    int pageSize = 10,
    int currentPage = 1
    ) : IRequest<PagedResponse<PartnerDto>>;


