using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Topup.Application.Features.Purchase.Command;
using Topup.Infrastructure.Persistence;
using Topup.Domain.Entities;
using Xunit;

namespace Topup.UnitTests.Application.Purchase
{
 
    public class PurchaseCommandHandlerTests
    {
        //تست PurchaseCommandHandler
        [Fact]
        public async Task Should_Create_Transaction()
        {
            // Arrange

            var options =
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            var db = new AppDbContext(options);

            var handler = new PurchaseCommandHandler(db);

            var command = new PurchaseCommand(
                10000,
                "09128083537",
                Guid.NewGuid().ToString());

            // Act

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            // Assert

            result.Should().NotBeEmpty();

            db.Transactions.Count()
                .Should().Be(1);
        }
    }
}
