using Microsoft.EntityFrameworkCore;
using Topup.Application.Interfaces;
using Topup.Domain.Entities;

namespace Topup.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IApplicationDbContext    
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Transaction> Transactions => Set<Transaction>();

        public DbSet<TopupSaga> Sagas => Set<TopupSaga>();
        public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();
    }
}
 