using Core.Aggregates.Common;
using Core.Aggregates.Fine.Entities;
using Core.Aggregates.Fine.Events;
using Core.Aggregates.VehicleRegistration.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Core.Aggregates.Fine;

public class Fine : Aggregate<Guid>
{
    ILazyLoader _lazyLoader;
    PaymentReceipt _paymentReceipt;
    protected Fine(ILazyLoader lazyLoader) { }
    public Fine(string reason, DateTime issueDate, PaymentReceipt paymentReceipt, Vehicle vehicle)
    {
        if(reason is null || paymentReceipt is null || vehicle is null)
            throw new ArgumentNullException();
        Reason = reason;
        IssueDate = issueDate;
        _paymentReceipt = paymentReceipt;
        Vehicle = vehicle;
        AddDomainEvent(new FineCreatedEvent(Id, Reason, issueDate, CreationDate, vehicle.LicensePlate, _paymentReceipt.Price));
    }
    public Guid Id { get; }
    public PaymentReceipt Receipt { get => _lazyLoader.Load(this, ref _paymentReceipt); }
    public Vehicle Vehicle { get; }
    public bool IsPaid { get => Receipt.PaymentTransactionId is not null; }
    public string Reason { get; }
    public DateTime IssueDate { get; }
    public DateTime CreationDate { get; } = DateTime.UtcNow;

    public bool Pay(Guid transactionId)
    {
        return Receipt.Pay(transactionId);
    }
}
