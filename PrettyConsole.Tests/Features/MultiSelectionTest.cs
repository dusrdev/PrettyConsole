using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class MultiSelectionTest : IPrettyConsoleTest {
    public string FeatureName => "MultiSelection";

    public ValueTask Implementation() {
        string[] options = [
            "Option 1",
            "Option 2",
            "Option 3"
        ];

        var selected = MultiSelection(["Select an option:"], options);
        WriteLine($"Selected: [{string.Join(", ", selected)}]");
        return ValueTask.CompletedTask;
    }
}