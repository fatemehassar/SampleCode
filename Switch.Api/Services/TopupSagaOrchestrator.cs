using Switch.Api.Enums;
using Switch.Api.Models;
using Switch.Api.Persistence;
using Switch.Api.Services;

public class TopupSagaOrchestrator
{
    private readonly AppDbContext _db;
    private readonly MciClient _mci;
    private readonly SwitchService _switch;

    public TopupSagaOrchestrator(
        AppDbContext db,
        MciClient mci,
        SwitchService @switch)
    {
        _db = db;
        _mci = mci;
        _switch = @switch;
    }

    public async Task ExecuteAsync(Transaction tx)
    {
        var saga = new TopupSaga
        {
            Id = Guid.NewGuid(),
            TransactionId = tx.Id,
            State = SagaState.Started,
            CreatedAt = DateTime.UtcNow
        };

        _db.Sagas.Add(saga);

        saga.State = SagaState.PurchaseCompleted;

        tx.Status = TransactionStatus.TopupProcessing;

        await _db.SaveChangesAsync();

        bool topupSuccess = false;

        try
        {
            topupSuccess =
                await _mci.TopupAsync(
                    tx.MobileNo,
                    tx.Amount);
        }
        catch
        {
            topupSuccess = false;
        }

        if (topupSuccess)
        {
            saga.State = SagaState.TopupCompleted;

            await _switch.AdviceAsync(tx.Id);

            saga.State = SagaState.AdviceCompleted;

            tx.Status = TransactionStatus.Completed;
        }
        else
        {
            await _switch.ReverseAsync(tx.Id);

            saga.State = SagaState.ReverseCompleted;

            tx.Status = TransactionStatus.Reversed;
        }

        await _db.SaveChangesAsync();
    }
}