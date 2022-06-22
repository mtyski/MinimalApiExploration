using MediatR;

namespace Minimal.Model.Base;

public abstract record DomainEvent : INotification
{
    public DateTime OccuredOn { get; } = DateTime.Now;
}