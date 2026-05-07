using MediatR;
using Topup.Application.Interfaces;
using Topup.Domain.Entities;
using Topup.Domain.Exceptions;
using Transaction = Topup.Domain.Entities.Transaction;

namespace Topup.Application.Features.Purchase.Command
{
    public class PurchaseCommandHandler
     : IRequestHandler<PurchaseCommand, Guid>
    {
        private readonly IApplicationDbContext _db;

        public PurchaseCommandHandler(
            IApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Guid> Handle(
            PurchaseCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
            {
                throw new BusinessException(
                    "Amount must be greater than zero");
            }
            var exists = _db.Transactions
                .FirstOrDefault(x =>
                    x.IdempotencyKey ==
                    request.IdempotencyKey);

            if (exists != null)
                return exists.Id;

            var tx = new Transaction(
                request.Amount,
                request.MobileNo,
                request.IdempotencyKey);

            _db.Transactions.Add(tx);

            _db.Outbox.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Payload = tx.Id.ToString()
            });

            await _db.SaveChangesAsync(cancellationToken);

            return tx.Id;
        }
    }
}
