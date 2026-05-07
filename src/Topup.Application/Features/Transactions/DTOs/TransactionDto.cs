using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topup.Domain.Enums;

namespace Topup.Application.Features.Transactions.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public string MobileNo { get; set; }

        public TransactionStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
