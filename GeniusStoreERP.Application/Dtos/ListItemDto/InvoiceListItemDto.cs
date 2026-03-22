using AutoMapper;
using GeniusStoreERP.Domain.Entities.Transactions;

namespace GeniusStoreERP.Application.Dtos.ListItemDto;

public record InvoiceListItemDto
(
    int Id,
    int InvoiceNumber,
    string PartnerName
);

public class InvoiceListItemDtoProfile : Profile
{
    public InvoiceListItemDtoProfile()
    {
        CreateMap<Invoice, InvoiceListItemDto>()
            .ForMember(dest => dest.PartnerName, opt => opt.MapFrom(src => src.Partner != null ? src.Partner.Name : string.Empty));
    }
}
