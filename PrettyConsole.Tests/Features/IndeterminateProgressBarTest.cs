using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class IndeterminateProgressBarTest : IPrettyConsoleTest {
    public string FeatureName => "IndeterminateProgressBar";

    public void Implementation() {
        IndeterminateProgressBar(Task.Delay(5000), ConsoleColor.Green, true).Wait();
    }
}