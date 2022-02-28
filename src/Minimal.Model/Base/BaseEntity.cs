namespace Minimal.Model.Base;

public abstract class BaseEntity<TId>
{

    protected BaseEntity()
    {
    }

    /// <summary>
    ///     Gets the Id of an entity.
    /// </summary>
    public TId Id { get; } = default!;
}
