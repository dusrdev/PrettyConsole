using TUnit.Core.Attributes;

namespace PrettyConsole.UnitTests;

[SkipWhenConsoleUnavailable]
public class ProgressBarTests {
    [Test]
    public async Task ProgressBar_Update_WritesStatusAndPercentage() {
        Error = Utilities.GetWriter(out var errorWriter);
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            var bar = new ProgressBar {
                ProgressChar = '#',
                ForegroundColor = White,
                ProgressColor = Green
            };

            bar.Update(50, "Loading");
        } finally {
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }

        var output = errorWriter.ToString();
        await Assert.That(output).Contains("Loading");
        await Assert.That(output).Contains("#");
        await Assert.That(output).Contains("50");
    }

    [Test]
    public async Task ProgressBar_Update_SamePercentage_RerendersOutput() {
        Error = Utilities.GetWriter(out var errorWriter);
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            var bar = new ProgressBar {
                ProgressChar = '#',
                ForegroundColor = White,
                ProgressColor = Green
            };

            bar.Update(25, "Loading");
            errorWriter.ToStringAndFlush();

            bar.Update(25, "Loading");
        } finally {
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }

        var output = errorWriter.ToString();
        await Assert.That(output).IsNotEqualTo(string.Empty);
        await Assert.That(output).Contains("Loading");
        await Assert.That(output).Contains("25");
    }

    [Test]
    public async Task ProgressBar_Update_SameLineFalse_WritesStatusOnSeparateLine() {
        var originalError = Error;
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            Error = Utilities.GetWriter(out var errorWriter);

            var bar = new ProgressBar {
                ProgressChar = '#'
            };

            bar.Update(75, "Working", sameLine: false);

            var output = errorWriter.ToString();
            await Assert.That(output).Contains("Working");
            await Assert.That(output).Contains(Environment.NewLine + "[");
        } finally {
            Error = originalError;
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task ProgressBar_WriteProgressBar_WritesFormattedOutput() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var outWriter);

            ProgressBar.WriteProgressBar(OutputPipe.Out, 75, Cyan, '*');

            var output = outWriter.ToString();
            await Assert.That(output).Contains("[");
            await Assert.That(output).Contains("75%");
            await Assert.That(output).Contains("*");
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task ProgressBar_WriteProgressBar_RespectsMaxLineWidth() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var outWriter);

            ProgressBar.WriteProgressBar(OutputPipe.Out, 50, Cyan, '*', maxLineWidth: 24);

            var output = outWriter.ToString();
            await Assert.That(output.Length).IsEqualTo(24);
            await Assert.That(output[0]).IsEqualTo('[');
            await Assert.That(output[^1]).IsEqualTo('%');
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task ProgressBar_Update_RespectsMaxLineWidth() {
        Error = Utilities.GetWriter(out var errorWriter);
        int cursorLine = 0;
        const int expectedWidth = 32;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            var bar = new ProgressBar {
                ProgressColor = Cyan,
                MaxLineWidth = expectedWidth
            };

            bar.Update(50, "Working");
        } finally {
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }

        var output = Utilities.StripAnsiSequences(errorWriter.ToString());
        int percentIndex = output.LastIndexOf('%');
        await Assert.That(percentIndex > 0).IsTrue();
        int bracketIndex = output.LastIndexOf('[', percentIndex);
        await Assert.That(bracketIndex >= 0).IsTrue();

        var segment = output[bracketIndex..(percentIndex + 1)];
        await Assert.That(segment.Length).IsEqualTo(expectedWidth);
    }

    [Test]
    public async Task ProgressBar_Update_Overloads_WriteOutput() {
        var originalError = Error;
        Error = Utilities.GetWriter(out var errorWriter);
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            var bar = new ProgressBar { ProgressColor = Cyan };

            bar.Update(10);
            bar.Update(20.0, "status");
        } finally {
            Error = originalError;
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }

        await Assert.That(errorWriter.ToString()).Contains("status");
    }

    [Test]
    public async Task ProgressBar_WriteProgressBar_DoubleOverload_WritesOutput() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);

            ProgressBar.WriteProgressBar(OutputPipe.Out, 33.3, Blue, '*');

            await Assert.That(writer.ToString()).Contains("33%");
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task ProgressBar_Update_DoubleOverload_WritesPercentage() {
        Error = Utilities.GetWriter(out var errorWriter);
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            var bar = new ProgressBar { ProgressColor = Green };
            bar.Update(12.5);
        } finally {
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }

        await Assert.That(errorWriter.ToString()).IsNotEqualTo(string.Empty);
    }

    [Test]
    public async Task ProgressBar_Update_StatusSpan_SameLineFalse() {
        Error = Utilities.GetWriter(out var errorWriter);
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            var bar = new ProgressBar { ProgressColor = Green };
            ReadOnlySpan<char> status = "span-status".AsSpan();

            bar.Update(30, status, sameLine: false);
        } finally {
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }

        await Assert.That(errorWriter.ToString()).Contains("span-status");
    }

    [Test]
    public async Task IndeterminateProgressBar_RunAsync_CompletesAndReturnsResult() {
        Error = Utilities.GetWriter(out var errorWriter);
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            var bar = new IndeterminateProgressBar {
                AnimationSequence = new(["|", "/"]),
                DisplayElapsedTime = false,
                UpdateRate = 5
            };

            var cancellation = CancellationToken.None;
            int result = await bar.RunAsync(Task.Run(async () => {
                await Task.Delay(20, cancellation);
                return 42;
            }, cancellation), _ => $"Working", cancellation);

            await Assert.That(result).IsEqualTo(42);
            await Assert.That(errorWriter.ToString()).IsNotEqualTo(string.Empty);
        } finally {
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task IndeterminateProgressBar_RunAsync_OverloadsAndForegroundSetter() {
        var originalError = Error;
        Error = Utilities.GetWriter(out var errorWriter);
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            var bar = new IndeterminateProgressBar {
                DisplayElapsedTime = false,
                UpdateRate = 5
            };
            bar.ForegroundColor = Cyan;

            var genericResult = await bar.RunAsync(Task.Run(async () => { await Task.Delay(10); return 7; }));
            await bar.RunAsync(Task.Run(async () => await Task.Delay(10)));

            await Assert.That(genericResult).IsEqualTo(7);
            await Assert.That(errorWriter.ToString()).IsNotEqualTo(string.Empty);
        } finally {
            Error = originalError;
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task IndeterminateProgressBar_RunAsync_Generic_TaskAlreadyCompleted() {
        Error = Utilities.GetWriter(out var errorWriter);
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            var bar = new IndeterminateProgressBar {
                DisplayElapsedTime = false,
                UpdateRate = 5
            };

            var completed = Task.FromResult(5);
            var result = await bar.RunAsync(completed, _ => $"done");

            await Assert.That(result).IsEqualTo(5);
        } finally {
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task IndeterminateProgressBar_RunAsync_CancelsQuickly() {
        Error = Utilities.GetWriter(out var errorWriter);
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        using var cts = new CancellationTokenSource();
        try {
            var bar = new IndeterminateProgressBar {
                DisplayElapsedTime = false,
                UpdateRate = 100
            };

            var task = Task.Run(async () => {
                await Task.Delay(10000, cts.Token);
            }, cts.Token);

            cts.CancelAfter(200);
            await bar.RunAsync(task, _ => $"cancelled", cts.Token);
        } catch (OperationCanceledException) {
            // expected in this path
        } finally {
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }

        await Assert.That(errorWriter.ToString()).Contains("cancelled");
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
internal sealed class SkipWhenConsoleUnavailableAttribute : SkipAttribute {
    public SkipWhenConsoleUnavailableAttribute() : base("Console handle unavailable for this environment.") {
    }

    public override Task<bool> ShouldSkip(TestRegisteredContext testContext) => Task.FromResult(!ConsoleAvailability.IsAvailable());
}

internal static class ConsoleAvailability {
    public static bool IsAvailable() {
        try {
            _ = Console.BufferWidth;
            _ = Console.CursorLeft;
            return true;
        } catch (IOException) {
            return false;
        }
    }
}
