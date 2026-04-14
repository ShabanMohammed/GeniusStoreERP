using System;
using System.Collections.Generic;
using AutoMapper;
using GeniusStoreERP.Domain.Entities.Stock;

namespace GeniusStoreERP.Application.Dtos;

public class StockAdjustmentDto
{
    public int Id { get; set; }
    public DateTime AdjustmentDate { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public int TotalProducts { get; set; }
    public List<StockAdjustmentItemDto> Items { get; set; } = new();
}

public class StockAdjustmentItemDto
{
    public int Id { get; set; }
    public int StockAdjustmentId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal PreviousQuantity { get; set; }
    public decimal QuantityChange { get; set; }
    public decimal NewQuantity { get; set; }
    public int StockTransactionTypeId { get; set; }
    public string? StockTransactionTypeName { get; set; }
}

public class StockAdjustmentProfile : Profile
{
    public StockAdjustmentProfile()
    {
        CreateMap<StockAdjustment, StockAdjustmentDto>()
            .ForMember(d => d.TotalProducts, opt => opt.MapFrom(s => s.Items.Count));
            
        CreateMap<StockAdjustmentItem, StockAdjustmentItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
            .ForMember(d => d.StockTransactionTypeName, opt => opt.MapFrom(s => s.StockTransactionType != null ? s.StockTransactionType.Name : string.Empty));
    }
}
