using Core.Aggregates.Common;

namespace Core.Aggregates.Fine.Entities;

public class PaymentReceipt : Entity<Guid>
{
    protected PaymentReceipt() {}
    public PaymentReceipt(Guid id, decimal price, ulong bankCode, ulong accountCode)
    {
        Id = id;
        Price = price;
        BankCode = bankCode;
        AccountCode = accountCode;
    }
    public Guid Id { get; init; }
    public decimal Price { get; init; }
    public ulong BankCode { get; init; }
    public ulong AccountCode { get; init; }
    public Guid? PaymentTransactionId { get; private set; }
    public DateTime? PaymentDate { get; private set; }

    public bool Pay(Guid transactionId)
    {
        PaymentTransactionId = transactionId;
        PaymentDate = DateTime.UtcNow;
        return true;
    }
}
