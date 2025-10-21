namespace PrettyConsole.Tests.Unit;

public class AdvancedOutputs {
    [Fact]
    public void OverwriteCurrentLine_WritesOutputToPipe() {
        Utilities.SkipIfNoInteractiveConsole();
        Error = Utilities.GetWriter(out var writer);

        OverwriteCurrentLine(["Updating" * Color.Green], OutputPipe.Error);

        Assert.Contains("Updating", writer.ToString());
    }

    [Fact]
    public void Overwrite_ExecutesActionAndWritesOutput() {
        Utilities.SkipIfNoInteractiveConsole();
        Error = Utilities.GetWriter(out var writer);
        bool executed = false;

        Overwrite(() => {
            executed = true;
            Write(OutputPipe.Error, $"Progress");
        }, lines: 1, pipe: OutputPipe.Error);

        Assert.True(executed);
        Assert.Contains("Progress", writer.ToString());
    }

    [Fact]
    public void Overwrite_WithState_ExecutesActionAndWritesOutput() {
        Utilities.SkipIfNoInteractiveConsole();
        Error = Utilities.GetWriter(out var writer);
        bool executed = false;

        Overwrite("Done", status => {
            executed = true;
            Write(OutputPipe.Error, $"{status}");
        }, lines: 1, pipe: OutputPipe.Error);

        Assert.True(executed);
        Assert.Contains("Done", writer.ToString());
    }

    [Fact]
    public async Task TypeWrite_Regular() {
        Out = Utilities.GetWriter(out var stringWriter);
        await TypeWrite("Hello world!" * Color.Green, 10);
        Assert.Contains("Hello world!", stringWriter.ToString());
    }

    [Fact]
    public async Task TypeWriteLine_Regular() {
        Out = Utilities.GetWriter(out var stringWriter);
        await TypeWriteLine("Hello world!" * Color.Green, 10);
        Assert.Contains("Hello world!" + Environment.NewLine, stringWriter.ToString());
    }
}