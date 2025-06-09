using FineService.Aggregates.Fine.Entities;

namespace FineService.DTOs;

public record PaymentReceiptFactoryDto(string Reason, DateTime IssueDate);

public record PaymentReceiptDto(Guid Id, decimal Price, ulong BankCode, ulong AccountCode);

public record FineDto(Guid id, string Reason, DateTime IssueDate, Vehicle Vehicle);