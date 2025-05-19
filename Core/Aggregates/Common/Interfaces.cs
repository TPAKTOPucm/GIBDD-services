using MediatR;

namespace Core.Aggregates.Common;

public interface IAggregateRoot
{
    IReadOnlyList<IDomainEvent> GetDomainEvents();
    void AddDomainEvent(IDomainEvent domainEvent);
    void ClearDomainEvents();
}

public interface IDomainEvent : INotification {}