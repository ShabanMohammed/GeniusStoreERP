using AutoMapper;
using GeniusStoreERP.Domain.Entities.Stock;
using GeniusStoreERP.Domain.Entities.Transactions;
using System.Collections.Generic;
using System.Text;

namespace GeniusStoreERP.Application.Dtos;

public record InvoiceItemDto(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountRate,
    decimal DisCountAmount,
    decimal TaxRate,
    decimal TaxAmount,
    decimal NetLineTotal
)
{
    public decimal LineTotal => Quantity * UnitPrice;
}


public class InvoiceItemDtoProfile : Profile
{
    public InvoiceItemDtoProfile()
    {
        CreateMap<InvoiceItem, InvoiceItemDto>()
            .ForCtorParam("ProductName", opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty));
        CreateMap<InvoiceItemDto, InvoiceItem>();

    }
}
