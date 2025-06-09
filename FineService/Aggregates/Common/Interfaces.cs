namespace FineService.Aggregates.Common;

public interface IAggregateRoot
{
    IReadOnlyList<IDomainEvent> GetDomainEvents();
    void AddDomainEvent(IDomainEvent domainEvent);
    void ClearDomainEvents();
}

public interface IDomainEvent {}