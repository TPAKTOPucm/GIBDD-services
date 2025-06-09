using ConfiscationService.Models;

namespace ConfiscationService.Events;

public record ConfiscationEvent
{
	public Guid Id { get; }
	public LicensePlate LicensePlate { get; }
	public ConfiscationReason Reason { get; }
	public string Comment { get; }
	public Address? ImpoundYardAddress { get; }
	public bool IsReturned { get; }
	public DateTime ExecutionDate { get; }

	public ConfiscationEvent(Guid id, LicensePlate licensePlate, ConfiscationReason reason, string comment, Address impoundYardAddress, bool isReturned, DateTime executionDate)
	{
		Id = id;
		LicensePlate = licensePlate;
		Reason = reason;
		Comment = comment;
		ImpoundYardAddress = impoundYardAddress;
		IsReturned = isReturned;
		ExecutionDate = executionDate;
	}
	public ConfiscationEvent(ConfiscationOrder order, DateTime executionDate)
	{
		Id = order.Id;
		LicensePlate = order.LicensePlate;
		Reason = order.Reason;
		Comment = order.Comment;
		ImpoundYardAddress = order.ImpoundYardAddress;
		IsReturned = order.IsReturned;
		ExecutionDate = executionDate;
	}
}
