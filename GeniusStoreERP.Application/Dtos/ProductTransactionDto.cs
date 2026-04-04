using System;
using AutoMapper;
using GeniusStoreERP.Domain.Entities.Stock;

namespace GeniusStoreERP.Application.Dtos;

public class ProductTransactionDto
{
    public ProductTransactionDto() { } // Added parameterless constructor

    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal Quantity { get; set; }
    public int StockTransactionTypeId { get; set; }
    public string? StockTransactionTypeName { get; set; }
    public DateTime TransactionDate { get; set; }
    public int? InvoiceId { get; set; }
    public string? InvoiceReference { get; set; }
    public int? AdjustmentId { get; set; }
    public string? AdjustmentReference { get; set; }
    public string? Remarks { get; set; }
}

public class ProductTransactionDtoProfile : Profile
{
    public ProductTransactionDtoProfile()
    {
        CreateMap<StockTransaction, ProductTransactionDto>()
        .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
        .ForMember(dest => dest.StockTransactionTypeName, opt => opt.MapFrom(src => src.Type != null ? src.Type.Name : string.Empty));
        
        CreateMap<ProductTransactionDto, StockTransaction>();
    }
}