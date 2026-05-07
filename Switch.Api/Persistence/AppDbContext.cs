using Microsoft.EntityFrameworkCore;
using Switch.Api.Models;

namespace Switch.Api.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Transaction> Transactions => Set<Transaction>();

        public DbSet<TopupSaga> Sagas => Set<TopupSaga>();
        public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();
    }
}
