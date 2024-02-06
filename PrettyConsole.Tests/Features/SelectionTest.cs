using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class SelectionTest : IPrettyConsoleTest {
    public string FeatureName => "Tree Menu";

    public void Implementation() {
        WriteLine(" Not implemented " & Color.Black | Color.Red);
    }
}