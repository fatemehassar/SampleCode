using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topup.Application.Features.Purchase.Query
{
    public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery,TransactionDto>
    {
    }
}
