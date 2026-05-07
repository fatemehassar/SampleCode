using Polly.CircuitBreaker;
using Switch.Api.Enums;
using Switch.Api.ExceptionHandeling;
using Switch.Api.Models;
using Switch.Api.Persistence;
using Switch.Api.Services;

public class TopupSagaOrchestrator
{
    private readonly AppDbContext _db;
    private readonly MciClient _mci;
    private readonly SwitchService _switch;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public TopupSagaOrchestrator(
        AppDbContext db,
        MciClient mci,
        SwitchService @switch,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _db = db;
        _mci = mci;
        _switch = @switch;
        _logger = logger;
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
            _logger.LogInformation(
                            "Topup Started | TxId:{TxId}",
                            tx.Id);
            topupSuccess =
                await _mci.TopupAsync(
                    tx.MobileNo,
                    tx.Amount);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex,
                "Circuit Breaker Open");

            topupSuccess = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected MCI Error");

            topupSuccess = false;
        }

        if (topupSuccess)
        {
            _logger.LogInformation(
                        "Topup Success | TxId:{TxId}",
                        tx.Id);

            saga.State = SagaState.TopupCompleted;

            await _switch.AdviceAsync(tx.Id);

            saga.State = SagaState.AdviceCompleted;

            tx.Status = TransactionStatus.Completed;
        }
        else
        {
            _logger.LogWarning(
                        "Reverse Executed | TxId:{TxId}",
                        tx.Id);

            await _switch.ReverseAsync(tx.Id);

            saga.State = SagaState.ReverseCompleted;

            tx.Status = TransactionStatus.Reversed;
        }

        await _db.SaveChangesAsync();
    }
}