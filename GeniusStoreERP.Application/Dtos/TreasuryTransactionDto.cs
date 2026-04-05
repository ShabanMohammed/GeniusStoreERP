using AutoMapper;
using GeniusStoreERP.Domain.Entities.Finances;

namespace GeniusStoreERP.Application.Dtos;

public record TreasuryTransactionDto(
    int Id,
    int TreasuryId,
    string? TreasuryName,
    decimal Amount,
    DateTime TransactionDate,
    TreasuryTransactionType Type,
    int? PartnerId,
    string? PartnerName,
    int? InvoiceId,
    string? ReferenceNumber,
    string? Notes
);

public class TreasuryTransactionProfile : Profile
{
    public TreasuryTransactionProfile()
    {
        CreateMap<TreasuryTransaction, TreasuryTransactionDto>()
            .ForMember(d => d.TreasuryName, opt => opt.MapFrom(s => s.Treasury != null ? s.Treasury.Name : null))
            .ForMember(d => d.PartnerName, opt => opt.MapFrom(s => s.Partner != null ? s.Partner.Name : null));
        
        CreateMap<TreasuryTransactionDto, TreasuryTransaction>();
    }
}
