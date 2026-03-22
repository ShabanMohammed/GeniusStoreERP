using AutoMapper;
using GeniusStoreERP.Domain.Entities.Partners;

namespace GeniusStoreERP.Application.Dtos.ListItemDto;

public record PartnerListItemDto
(
    int Id,
    string Name
    );

public class PartnerListItemDtoProfile : Profile
{
    public PartnerListItemDtoProfile()
    {
        CreateMap<Partner, PartnerListItemDto>();
        CreateMap<PartnerListItemDto, Partner>();
    }
}