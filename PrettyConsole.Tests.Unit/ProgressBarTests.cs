namespace PrettyConsole.Tests.Unit;

public class ProgressBarTests {
    [Fact]
    public void ProgressBar_Update_WritesStatusAndPercentage() {
        Utilities.SkipIfNoInteractiveConsole();
        Error = Utilities.GetWriter(out var errorWriter);

        var bar = new ProgressBar {
            ProgressChar = '#',
            ForegroundColor = White,
            ProgressColor = Green
        };

        bar.Update(50, "Loading");

        var output = errorWriter.ToString();
        Assert.Contains("Loading", output);
        Assert.Contains("#", output);
        Assert.Contains("50", output);
    }

    [Fact]
    public void ProgressBar_Update_SamePercentage_RerendersOutput() {
        Utilities.SkipIfNoInteractiveConsole();
        Error = Utilities.GetWriter(out var errorWriter);

        var bar = new ProgressBar {
            ProgressChar = '#',
            ForegroundColor = White,
            ProgressColor = Green
        };

        bar.Update(25, "Loading");
        errorWriter.ToStringAndFlush();

        bar.Update(25, "Loading");

        var output = errorWriter.ToString();
        Assert.NotEqual(string.Empty, output);
        Assert.Contains("Loading", output);
        Assert.Contains("25", output);
    }

    [Fact]
    public void ProgressBar_Update_SameLineFalse_WritesStatusOnSeparateLine() {
        Utilities.SkipIfNoInteractiveConsole();

        var originalError = Error;
        try {
            Error = Utilities.GetWriter(out var errorWriter);

            var bar = new ProgressBar {
                ProgressChar = '#'
            };

            bar.Update(75, "Working", sameLine: false);

            var output = errorWriter.ToString();
            Assert.Contains("Working", output);
            Assert.Contains(Environment.NewLine + "[", output);
        } finally {
            Error = originalError;
        }
    }

    [Fact]
    public void ProgressBar_WriteProgressBar_WritesFormattedOutput() {
        Utilities.SkipIfNoInteractiveConsole();

        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var outWriter);

            ProgressBar.WriteProgressBar(OutputPipe.Out, 75, Cyan, '*');

            var output = outWriter.ToString();
            Assert.Contains("[", output);
            Assert.Contains("75%", output);
            Assert.Contains("*", output);
        } finally {
            Out = originalOut;
        }
    }

    [Fact]
    public void ProgressBar_WriteProgressBar_RespectsMaxLineWidth() {
        Utilities.SkipIfNoInteractiveConsole();

        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var outWriter);

            ProgressBar.WriteProgressBar(OutputPipe.Out, 50, Cyan, '*', maxLineWidth: 24);

            var output = outWriter.ToString();
            Assert.Equal(24, output.Length);
            Assert.Equal('[', output[0]);
            Assert.Equal('%', output[^1]);
        } finally {
            Out = originalOut;
        }
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
