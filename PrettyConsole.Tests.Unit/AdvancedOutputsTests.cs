namespace PrettyConsole.Tests.Unit;

public class AdvancedOutputsTests {
    [Fact]
    public void Overwrite_ExecutesActionAndWritesOutput() {
        Error = Utilities.GetWriter(out var writer);
        bool executed = false;
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            Console.Overwrite(() => {
                executed = true;
                Console.WriteInterpolated(OutputPipe.Error, $"Progress");
            }, lines: 1, pipe: OutputPipe.Error);
        } finally {
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }

        Assert.True(executed);
        Assert.Contains("Progress", writer.ToString());
    }

    [Fact]
    public void Overwrite_WithState_ExecutesActionAndWritesOutput() {
        Error = Utilities.GetWriter(out var writer);
        bool executed = false;
        int cursorLine = 0;
        RenderingExtensions.ConfigureCursorAccessors(() => cursorLine, (_, line) => cursorLine = line);
        try {
            Console.Overwrite("Done", status => {
                executed = true;
                Console.WriteInterpolated(OutputPipe.Error, $"{status}");
            }, lines: 1, pipe: OutputPipe.Error);
        } finally {
            RenderingExtensions.ConfigureCursorAccessors(null, null);
        }

        Assert.True(executed);
        Assert.Contains("Done", writer.ToString());
    }

    [Fact]
    public async Task TypeWrite_Regular() {
        Out = Utilities.GetWriter(out var stringWriter);
        await Console.TypeWrite("Hello world!", Green / Black, 10);
        Assert.Contains("Hello world!", stringWriter.ToString());
    }

    [Fact]
    public async Task TypeWriteLine_Regular() {
        Out = Utilities.GetWriter(out var stringWriter);
        await Console.TypeWriteLine("Hello world!", Green / ConsoleColor.Default, 10);
        Assert.Contains("Hello world!" + Environment.NewLine, stringWriter.ToString());
    }
}
