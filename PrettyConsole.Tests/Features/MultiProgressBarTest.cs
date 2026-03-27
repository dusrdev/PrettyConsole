namespace PrettyConsole.Tests.Features;

public sealed class MultiProgressBarTest : IPrettyConsoleTest {
    public string FeatureName => "MultiProgressBar";

    public async ValueTask Implementation() {
        const int count = 333;
        var currentLine = Console.GetCurrentLine();
        for (int i = 1; i <= count; i++) {
            double percentage = 100 * (double)i / count;

            Console.Overwrite((int)percentage, p => {
                Console.WriteInterpolated(OutputPipe.Error, $"Task {1}: ");
                ProgressBar.Render(OutputPipe.Error, p, Color.Magenta);
                Console.WriteInterpolated(OutputPipe.Error, $"Task {2}: ");
                ProgressBar.Render(OutputPipe.Error, p, Color.Magenta);
            }, 2);

            await Task.Delay(15);
        }
        Console.ClearNextLines(2, OutputPipe.Error);
        Console.WriteLineInterpolated(OutputPipe.Error, $"Done");
    }
}