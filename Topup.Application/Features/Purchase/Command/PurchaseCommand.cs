using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topup.Application.Features.Purchase.Command
{
    public record PurchaseCommand(
        decimal Amount,
        string MobileNo,
        string IdempotencyKey
    ) :IRequest<Guid>;
}
