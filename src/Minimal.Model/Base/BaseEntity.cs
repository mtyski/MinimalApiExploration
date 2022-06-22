namespace Minimal.Model.Base;

public abstract class BaseEntity
{
    private readonly List<DomainEvent> events;

    protected BaseEntity()
    {
        events = new();
    }

    /// <summary>
    ///     Gets the Id of an entity.
    /// </summary>
    public long Id { get; private set; }

    public IReadOnlyCollection<DomainEvent> Events => events.AsReadOnly();

    public void RegisterEvent<TEvent>(TEvent @event)
        where TEvent : DomainEvent =>
        events.Add(@event);
}
