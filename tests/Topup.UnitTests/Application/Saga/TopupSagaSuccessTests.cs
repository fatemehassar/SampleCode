using FluentAssertions;
using global::Topup.Application.Features.Topup;
using global::Topup.Application.Interfaces;
using global::Topup.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Topup.Domain.Entities;
using Topup.Domain.Enums;

namespace Topup.UnitTests.Application.Saga;

public class TopupSagaSuccessTests
{
    // تست Saga Success Flow
    [Fact]
    public async Task Should_Complete_Transaction_When_Mci_Succeeds()
    {
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        var db = new AppDbContext(options);

        var tx = new Transaction(
            1000,
            "09128083537",
            Guid.NewGuid().ToString());

        db.Transactions.Add(tx);

        await db.SaveChangesAsync();

        var mciMock = new Mock<IMciClient>();

        mciMock
            .Setup(x => x.TopupAsync(
                It.IsAny<string>(),
                It.IsAny<decimal>()))
            .ReturnsAsync(true);

        var switchMock = new Mock<ISwitchService>();

        var logger =
            new Mock<ILogger<TopupSagaOrchestrator>>();

        var saga = new TopupSagaOrchestrator(
            db,
            mciMock.Object,
            switchMock.Object,
            logger.Object);

        await saga.ExecuteAsync(
            tx,
            CancellationToken.None);

        tx.Status.Should()
            .Be(TransactionStatus.Completed);
    }
}

