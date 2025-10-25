using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

/// <summary>
/// Uses the default update method parameter (same line)
/// </summary>
public sealed class ProgressBarDefaultTest : IPrettyConsoleTest {
    public string FeatureName => "ProgressBarDefault";

    public async ValueTask Implementation() {
        var prg = new ProgressBar {
            ProgressColor = Color.Magenta,
        };
        const int count = 333;
        for (int i = 1; i <= count; i++) {
            double percentage = 100 * (double)i / count;
            prg.Update(percentage, "TESTING");
            await Task.Delay(15);
        }
        ClearNextLines(1, OutputPipe.Error);
        WriteLine(OutputPipe.Error, $"Done");
    }
}