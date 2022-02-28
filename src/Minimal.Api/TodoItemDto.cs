﻿using Minimal.Model;

namespace Minimal.Api;

public record TodoItemDto(string Name, TodoItemDto.ItemStatus Status)
{
    public static TodoItemDto FromTodoItem(TodoItem item) =>
        new(item.Name, (ItemStatus)item.Status);

    public enum ItemStatus
    {
        Created,

        InProgress,

        Done
    }
}
