using Microsoft.Extensions.Logging;
using Topup.Application.Interfaces;
using Topup.Domain.Entities;
using Topup.Domain.Enums;
using Polly.CircuitBreaker;

namespace Topup.Application.Features.Topup;

public class TopupSagaOrchestrator
{
    private readonly IApplicationDbContext _db;

    private readonly IMciClient _mci;

    private readonly ISwitchService _switch;

    private readonly ILogger<TopupSagaOrchestrator>
        _logger;

    public TopupSagaOrchestrator(
        IApplicationDbContext db,
        IMciClient mci,
        ISwitchService @switch,
        ILogger<TopupSagaOrchestrator> logger)
    {
        _db = db;
        _mci = mci;
        _switch = @switch;
        _logger = logger;
    }

    public async Task ExecuteAsync(
        Transaction tx,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Saga Started | TxId:{TxId}",
            tx.Id);

        var saga = new TopupSaga
        {
            Id = Guid.NewGuid(),
            TransactionId = tx.Id,
            State = SagaState.Started,
            CreatedAt = DateTime.UtcNow
        };

        _db.Sagas.Add(saga);

        saga.State = SagaState.PurchaseCompleted;

        tx.MarkProcessing();

        await _db.SaveChangesAsync(
            cancellationToken);

        bool topupSuccess;

        try
        {
            _logger.LogInformation(
                "Topup Request Started | TxId:{TxId}",
                tx.Id);

            topupSuccess =
                await _mci.TopupAsync(
                    tx.MobileNo,
                    tx.Amount);

            _logger.LogInformation(
                "Topup Response Received | TxId:{TxId} | Success:{Success}",
                tx.Id,
                topupSuccess);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(
                ex,
                "Circuit Breaker Open | TxId:{TxId}",
                tx.Id);

            topupSuccess = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected MCI Error | TxId:{TxId}",
                tx.Id);

            topupSuccess = false;
        }

        if (topupSuccess)
        {
            await HandleSuccessAsync(
                tx,
                saga,
                cancellationToken);
        }
        else
        {
            await HandleFailureAsync(
                tx,
                saga,
                cancellationToken);
        }

        _logger.LogInformation(
            "Saga Finished | TxId:{TxId} | Status:{Status}",
            tx.Id,
            tx.Status);
    }

    private async Task HandleSuccessAsync(
        Transaction tx,
        TopupSaga saga,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Executing Advice | TxId:{TxId}",
            tx.Id);

        saga.State = SagaState.TopupCompleted;

        await _switch.AdviceAsync(tx.Id);

        saga.State = SagaState.AdviceCompleted;

        tx.MarkCompleted();

        await _db.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Advice Completed | TxId:{TxId}",
            tx.Id);
    }

    private async Task HandleFailureAsync(
        Transaction tx,
        TopupSaga saga,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogWarning(
                "Executing Reverse | TxId:{TxId}",
                tx.Id);

            await _switch.ReverseAsync(tx.Id);

            saga.State =
                SagaState.ReverseCompleted;

            tx.MarkReversed();

            await _db.SaveChangesAsync(
                cancellationToken);

            _logger.LogWarning(
                "Reverse Completed | TxId:{TxId}",
                tx.Id);
        }
        catch (Exception ex)
        {
            saga.State = SagaState.Failed;

            tx.MarkFailed();

            await _db.SaveChangesAsync(
                cancellationToken);

            _logger.LogCritical(
                ex,
                "Reverse Failed | TxId:{TxId}",
                tx.Id);

            throw;
        }
    }
}