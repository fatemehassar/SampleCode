using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Topup.Application.Features.Topup;
using Topup.Application.Interfaces;
using Topup.Domain.Entities;
using Topup.Domain.Enums;
using Topup.Infrastructure.Persistence;
using Xunit;

namespace Topup.UnitTests.Application.Saga;

public class TopupSagaReverseTests
{
    [Fact]
    public async Task Should_Reverse_When_Mci_Fails()
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
            .ReturnsAsync(false);

        var switchMock = new Mock<ISwitchService>();

        var logger =
            new Mock<ILogger<TopupSagaOrchestrator>>();

        var orchestrator = new TopupSagaOrchestrator(
            db,
            mciMock.Object,
            switchMock.Object,
            logger.Object);

        await orchestrator.ExecuteAsync(
            tx,
            CancellationToken.None);

        tx.Status.Should()
            .Be(TransactionStatus.Reversed);

        switchMock.Verify(
            x => x.ReverseAsync(tx.Id),
            Times.Once);
    }
}
