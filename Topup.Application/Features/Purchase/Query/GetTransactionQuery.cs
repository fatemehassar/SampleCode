using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topup.Application.Features.Purchase.Query
{
    public record GetTransactionQuery(
     Guid TransactionId)
     : IRequest<TransactionDto>;
}
