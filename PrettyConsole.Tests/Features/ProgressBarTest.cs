using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class ProgressBarTest : IPrettyConsoleTest {
    public string FeatureName => "ProgressBar";

    public async ValueTask Implementation() {
        var prg = new ProgressBar {
            ProgressColor = Color.Magenta,
            // ProgressChar = '🧎‍♂️‍➡️'
        };
        const int count = 333;
        var currentLine = GetCurrentLine();
        for (int i = 1; i <= count; i++) {
            double percentage = 100 * (double)i / count;
            prg.Update(percentage, "TESTING");
            await Task.Delay(15);
        }
        ClearNextLines(1, OutputPipe.Error);
        GoToLine(currentLine);
    }
}