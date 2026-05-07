using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topup.Application.Features.Purchase.Command;
using Topup.Infrastructure.Persistence;

namespace Topup.UnitTests.Application.Purchase
{
    public class DuplicateTransactionTest
    {
        [Fact]
        public async Task Should_Not_Create_Duplicate_Transaction()
        {
            var options =
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            var db = new AppDbContext(options);

            var handler = new PurchaseCommandHandler(db);

            var key = Guid.NewGuid().ToString();

            var command = new PurchaseCommand(
                1000,
                "09128083537",
                key);

            await handler.Handle(command, CancellationToken.None);

            await handler.Handle(command, CancellationToken.None);

            db.Transactions.Count().Should().Be(1);
        }
    }
}
