using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class TableTest : IPrettyConsoleTest {
    public string FeatureName => "Table";

    public void Implementation() {
        var attributes = Enum.GetNames<FileAttributes>();
        var lowered = attributes.Select(x => x.ToLower()).ToArray();

        Table(["attributes", "lowered"], [attributes, lowered]);
    }
}