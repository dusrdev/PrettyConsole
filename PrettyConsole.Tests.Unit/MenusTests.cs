namespace PrettyConsole.Tests.Unit;

public class MenusTests {
    [Fact]
    public void Selection_ReturnsSelectedChoice_WhenInputValid() {
        Out = Utilities.GetWriter(out var writer);
        In = Utilities.GetReader("2");

        var choices = new List<string> { "Apple", "Banana", "Cherry" };

        var result = Console.Selection(choices, $"Choose a fruit:");

        var output = writer.ToStringAndFlush();

        Assert.Equal(
            """
            Choose a fruit:
             1) Apple
             2) Banana
             3) Cherry

            Enter your choice: 
            """.Replace("\r\n", "\n"),
            Normalize(output));
        Assert.Equal("Banana", result);

        static string Normalize(string value) => value.Replace("\r\n", "\n");
    }

    [Fact]
    public void Selection_InvalidNumber_ReturnsEmptyString() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("5");

        var choices = new List<string> { "One", "Two" };

        var result = Console.Selection(choices, $"Pick a number: ");

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Selection_NonNumericInput_ReturnsEmptyString() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("abc");

        var choices = new List<string> { "First", "Second" };

        var result = Console.Selection(choices, $"Pick a number: ");

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void MultiSelection_ReturnsSelectedChoices_InOrder() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("3 1");

        var choices = new List<string> { "Mercury", "Venus", "Earth" };

        var result = Console.MultiSelection(choices, $"Plants: ");

        Assert.Equal(["Earth", "Mercury"], result);
    }

    [Fact]
    public void MultiSelection_InvalidEntry_ReturnsEmptyArray() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("2 x");

        var choices = new List<string> { "Alpha", "Beta", "Gamma" };

        var result = Console.MultiSelection(choices, $"Letters: ");

        Assert.Empty(result);
    }

    [Fact]
    public void MultiSelection_EmptyInput_ReturnsEmptyArray() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader(string.Empty);

        var choices = new List<string> { "Alpha", "Beta" };

        var result = Console.MultiSelection(choices, $"Letters: ");

        Assert.Empty(result);
    }

    [Fact]
    public void TreeMenu_ValidSelection_ReturnsTuple() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("2 1");

        var menu = new Dictionary<string, IList<string>> {
            ["Files"] = new List<string> { "Open", "Save" },
            ["Edit"] = new List<string> { "Undo", "Redo" }
        };

        var (option, subOption) = Console.TreeMenu(menu, $"Menu: ");

        Assert.Equal("Edit", option);
        Assert.Equal("Undo", subOption);
    }

    [Fact]
    public void TreeMenu_MissingSelectionParts_ThrowsArgumentException() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("1");

        var menu = new Dictionary<string, IList<string>> {
            ["Files"] = new List<string> { "Open" }
        };

        Assert.Throws<ArgumentException>(() => Console.TreeMenu(menu, $"Menu: "));
    }

    [Fact]
    public void TreeMenu_InvalidIndexes_ThrowsArgumentException() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("3 1");

        var menu = new Dictionary<string, IList<string>> {
            ["Files"] = new List<string> { "Open" },
            ["Edit"] = new List<string> { "Undo" }
        };

        Assert.Throws<ArgumentException>(() => Console.TreeMenu(menu, $"Menu: "));
    }

    [Fact]
    public void Table_WritesHeaderAndRows() {
        Out = Utilities.GetWriter(out var writer);

        var headers = new List<string> { "Name", "Age" };
        var column1 = new List<string> { "Alice", "Bob" };
        var column2 = new List<string> { "30", "25" };

        Console.Table(headers, [column1, column2]);

        var output = writer.ToString();
        Assert.Contains("Name", output);
        Assert.Contains("Age", output);
        Assert.Contains("Alice", output);
        Assert.Contains("Bob", output);
    }

    [Fact]
    public void Table_DifferentHeaderAndColumnCounts_ThrowsArgumentException() {
        Out = Utilities.GetWriter(out _);

        var headers = new List<string> { "Name", "Age" };
        var column1 = new List<string> { "Alice", "Bob" };

        Assert.Throws<ArgumentException>(() => Console.Table(headers, [column1]));
    }
}