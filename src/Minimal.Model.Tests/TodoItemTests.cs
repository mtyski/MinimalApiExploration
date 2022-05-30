using System.Linq;
using static FluentAssertions.FluentActions;

namespace Minimal.Model.Tests;

public sealed class TodoItemTests
{
    private const string DefaultName = "Learn proper unit testing";

    private readonly TodoItem templateItem;

    public TodoItemTests()
    {
        templateItem = TodoItem.Create(DefaultName);
    }

    [Fact(DisplayName = "New items should be in Created state.")]
    public void NewItemsAreInCreatedState() =>
        templateItem.Should()
            .Match<TodoItem>(static ti => ti.Name == DefaultName)
            .And.Match<TodoItem>(static ti => ti.Status == TodoItem.State.Created)
            .And.Match<TodoItem>(
                static ti => ti.Events.OfType<TodoItem.DomainEvents.ItemCreated>()
                    .Any());

    [Fact(DisplayName = "Renaming an item should properly modify the name of the item")]
    public void RenamedItemHasANewName()
    {
        const string newName = "Learn ASP.NET's minimal APIs";

        templateItem.Rename(newName);

        templateItem.Name.Should()
            .Be(newName);
    }

    [Theory(DisplayName = "Renaming an item to a whitespace name should throw exception")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void RenamingItemToAWhitespaceStringShouldThrowException(string? name) =>
        Invoking(() => templateItem.Rename(name!))
            .Should()
            .Throw<InvalidOperationException>();
}