using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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
