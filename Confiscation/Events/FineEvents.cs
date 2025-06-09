using ConfiscationService.Models;

namespace ConfiscationService.Aggregates.Fine.Events;

public enum FineStatus
{
	Confirmed,
	Suspected,
	Rejected,
	Paid
}

public record FineStatusChanged(Guid Id, string Reason, DateTime IssueDate, LicensePlate LicensePlate, FineStatus FineStatus);