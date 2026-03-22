using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using GeniusStoreERP.Domain.Entities.Transactions;

namespace GeniusStoreERP.Application.Dtos;

public record InvoiceDto(
    int Id,
    int InvoiceNumber,
    DateTime InvoiceDate,
    decimal TotalItemsAmount,
    decimal TotalItemsDiscount,
    decimal TotalItemsTax,
    decimal FinalAmount,
    string? Notes,
    int PartnerId,
    string PartnerName,
    int InvoiceStatusId,
    string InvoiceStatus,
    int InvoiceTypeId,
    string InvoiceType,
    ICollection<InvoiceItemDto> InvoiceItems
    );


public class InvoiceDtoProfile : Profile
{
    public InvoiceDtoProfile()
    {
        CreateMap<Invoice, InvoiceDto>()
              .ForMember(dest => dest.PartnerName, opt => opt.MapFrom(src => src.Partner != null ? src.Partner.Name : string.Empty))
              .ForMember(dest => dest.InvoiceStatus, opt => opt.MapFrom(src => src.InvoiceStatus != null ? src.InvoiceStatus.Name : string.Empty))
              .ForMember(dest => dest.InvoiceType, opt => opt.MapFrom(src => src.InvoiceType != null ? src.InvoiceType.Name : string.Empty));

        CreateMap<InvoiceDto,Invoice>();
    }
}



