using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class TreeMenuTest : IPrettyConsoleTest {
    public string FeatureName => "Tree Menu";

    public void Implementation() {
        Dictionary<string, List<string>> options = new() {
            { "Option 1", ["Option 1.1", "Option 1.2", "Option 1.3"] },
            { "Option 2", ["Option 2.1", "Option 2.2", "Option 2.3"] },
            { "Option 3", ["Option 3.1", "Option 3.2", "Option 3.3"] }
        };

        var (main, sub) = TreeMenu(["Select an option"], options);
        WriteLine($"Selected: ({main}, {sub})");
    }
}