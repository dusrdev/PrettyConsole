namespace PrettyConsole.Tests.Unit;

public class ProgressBarTests {
    [Fact]
    public void ProgressBar_Update_WritesStatusAndPercentage() {
        Utilities.SkipIfNoInteractiveConsole();
        Error = Utilities.GetWriter(out var errorWriter);

        var bar = new ProgressBar {
            ProgressChar = '#',
            ForegroundColor = ConsoleColor.White,
            ProgressColor = ConsoleColor.Green
        };

        bar.Update(50, "Loading");

        var output = errorWriter.ToString();
        Assert.Contains("Loading", output);
        Assert.Contains("#", output);
        Assert.Contains("50", output);
    }

    [Fact]
    public void ProgressBar_Update_SamePercentage_NoAdditionalOutput() {
        Utilities.SkipIfNoInteractiveConsole();
        Error = Utilities.GetWriter(out var errorWriter);

        var bar = new ProgressBar();

        bar.Update(25);
        errorWriter.ToStringAndFlush();

        bar.Update(25);

        Assert.Equal(string.Empty, errorWriter.ToString());
    }

    [Fact]
    public async Task IndeterminateProgressBar_RunAsync_CompletesAndReturnsResult() {
        Utilities.SkipIfNoInteractiveConsole();
        Error = Utilities.GetWriter(out var errorWriter);

        var bar = new IndeterminateProgressBar {
            AnimationSequence = new(["|", "/"]),
            DisplayElapsedTime = false,
            UpdateRate = 5
        };

        var cancellation = TestContext.Current.CancellationToken;
        var result = await bar.RunAsync(Task.Run(async () => {
            await Task.Delay(20, cancellation);
            return 42;
        }, cancellation), "Working", cancellation);

        Assert.Equal(42, result);
        Assert.NotEqual(string.Empty, errorWriter.ToString());
    }
}