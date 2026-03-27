using AutoMapper;
using GeniusStoreERP.Domain.Entities.Stock;

namespace GeniusStoreERP.Application.Products.Queries.GetProductById;

public record ProductDto(
    int Id,
    string Name,
    decimal Price,
    int CategoryId,
    string CategoryName,
    string? Description = null,
    decimal? StockQuantity = 0,
    decimal? ReorderLevel = 0,
    string? SKU = null,
    string? Barcode = null
);

public class ProductDtoProfile : Profile
{
    public ProductDtoProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName,
            opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
        CreateMap<ProductDto, Product>();
    }
}
