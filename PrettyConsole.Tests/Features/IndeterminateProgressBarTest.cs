using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class IndeterminateProgressBarTest : IPrettyConsoleTest {
    public string FeatureName => "IndeterminateProgressBar";

    public void Implementation() {
        var prg = new IndeterminateProgressBar {
            ForegroundColor = Color.Red,
            DisplayElapsedTime = true
        };
        prg.RunAsync(Task.Delay(5000)).Wait();
    }
}