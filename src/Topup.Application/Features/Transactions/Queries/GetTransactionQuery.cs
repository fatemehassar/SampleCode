using MediatR;
using Topup.Application.Features.Transactions.DTOs;

namespace Topup.Application.Features.Transactions.Queries
{
    public record GetTransactionQuery(
     Guid TransactionId)
     : IRequest<TransactionDto>;
}
