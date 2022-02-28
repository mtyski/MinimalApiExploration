namespace Minimal.Model.Tests;

public sealed class TodoItemTests
{
    private const string DefaultName = "Learn proper unit testing";

    public TodoItemTests()
    {
        TemplateItem = TodoItem.Create(DefaultName);
    }

    private readonly TodoItem TemplateItem;

    [Fact(DisplayName = "New items should be in Created state.")]
    public void NewItemsAreInCreatedState() => 
        TemplateItem.Should()
            .Match<TodoItem>(static ti => ti.Name == DefaultName)
            .And.Match<TodoItem>(static ti => ti.Status == TodoItem.State.Created);

    [Fact(DisplayName = "Renaming an item should properly modify the name of the item")]
    public void RenamedItemHasANewName()
    {
        const string newName = "Learn ASP.NET's minimal APIs";

        TemplateItem.Rename(newName);

        TemplateItem.Name.Should().Be(newName);
    }

    [Theory(DisplayName = "Renaming an item to a whitespace name should throw exception")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void RenamingItemToAWhitespaceStringShouldThrowException(string? name)
    {
        var action = () => TemplateItem.Rename(name!);

        action.Should().Throw<InvalidOperationException>();
    }
}
