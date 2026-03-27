namespace PrettyConsole.UnitTests;

[SkipWhenConsoleUnavailable]
public class LiveConsoleRegionTests {
    [Test]
    public async Task Render_WritesSnapshotAndMarksRegionActive() {
        var originalError = Error;
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            Error = Utilities.GetWriter(out var errorWriter);
            using var region = new LiveConsoleRegion();

            region.Render($"Working");

            await Assert.That(Utilities.StripAnsiSequences(errorWriter.ToString())).Contains("Working");
            await Assert.That(region.IsActive).IsTrue();
            await Assert.That(region.OccupiedLines).IsEqualTo(1);
        } finally {
            Error = originalError;
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task WriteLine_WhileActive_WritesDurableOutputAndRestoresRegion() {
        var originalError = Error;
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            Error = Utilities.GetWriter(out var errorWriter);
            using var region = new LiveConsoleRegion();

            region.Render($"Loading");
            errorWriter.ToStringAndFlush();

            region.WriteLine($"Updated package-a");

            var output = Utilities.StripAnsiSequences(errorWriter.ToString());
            await Assert.That(output).Contains("Updated package-a");
            await Assert.That(CountOccurrences(output, "Loading")).IsEqualTo(1);
            await Assert.That(region.IsActive).IsTrue();
        } finally {
            Error = originalError;
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task Render_SameVisibleSnapshot_DoesNotRewriteOutput() {
        var originalError = Error;
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            Error = Utilities.GetWriter(out var errorWriter);
            using var region = new LiveConsoleRegion();

            region.Render($"Loading");
            errorWriter.ToStringAndFlush();

            region.Render($"Loading");

            await Assert.That(errorWriter.ToString()).IsEqualTo(string.Empty);
        } finally {
            Error = originalError;
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task RenderProgress_WithFactory_SameLine_WritesHeaderAndBar() {
        var originalError = Error;
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            Error = Utilities.GetWriter(out var errorWriter);
            using var region = new LiveConsoleRegion();

            region.RenderProgress(40, (builder, out handler) => handler = builder.Build(OutputPipe.Error, $"hdr"), sameLine: true, progressColor: Color.Cyan);

            var output = Utilities.StripAnsiSequences(errorWriter.ToString());
            await Assert.That(output).Contains("hdr");
            await Assert.That(output).Contains("[");
            await Assert.That(output).Contains("40%");
        } finally {
            Error = originalError;
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task RenderProgress_WithFactory_TwoLines_WritesHeaderAboveBar() {
        var originalError = Error;
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            Error = Utilities.GetWriter(out var errorWriter);
            using var region = new LiveConsoleRegion();

            region.RenderProgress(55, (builder, out handler) => handler = builder.Build(OutputPipe.Error, $"status"), sameLine: false, progressColor: Color.Cyan);

            var output = Utilities.StripAnsiSequences(errorWriter.ToString());
            await Assert.That(output).Contains("status");
            await Assert.That(output).Contains(Environment.NewLine);
            await Assert.That(output).Contains("55%");
            await Assert.That(region.OccupiedLines).IsEqualTo(2);
        } finally {
            Error = originalError;
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task Render_UsesCurrentWriter_WhenPipeTargetChangesAfterCreation() {
        var originalError = Error;
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            using var region = new LiveConsoleRegion();

            Error = Utilities.GetWriter(out var firstWriter);
            region.Render($"First");
            firstWriter.ToStringAndFlush();

            Error = Utilities.GetWriter(out var secondWriter);
            region.WriteLine($"Next");

            var output = Utilities.StripAnsiSequences(secondWriter.ToString());
            await Assert.That(output).Contains("Next");
            await Assert.That(output).Contains("First");
        } finally {
            Error = originalError;
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task Render_WithLoneLineFeeds_CountsOccupiedLinesCorrectly() {
        var originalError = Error;
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            Error = Utilities.GetWriter(out var errorWriter);
            using var region = new LiveConsoleRegion();

            region.Render($"Line1{Environment.NewLine}Line2{Environment.NewLine}Line3");

            await Assert.That(region.OccupiedLines).IsEqualTo(3);
            await Assert.That(Utilities.StripAnsiSequences(errorWriter.ToString())).Contains("Line1");
        } finally {
            Error = originalError;
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task Clear_RemovesSnapshotAndMarksRegionInactive() {
        var originalError = Error;
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            Error = Utilities.GetWriter(out var errorWriter);
            using var region = new LiveConsoleRegion();

            region.Render($"Loading");
            errorWriter.ToStringAndFlush();

            region.Clear();

            await Assert.That(region.IsActive).IsFalse();
            await Assert.That(region.OccupiedLines).IsEqualTo(0);
            await Assert.That(errorWriter.ToString()).IsNotEqualTo(string.Empty);
        } finally {
            Error = originalError;
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task DisposedRegion_RejectsFurtherWrites() {
        var originalError = Error;
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            Error = Utilities.GetWriter(out _);
            var region = new LiveConsoleRegion();
            region.Dispose();

            await Assert.That(() => region.Render($"Loading")).Throws<ObjectDisposedException>();
        } finally {
            Error = originalError;
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    private static int CountOccurrences(string value, string needle) {
        int count = 0;
        int index = 0;
        while ((index = value.IndexOf(needle, index, StringComparison.Ordinal)) >= 0) {
            count++;
            index += needle.Length;
        }
        return count;
    }
}
