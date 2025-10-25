using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

/// <summary>
/// Configures sameLine = false
/// </summary>
public sealed class ProgressBarMultiLineTest : IPrettyConsoleTest {
    public string FeatureName => "ProgressBarMultiLine";

    public async ValueTask Implementation() {
        var prg = new ProgressBar {
            ProgressColor = Color.Magenta,
        };
        const int count = 333;
        for (int i = 1; i <= count; i++) {
            double percentage = 100 * (double)i / count;
            prg.Update(percentage, "TESTING", false);
            await Task.Delay(15);
        }
        ClearNextLines(2, OutputPipe.Error);
        WriteLine(OutputPipe.Error, $"Done");
    }
}