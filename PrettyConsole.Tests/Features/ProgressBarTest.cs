using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class ProgressBarTest : IPrettyConsoleTest {
    public string FeatureName => "ProgressBar";

    public async ValueTask Implementation() {
        var prg = new ProgressBar {
            ProgressColor = Color.Yellow,
            ProgressChar = '»'
        };
        const int count = 1_000;
        var currentLine = GetCurrentLine();
        for (int i = 1; i <= count; i++) {
            double percentage = 100 * (double)i / count;
            prg.Update(percentage);
            await Task.Delay(1);
        }
        ClearNextLines(1, OutputPipe.Error);
        GoToLine(currentLine);
    }
}