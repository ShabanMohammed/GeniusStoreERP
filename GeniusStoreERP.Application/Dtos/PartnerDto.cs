using AutoMapper;
using GeniusStoreERP.Domain.Entities.Partners;

namespace GeniusStoreERP.Application.Dtos;

public record PartnerDto
(
    int Id,
    string Name,
    string Email,
    string PhoneNumber,
    string Address,
    bool IsSupplier,
    bool IsCustomer);

public class PartnerDtoProfile : Profile
{
    public PartnerDtoProfile()
    {
        CreateMap<Partner, PartnerDto>();
        CreateMap<PartnerDto, Partner>();
    }
}
