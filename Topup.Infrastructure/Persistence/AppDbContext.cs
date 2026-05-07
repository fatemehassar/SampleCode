using Microsoft.EntityFrameworkCore;
using Topup.Application.Interfaces;
using Topup.Domain.Entities;

namespace Topup.Infrastructure.Persistence
{
    public class AppDbContext
     : DbContext,
       IApplicationDbContext
    {
        public DbSet<Transaction> Transactions => throw new NotImplementedException();

        public DbSet<OutboxMessage> Outbox => throw new NotImplementedException();

        public DbSet<TopupSaga> Sagas => throw new NotImplementedException();
    }
}
