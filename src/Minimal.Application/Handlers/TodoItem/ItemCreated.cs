using Minimal.Redis;
using ItemCreatedEvent = Minimal.Model.TodoItem.DomainEvents.ItemCreated;

namespace Minimal.Application.Handlers.TodoItem;

public class ItemCreated : INotificationHandler<ItemCreatedEvent>
{
    public ItemCreated(RedisRepository redisRepository)
    {
        Repository = redisRepository;
    }

    private RedisRepository Repository { get; }

    /// <inheritdoc />
    public async Task Handle(ItemCreatedEvent notification, CancellationToken cancellationToken) =>
        await Repository.SaveModelAsync<Model.TodoItem, TodoItemDto>(
            notification.Item.Id,
            TodoItemDto.FromTodoItem(notification.Item));
}