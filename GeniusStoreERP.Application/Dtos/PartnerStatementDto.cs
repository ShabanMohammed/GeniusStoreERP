using System;
using System.Collections.Generic;

namespace GeniusStoreERP.Application.Dtos
{
    public record PartnerStatementDto(
        PartnerDto Partner,
        decimal OpeningBalance,
        List<PartnerStatementItemDto> Items,
        decimal ClosingBalance
    );

    public record PartnerStatementItemDto(
        int TransactionId,
        DateTime Date,
        string TransactionType,
        string? ReferenceNumber,
        string? Remarks,
        decimal Debit,
        decimal Credit,
        decimal RunningBalance
    );
}
