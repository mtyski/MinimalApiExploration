using Minimal.Model.Base;

namespace Minimal.Model;

public class TodoItem : BaseEntity
{
    private TodoItem(
        string name,
        State status = State.Created)
    {
        Name = name;
        Status = status;
    }

    protected TodoItem()
    {
    }

    public string Name { get; private set; }

    public State Status { get; private set; }

    public static TodoItem Create(string name)
    {
        var item = new TodoItem(name);
        item.RegisterEvent(new DomainEvents.ItemCreated(item));
        return item;
    }

    public bool CanBeRenamedTo(string newName) => !string.IsNullOrWhiteSpace(newName) && Status != State.Done;

    public bool CanHaveSetStateTo(State state) => Enum.GetValues<State>().Contains(state) && Status != State.Done;

    public void Rename(string newName) =>
        Name = !string.IsNullOrWhiteSpace(newName) ?
        newName :
        throw new InvalidOperationException("New name cannot be empty!");

    public void SetState(State status) => Status = status;

    public enum State
    {
        Created,

        InProgress,

        Done
    }

    public abstract class DomainEvents
    {
        public record ItemCreated(TodoItem Item) : DomainEvent;

        public record ItemDeleted(long ItemId) : DomainEvent;

        public record ItemUpdated(TodoItem Item) : DomainEvent;
    }
}
