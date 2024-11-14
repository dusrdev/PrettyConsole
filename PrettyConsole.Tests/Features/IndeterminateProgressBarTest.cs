using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class IndeterminateProgressBarTest : IPrettyConsoleTest {
    public string FeatureName => "IndeterminateProgressBar";

    public async ValueTask Implementation() {
        var prg = new IndeterminateProgressBar {
            ForegroundColor = Color.Red,
            DisplayElapsedTime = true
        };
        await prg.RunAsync(Task.Delay(1_000), "running...");
    }
}