using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class IndeterminateProgressBarTest : IPrettyConsoleTest {
    public string FeatureName => "IndeterminateProgressBar";

    public async ValueTask Implementation() {
        var prg = new IndeterminateProgressBar {
            AnimationSequence = IndeterminateProgressBar.Patterns.Braille,
            ForegroundColor = Color.Magenta,
            // UpdateRate = 120,
            DisplayElapsedTime = true
        };
        await prg.RunAsync(Task.Delay(5_000), "running...");
    }
}