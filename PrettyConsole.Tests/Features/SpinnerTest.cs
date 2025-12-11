namespace PrettyConsole.Tests.Features;

public sealed class SpinnerTest : IPrettyConsoleTest {
    public string FeatureName => "Spinner";

    public async ValueTask Implementation() {
        var spinner = new Spinner {
            Pattern = Spinner.Patterns.Braille,
            ForegroundColor = ConsoleColor.Magenta,
            DisplayElapsedTime = true
        };
        await spinner.RunAsync(Task.Delay(5_000), (builder, out handler) => handler = builder.Build(OutputPipe.Error, $"...{ConsoleColor.Green}Running{ConsoleColor.DefaultForeground}..."));
    }
}
