namespace PrettyConsole.UnitTests;

public class MenusTests {
    [Test]
    public async Task Selection_ReturnsSelectedChoice_WhenInputValid() {
        Out = Utilities.GetWriter(out var writer);
        In = Utilities.GetReader("2");

        var choices = new List<string> { "Apple", "Banana", "Cherry" };

        var result = Console.Selection(choices, $"Choose a fruit:");

        var output = writer.ToStringAndFlush();

        await Assert.That(Normalize(output)).IsEqualTo(
            """
            Choose a fruit:
             1) Apple
             2) Banana
             3) Cherry

            Enter your choice: 
            """.Replace("\r\n", "\n"));
        await Assert.That(result).IsEqualTo("Banana");

        static string Normalize(string value) => value.Replace("\r\n", "\n");
    }

    [Test]
    public async Task Selection_InvalidNumber_ReturnsEmptyString() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("5");

        var choices = new List<string> { "One", "Two" };

        var result = Console.Selection(choices, $"Pick a number: ");

        await Assert.That(result).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task Selection_NonNumericInput_ReturnsEmptyString() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("abc");

        var choices = new List<string> { "First", "Second" };

        var result = Console.Selection(choices, $"Pick a number: ");

        await Assert.That(result).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task MultiSelection_ReturnsSelectedChoices_InOrder() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("3 1");

        var choices = new List<string> { "Mercury", "Venus", "Earth" };

        var result = Console.MultiSelection(choices, $"Plants: ");

        await Assert.That(result.Length).IsEqualTo(2);
        await Assert.That(result[0]).IsEqualTo("Earth");
        await Assert.That(result[1]).IsEqualTo("Mercury");
    }

    [Test]
    public async Task MultiSelection_InvalidEntry_ReturnsEmptyArray() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("2 x");

        var choices = new List<string> { "Alpha", "Beta", "Gamma" };

        var result = Console.MultiSelection(choices, $"Letters: ");

        await Assert.That(result.Length).IsEqualTo(0);
    }

    [Test]
    public async Task MultiSelection_EmptyInput_ReturnsEmptyArray() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader(string.Empty);

        var choices = new List<string> { "Alpha", "Beta" };

        var result = Console.MultiSelection(choices, $"Letters: ");

        await Assert.That(result.Length).IsEqualTo(0);
    }

    [Test]
    public async Task TreeMenu_ValidSelection_ReturnsTuple() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("2 1");

        var menu = new Dictionary<string, IList<string>> {
            ["Files"] = new List<string> { "Open", "Save" },
            ["Edit"] = new List<string> { "Undo", "Redo" }
        };

        var (option, subOption) = Console.TreeMenu(menu, $"Menu: ");

        await Assert.That(option).IsEqualTo("Edit");
        await Assert.That(subOption).IsEqualTo("Undo");
    }

    [Test]
    public async Task TreeMenu_MissingSelectionParts_ThrowsArgumentException() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("1");

        var menu = new Dictionary<string, IList<string>> {
            ["Files"] = new List<string> { "Open" }
        };

        await Assert.That(() => Console.TreeMenu(menu, $"Menu: "))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task TreeMenu_InvalidIndexes_ThrowsArgumentException() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("3 1");

        var menu = new Dictionary<string, IList<string>> {
            ["Files"] = new List<string> { "Open" },
            ["Edit"] = new List<string> { "Undo" }
        };

        await Assert.That(() => Console.TreeMenu(menu, $"Menu: "))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task Table_WritesHeaderAndRows() {
        Out = Utilities.GetWriter(out var writer);

        var headers = new List<string> { "Name", "Age" };
        var column1 = new List<string> { "Alice", "Bob" };
        var column2 = new List<string> { "30", "25" };

        Console.Table(headers, [column1, column2]);

        var output = writer.ToString();
        await Assert.That(output).Contains("Name");
        await Assert.That(output).Contains("Age");
        await Assert.That(output).Contains("Alice");
        await Assert.That(output).Contains("Bob");
    }

    [Test]
    public async Task Table_DifferentHeaderAndColumnCounts_ThrowsArgumentException() {
        Out = Utilities.GetWriter(out _);

        var headers = new List<string> { "Name", "Age" };
        var column1 = new List<string> { "Alice", "Bob" };

        await Assert.That(() => Console.Table(headers, [column1]))
            .Throws<ArgumentException>();
    }
}
