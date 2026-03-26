namespace PrettyConsole.UnitTests;

[SkipWhenConsoleUnavailable]
public class TransientConsoleRegionTests {
    [Test]
    public async Task Render_WritesSnapshotAndMarksRegionActive() {
        var originalError = Error;
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            Error = Utilities.GetWriter(out var errorWriter);
            using var region = new TransientConsoleRegion();

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
            using var region = new TransientConsoleRegion();

            region.Render($"Loading");
            errorWriter.ToStringAndFlush();

            region.WriteLine($"Updated serde");

            var output = Utilities.StripAnsiSequences(errorWriter.ToString());
            await Assert.That(output).Contains("Updated serde");
            await Assert.That(CountOccurrences(output, "Loading")).IsEqualTo(1);
            await Assert.That(region.IsActive).IsTrue();
        } finally {
            Error = originalError;
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task Write_WhileActive_WritesDurableOutputWithoutRedrawingImmediately() {
        var originalError = Error;
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            Error = Utilities.GetWriter(out var errorWriter);
            using var region = new TransientConsoleRegion();

            region.Render($"Loading");
            errorWriter.ToStringAndFlush();

            region.Write($"step ");

            var output = Utilities.StripAnsiSequences(errorWriter.ToString());
            await Assert.That(output).Contains("step ");
            await Assert.That(output).DoesNotContain("Loading");
            await Assert.That(region.IsActive).IsTrue();
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
            using var region = new TransientConsoleRegion();

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
            var region = new TransientConsoleRegion();
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
