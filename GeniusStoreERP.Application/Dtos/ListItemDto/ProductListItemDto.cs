using AutoMapper;
using GeniusStoreERP.Domain.Entities.Stock;

namespace GeniusStoreERP.Application.Dtos.ListItemDto;

public record ProductListItemDto(
    int Id,
    string Name
   );
public class ProductListItemDtoProfile : Profile
{
    public ProductListItemDtoProfile()
    {
        CreateMap<Product, ProductListItemDto>();
        CreateMap<ProductListItemDto, Product>();
    }
}