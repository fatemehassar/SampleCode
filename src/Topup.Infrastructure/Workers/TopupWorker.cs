using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Topup.Application.Features.Topup;
using Topup.Domain.Enums;
using Topup.Infrastructure.Persistence;
using Topup.Infrastructure.Queue;

namespace Topup.Infrastructure.Workers
{
    public class TopupWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly InMemoryQueue<string> _queue;

        public TopupWorker(
            IServiceScopeFactory scopeFactory,
            InMemoryQueue<string> queue)
        {
            _scopeFactory = scopeFactory;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var msg
                in _queue.SubscribeAsync())
            {
                using var scope =
                    _scopeFactory.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                var orchestrator =
                    scope.ServiceProvider
                    .GetRequiredService<TopupSagaOrchestrator>();

                var tx = await db.Transactions
                    .FindAsync(Guid.Parse(msg));

                if (tx == null)
                    continue;

                if (tx.Status ==
                        TransactionStatus.Completed
                    ||
                    tx.Status ==
                        TransactionStatus.Reversed)
                    continue;

                await orchestrator.ExecuteAsync(tx, stoppingToken);
            }
        }
    }
}
