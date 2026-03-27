namespace PrettyConsole.UnitTests;

public class AdvancedOutputsTests {
    [Test]
    public async Task Overwrite_ExecutesActionAndWritesOutput() {
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

        await Assert.That(executed).IsTrue();
        await Assert.That(writer.ToString()).Contains("Progress");
    }

    [Test]
    public async Task Overwrite_WithState_ExecutesActionAndWritesOutput() {
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

        await Assert.That(executed).IsTrue();
        await Assert.That(writer.ToString()).Contains("Done");
    }

    [Test]
    public async Task TypeWrite_Regular() {
        Out = Utilities.GetWriter(out var stringWriter);
        await Console.TypeWrite("Hello world!", (Color.Green, Color.BlackBackground), 10);
        await Assert.That(stringWriter.ToString()).Contains("Hello world!");
    }

    [Test]
    public async Task TypeWriteLine_Regular() {
        Out = Utilities.GetWriter(out var stringWriter);
        await Console.TypeWriteLine("Hello world!", (Color.Green, Color.DefaultBackground), 10);
        await Assert.That(stringWriter.ToString()).Contains("Hello world!" + Environment.NewLine);
    }
}
