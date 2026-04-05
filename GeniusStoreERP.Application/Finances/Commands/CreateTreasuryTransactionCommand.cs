using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Domain.Entities.Finances;
using GeniusStoreERP.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Finances.Commands;

public record CreateTreasuryTransactionCommand(
    int TreasuryId,
    decimal Amount,
    DateTime TransactionDate,
    TreasuryTransactionType Type,
    int? PartnerId = null,
    int? InvoiceId = null,
    string? ReferenceNumber = null,
    string? Notes = null
) : IRequest<int>;

public class CreateTreasuryTransactionCommandHandler : IRequestHandler<CreateTreasuryTransactionCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTreasuryTransactionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTreasuryTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _context.BeginTransactionAsync(cancellationToken);

            var treasury = await _context.Treasuries
                .FirstOrDefaultAsync(t => t.Id == request.TreasuryId, cancellationToken);

            if (treasury == null)
                throw new NotFoundException();

            // التحقق من الرصيد في حالة الصرف
            if (request.Type == TreasuryTransactionType.CashOut && treasury.Balance < request.Amount)
            {
                throw new BusinessException("رصيد الخزينة غير كافٍ لإتمام عملية الصرف.");
            }

            // تحديث رصيد الخزينة
            if (request.Type == TreasuryTransactionType.CashIn)
            {
                treasury.Balance += request.Amount;
            }
            else
            {
                treasury.Balance -= request.Amount;
            }

            var transaction = new TreasuryTransaction
            {
                TreasuryId = request.TreasuryId,
                Amount = request.Amount,
                TransactionDate = request.TransactionDate,
                Type = request.Type,
                PartnerId = request.PartnerId,
                InvoiceId = request.InvoiceId,
                ReferenceNumber = request.ReferenceNumber,
                Notes = request.Notes
            };

            _context.TreasuryTransactions.Add(transaction);

            // التأثير على حساب الشريك إذا وجد
            if (request.PartnerId.HasValue)
            {
                var partnerTransaction = new PartnerTransaction
                {
                    PartnerId = request.PartnerId.Value,
                    TransactionDate = request.TransactionDate,
                    ReferenceNumber = request.ReferenceNumber,
                    Remarks = $"حركة خزينة: {request.Notes}",
                    InvoiceId = request.InvoiceId
                };

                if (request.Type == TreasuryTransactionType.CashIn)
                {
                    partnerTransaction.TransactionTypeId = (int)PartnerTransactionTypeEnum.ReceiptVoucher;
                    partnerTransaction.Credit = request.Amount; // القبض يقلل مديونية العميل
                    partnerTransaction.Remarks = $"سند قبض رقم {request.ReferenceNumber} - {request.Notes}";
                }
                else
                {
                    partnerTransaction.TransactionTypeId = (int)PartnerTransactionTypeEnum.PaymentVoucher;
                    partnerTransaction.Debit = request.Amount; // الصرف يقلل مديونية المورد أو يزيد مديونية العميل
                    partnerTransaction.Remarks = $"سند صرف رقم {request.ReferenceNumber} - {request.Notes}";
                }

                _context.PartnerTransactions.Add(partnerTransaction);
            }

            await _context.SaveChangesAsync(cancellationToken);
            await _context.CommitTransactionAsync(cancellationToken);

            return transaction.Id;
        }
        catch (Exception)
        {
            await _context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
