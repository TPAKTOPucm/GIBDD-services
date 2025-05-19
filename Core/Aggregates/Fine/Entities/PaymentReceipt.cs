using Core.Aggregates.Common;

namespace Core.Aggregates.Fine.Entities;

public class PaymentReceipt : Entity<Guid>
{
    public PaymentReceipt(Guid id, decimal price, ulong bankCode, ulong accountCode)
    {
        Id = id;
        Price = price;
        BankCode = bankCode;
        AccountCode = accountCode;
    }
    public Guid Id { get; set; }
    public decimal Price { get; }
    public ulong BankCode { get; }
    public ulong AccountCode { get; }
    public Guid? PaymentTransactionId { get; private set; }
    public DateTime? PaymentDate { get; private set; }

    public bool Pay(Guid transactionId)
    {
        PaymentTransactionId = transactionId;
        PaymentDate = DateTime.UtcNow;
        return true;
    }
}
