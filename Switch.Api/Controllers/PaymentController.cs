using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Switch.Api.Enums;
using Switch.Api.ExceptionHandeling;
using Switch.Api.Models;
using Switch.Api.Persistence;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly AppDbContext _db;

    public PaymentController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> Purchase(
        decimal amount,
        string mobileNo,
        string key)
    {
        if (amount <= 0)
        {
            throw new BusinessException(
                "Amount must be greater than zero");
        }
        var exists = await _db.Transactions
            .FirstOrDefaultAsync(
                x => x.IdempotencyKey == key);

        if (exists != null)
            return Ok(exists.Id);

        var tx = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            MobileNo = mobileNo,
            IdempotencyKey = key,
            Status = TransactionStatus.PendingPurchase,
            CreatedAt = DateTime.UtcNow
        };

        _db.Transactions.Add(tx);

        _db.Outbox.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Payload = tx.Id.ToString()
        });

        await _db.SaveChangesAsync();

        return Ok(new
        {
            tx.Id,
            tx.Status
        });
    }
}