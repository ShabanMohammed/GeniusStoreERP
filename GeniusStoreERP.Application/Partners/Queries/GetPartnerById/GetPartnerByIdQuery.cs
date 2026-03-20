using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.Partners.Queries.GetPartnerById;

public record GetPartnerByIdQuery(int id) : IRequest<PartnerDto>;

