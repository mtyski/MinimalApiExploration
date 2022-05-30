using Minimal.Redis;
using ItemUpdatedEvent = Minimal.Model.TodoItem.DomainEvents.ItemUpdated;

namespace Minimal.Application.Handlers.TodoItem;

public class ItemUpdated : INotificationHandler<ItemUpdatedEvent>
{
    public ItemUpdated(RedisRepository repository)
    {
        Repository = repository;
    }

    private RedisRepository Repository { get; }

    public async Task Handle(ItemUpdatedEvent notification, CancellationToken cancellationToken) =>
        await Repository.SaveModelAsync<Model.TodoItem, TodoItemDto>(
            notification.Item.Id,
            TodoItemDto.FromTodoItem(notification.Item));
}