using FineService.Aggregates.Common;
using FineService.Aggregates.Fine.Entities;
using FineService.Aggregates.Fine.Events;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FineService.Aggregates.Fine;

public class Fine : Aggregate<Guid>
{
    ILazyLoader _lazyLoader;
    Vehicle _vehicle;
    protected Fine(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }
    public Fine(Guid id, string reason, DateTime issueDate, Vehicle vehicle)
    {
        if(reason is null || vehicle is null)
            throw new ArgumentNullException();
        Id = id;
        Reason = reason;
        IssueDate = issueDate;
        Vehicle = vehicle;
        Status = FineStatus.Suspected;
    }
    public Guid Id { get; init; }
    public PaymentReceipt? Receipt { get; private set; }
    public Vehicle Vehicle { get => _lazyLoader.Load(this, ref _vehicle); private set => _vehicle = value; }
    public string Reason { get; init; }
    public DateTime IssueDate { get; init; }
    public DateTime? ConfirmationDate { get; private set; }
    public FineStatus Status { get; private set; }

    public bool Confirm(PaymentReceipt receipt)
    {
        if (Status != FineStatus.Suspected || receipt == null)
            return false;
        receipt.FineId = Id;
        Receipt = receipt;
        Status = FineStatus.Confirmed;
        ConfirmationDate = DateTime.UtcNow;
        AddDomainEvent(new FineStatusChangedEvent(Id, Reason, IssueDate, (DateTime)ConfirmationDate, Vehicle.LicensePlate, Receipt.Price, FineStatus.Confirmed));
        return true;
    }

    public bool Pay(Guid transactionId)
    {
        if (Status == FineStatus.Confirmed && Receipt.Pay(transactionId))
        {
            Status = FineStatus.Paid;
            AddDomainEvent(new FineStatusChangedEvent(Id, Reason, IssueDate, (DateTime)ConfirmationDate, Vehicle.LicensePlate, Receipt.Price, FineStatus.Paid));
            return true;
        }
        return false;
    }

    public bool Reject()
    {
        switch (Status)
        {
            case FineStatus.Confirmed:
                Receipt.Reject();
                goto case FineStatus.Suspected;
			case FineStatus.Suspected:
                Status = FineStatus.Rejected;
                AddDomainEvent(new FineRejectedEvent(Id, Reason, IssueDate, Vehicle.LicensePlate));
                return true;
        }
        return false;
    }
}
