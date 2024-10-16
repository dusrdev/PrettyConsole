using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class TableTest : IPrettyConsoleTest {
    public string FeatureName => "Table";

    public ValueTask Implementation() {
        var attributes = Enum.GetNames<FileAttributes>();
        var lowered = attributes.Select(x => x.ToLower()).ToArray();

        Table(["attributes", "lowered"], [attributes, lowered]);
        return ValueTask.CompletedTask;
    }
}