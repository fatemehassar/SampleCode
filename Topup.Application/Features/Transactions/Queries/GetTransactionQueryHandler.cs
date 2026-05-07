using MediatR;
using Microsoft.EntityFrameworkCore;
using Topup.Application.Features
    .Transactions.DTOs;
using Topup.Application.Interfaces;
using Topup.Domain.Exceptions;

namespace Topup.Application.Features
    .Transactions.Queries;

public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery,TransactionDto>
{
    private readonly IApplicationDbContext _db;

    public GetTransactionQueryHandler(
        IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<TransactionDto> Handle(
        GetTransactionQuery request,
        CancellationToken cancellationToken)
    {
        var tx = await _db.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.TransactionId,
                cancellationToken);

        if (tx == null)
        {
            throw new BusinessException(
                "Transaction not found");
        }

        return new TransactionDto
        {
            Id = tx.Id,
            Amount = tx.Amount,
            MobileNo = tx.MobileNo,
            Status = tx.Status,
            CreatedAt = tx.CreatedAt
        };
    }
}