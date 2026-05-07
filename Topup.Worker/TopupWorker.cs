using Switch.Api;
using Switch.Api.Persistence;
using System.Transactions;
using TransactionStatus = Switch.Api.TransactionStatus;

public class TopupWorker : BackgroundService
{
    private readonly InMemoryQueue<string> _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly MciClient _mci;

    public TopupWorker(
        InMemoryQueue<string> queue,
        IServiceScopeFactory scopeFactory,
        MciClient mci)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _mci = mci;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var msg in _queue.SubscribeAsync())
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var tx = await db.Transactions.FindAsync(Guid.Parse(msg));

            if (tx.Status != TransactionStatus.Pending)
                continue;

            bool success = false;

            try
            {
                success = await _mci.CallAsync();
            }
            catch
            {
                success = false;
            }

            if (success)
            {
                tx.Status = TransactionStatus.Success;
            }
            else
            {
                // Saga → Reversal
                tx.Status = TransactionStatus.Failed;
            }

            await db.SaveChangesAsync();
        }
    }
}