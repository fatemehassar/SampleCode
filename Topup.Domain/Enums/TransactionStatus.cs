namespace Topup.Domain.Enums
{
    public enum TransactionStatus
    {
        PendingPurchase,
        TopupProcessing,
        Completed,
        Reversed,
        Failed
    }
}
