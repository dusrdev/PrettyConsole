using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class SelectionTest : IPrettyConsoleTest {
    public string FeatureName => "Selection";

    public ValueTask Implementation() {
        string[] options = [
            "Option 1",
            "Option 2",
            "Option 3"
        ];

        var selected = Selection(["Select an option"], options);
        WriteLine($"Selected: {selected}");
        return ValueTask.CompletedTask;
    }
}