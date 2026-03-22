using AutoMapper;
using GeniusStoreERP.Domain.Entities.Stock;

namespace GeniusStoreERP.Application.Dtos.ListItemDto;

public record CategoryListItemDto(int Id, string Name);

public class CategoryListItemDtoProfile : Profile
{
    public CategoryListItemDtoProfile()
    {
        CreateMap<Category, CategoryListItemDto>();
        CreateMap<CategoryListItemDto, Category>();
    }
}
