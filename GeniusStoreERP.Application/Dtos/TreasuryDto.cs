using AutoMapper;
using GeniusStoreERP.Domain.Entities.Finances;

namespace GeniusStoreERP.Application.Dtos;

public record TreasuryDto(
    int Id,
    string Name,
    string Code,
    decimal Balance,
    string? Description
);

public class TreasuryProfile : Profile
{
    public TreasuryProfile()
    {
        CreateMap<Treasury, TreasuryDto>().ReverseMap();
    }
}
