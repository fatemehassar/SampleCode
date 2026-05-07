using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topup.Domain.Entities;

namespace Topup.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Transaction> Transactions { get; }

        DbSet<OutboxMessage> Outbox { get; }

        DbSet<TopupSaga> Sagas { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
