using Minimal.Model.Base;

namespace Minimal.Model;

public class TodoItem : BaseEntity<long>
{
    private TodoItem(string name)
        : this(name, State.Created)
    {
    }

    private TodoItem(
        string name,
        State status)
    {
        Name = name;
        Status = status;
    }

    public string Name { get; private set; }

    public State Status { get; private set; }

    public static TodoItem Create(string name) => new(name);

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
}
