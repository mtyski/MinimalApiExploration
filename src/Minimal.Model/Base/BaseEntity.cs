namespace Minimal.Model.Base;

public abstract class BaseEntity<TId>
{
    /// <summary>
    ///     Gets the Id of an entity.
    /// </summary>
    public TId Id { get; private set; }
}
