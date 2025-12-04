namespace PrettyConsole.UnitTests;

public class RenderingExtensionsTests {
    [Test]
    public async Task WriteWhiteSpaces_WritesExpectedCount() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);

            Console.WriteWhiteSpaces(5);

            await Assert.That(writer.ToString()).IsEqualTo(new string(' ', 5));
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task NewLine_WritesEnvironmentNewLine() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);

            Console.NewLine();

            await Assert.That(writer.ToString()).IsEqualTo(Environment.NewLine);
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    [SkipWhenConsoleColorsUnavailable]
    public async Task SetColors_UpdatesConsoleColors() {
        var originalForeground = Console.ForegroundColor;
        var originalBackground = Console.BackgroundColor;

        try {
            Console.SetColors(Cyan, DarkRed);

            await Assert.That(Console.ForegroundColor).IsEqualTo(Cyan);
            await Assert.That(Console.BackgroundColor).IsEqualTo(DarkRed);
        } finally {
            Console.ForegroundColor = originalForeground;
            Console.BackgroundColor = originalBackground;
        }
    }

    [Test]
    public async Task GetCurrentLine_UsesConfiguredAccessor() {
        const int expectedLine = 17;
        RenderingExtensions.ConfigureCursorAccessors(() => expectedLine, null);

        try {
            int currentLine = Console.GetCurrentLine();

            await Assert.That(currentLine).IsEqualTo(expectedLine);
        } finally {
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task GoToLine_InvokesConfiguredSetter() {
        List<(int Left, int Top)> positions = [];
        RenderingExtensions.ConfigureCursorAccessors(null, (left, top) => positions.Add((left, top)));

        try {
            Console.GoToLine(12);

            await Assert.That(positions.Count).IsEqualTo(1);
            await Assert.That(positions[0]).IsEqualTo((0, 12));
        } finally {
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task SkipLines_MovesCursorRelativeToCurrentLine() {
        const int startingLine = 4;
        List<(int Left, int Top)> positions = [];
        RenderingExtensions.ConfigureCursorAccessors(() => startingLine, (left, top) => positions.Add((left, top)));

        try {
            Console.SkipLines(3);

            await Assert.That(positions.Count).IsEqualTo(1);
            await Assert.That(positions[0]).IsEqualTo((0, startingLine + 3));
        } finally {
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }
    }

    [Test]
    public async Task ClearNextLines_WritesSpacesAndRestoresCursor() {
        var originalOut = Out;
        var originalError = Error;
        const int currentLine = 5;
        List<(int Left, int Top)> positions = [];

        try {
            Out = Utilities.GetWriter(out _); // force width fallback path
            Error = Utilities.GetWriter(out var errorWriter);
            RenderingExtensions.ConfigureCursorAccessors(() => currentLine, (left, top) => positions.Add((left, top)));

            int width = GetWidthOrDefault();

            Console.ClearNextLines(3, OutputPipe.Error);

            await Assert.That(errorWriter.ToString()).IsEqualTo(new string(' ', width * 3));
            await Assert.That(positions.Count).IsEqualTo(2);
            await Assert.That(positions[0]).IsEqualTo((0, currentLine));
            await Assert.That(positions[1]).IsEqualTo((0, currentLine));
        } finally {
            RenderingExtensions.ConfigureCursorAccessors(null, null);
            Out = originalOut;
            Error = originalError;
        }
    }
}