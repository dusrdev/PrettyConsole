namespace PrettyConsole.Tests.Unit;

public class MenusTests {
    [Fact]
    public void Selection_ReturnsSelectedChoice_WhenInputValid() {
        Out = Utilities.GetWriter(out var writer);
        In = Utilities.GetReader("2");

        var choices = new List<string> { "Apple", "Banana", "Cherry" };

        var result = Selection(["Choose a fruit:"], choices);

        var output = writer.ToStringAndFlush();

        // Do not remove extra whitespace
        Assert.Equal(
            """
            Choose a fruit:
             1) Apple
             2) Banana
             3) Cherry

            Enter your choice: 
            """
        , output);
        Assert.Equal("Banana", result);
    }

    [Fact]
    public void Selection_InvalidNumber_ReturnsEmptyString() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("5");

        var choices = new List<string> { "One", "Two" };

        var result = Selection(["Pick a number:"], choices);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Selection_NonNumericInput_ReturnsEmptyString() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("abc");

        var choices = new List<string> { "First", "Second" };

        var result = Selection(["Pick a number:"], choices);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void MultiSelection_ReturnsSelectedChoices_InOrder() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("3 1");

        var choices = new List<string> { "Mercury", "Venus", "Earth" };

        var result = MultiSelection(["Planets:"], choices);

        Assert.Equal(["Earth", "Mercury"], result);
    }

    [Fact]
    public void MultiSelection_InvalidEntry_ReturnsEmptyArray() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("2 x");

        var choices = new List<string> { "Alpha", "Beta", "Gamma" };

        var result = MultiSelection(["Letters:"], choices);

        Assert.Empty(result);
    }

    [Fact]
    public void MultiSelection_EmptyInput_ReturnsEmptyArray() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader(string.Empty);

        var choices = new List<string> { "Alpha", "Beta" };

        var result = MultiSelection(["Letters:"], choices);

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

        var (option, subOption) = TreeMenu(["Menu:"], menu);

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

        Assert.Throws<ArgumentException>(() => TreeMenu(["Menu:"], menu));
    }

    [Fact]
    public void TreeMenu_InvalidIndexes_ThrowsArgumentException() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("3 1");

        var menu = new Dictionary<string, IList<string>> {
            ["Files"] = new List<string> { "Open" },
            ["Edit"] = new List<string> { "Undo" }
        };

        Assert.Throws<ArgumentException>(() => TreeMenu(["Menu:"], menu));
    }

    [Fact]
    public void Table_WritesHeaderAndRows() {
        Out = Utilities.GetWriter(out var writer);

        var headers = new List<string> { "Name", "Age" };
        var column1 = new List<string> { "Alice", "Bob" };
        var column2 = new List<string> { "30", "25" };

        Table(headers, [column1, column2]);

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

        Assert.Throws<ArgumentException>(() => Table(headers, [column1]));
    }
}