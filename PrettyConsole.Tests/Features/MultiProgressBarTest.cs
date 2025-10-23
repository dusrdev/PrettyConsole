using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class MultiProgressBarTest : IPrettyConsoleTest {
    public string FeatureName => "MultiProgressBar";

    public async ValueTask Implementation() {
        const int count = 333;
        var currentLine = GetCurrentLine();
        for (int i = 1; i <= count; i++) {
            double percentage = 100 * (double)i / count;

            Overwrite((int)percentage, p => {
                Write(OutputPipe.Error, $"Task {1}: ");
                ProgressBar.WriteBar(OutputPipe.Error, p, Color.Magenta);
                NewLine();
                Write(OutputPipe.Error, $"Task {2}: ");
                ProgressBar.WriteBar(OutputPipe.Error, p, Color.Magenta);
                NewLine();
            }, 2);

            // prg.Update(percentage, "TESTING");
            await Task.Delay(15);
        }
        ClearNextLines(2, OutputPipe.Error);
        // ClearNextLines(1, OutputPipe.Error);
        // GoToLine(currentLine);
    }
}