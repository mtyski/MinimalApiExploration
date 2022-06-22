using Minimal.Redis;
using ItemDeletedEvent = Minimal.Model.TodoItem.DomainEvents.ItemDeleted;

namespace Minimal.Application.Handlers.TodoItem;

public class ItemDeleted : INotificationHandler<ItemDeletedEvent>
{
    public ItemDeleted(RedisRepository repository)
    {
        Repository = repository;
    }

    private RedisRepository Repository { get; }

    public async Task Handle(ItemDeletedEvent notification, CancellationToken cancellationToken) =>
        await Repository.DeleteModelAsync<Model.TodoItem>(notification.ItemId);
}