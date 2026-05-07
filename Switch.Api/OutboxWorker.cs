using Switch.Api.Persistence;
namespace Switch.Api
{
    public class OutboxWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly InMemoryQueue<string> _queue;

        public OutboxWorker(IServiceScopeFactory scopeFactory, InMemoryQueue<string> queue)
        {
            _scopeFactory = scopeFactory;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var messages = db.Outbox.Where(x => !x.Processed).ToList();

                foreach (var msg in messages)
                {
                    await _queue.PublishAsync(msg.Payload);
                    msg.Processed = true;
                }

                await db.SaveChangesAsync();

                await Task.Delay(500);
            }
        }
    }
}
