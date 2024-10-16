using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class ProgressBarTest : IPrettyConsoleTest {
    public string FeatureName => "ProgressBar";

    public async ValueTask Implementation() {
        var prg = new ProgressBar {
            ProgressColor = Color.Yellow,
            ProgressChar = '#'
        };
        const int count = 10_000;
        for (int i = 1; i <= count; i++) {
            double percentage = 100 * (double)i / count;
            prg.Update(percentage, "Running...");
            await Task.Delay(1);
        }
    }
}