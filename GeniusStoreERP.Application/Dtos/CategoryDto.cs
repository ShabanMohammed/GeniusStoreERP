using AutoMapper;
using GeniusStoreERP.Domain.Entities.Stock;

namespace GeniusStoreERP.Application.Dtos;

public record CategoryDto(int Id, string Name, string? Description = null);

public class CategoryDtoProfile : Profile
{
    public CategoryDtoProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryDto, Category>();
    }
}
