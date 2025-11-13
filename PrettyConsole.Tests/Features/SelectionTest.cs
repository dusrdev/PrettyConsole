namespace PrettyConsole.Tests.Features;

public sealed class SelectionTest : IPrettyConsoleTest {
    public string FeatureName => "Selection";

    public ValueTask Implementation() {
        string[] options = [
            "Option 1",
            "Option 2",
            "Option 3"
        ];

        var selected = Console.Selection(options, $"Select an option: ");
        Console.WriteLineInterpolated($"Selected: {selected}");
        return ValueTask.CompletedTask;
    }
}