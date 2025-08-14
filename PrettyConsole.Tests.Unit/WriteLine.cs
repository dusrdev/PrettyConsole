namespace PrettyConsole.Tests.Unit;

public class WriteLine {
    private readonly StringWriter _writer;
    private readonly StringWriter _errorWriter;

    public WriteLine() {
        Out = Utilities.GetWriter(out _writer);
        Error = Utilities.GetWriter(out _errorWriter);
    }

    [Fact]
    public void WriteLine_ColoredOutput_Single() {
        WriteLine("Hello world!" * Color.Green);
        Assert.Equal("Hello world!".WithNewLine(), _writer.ToStringAndFlush());
    }

    [Fact]
    public void WriteLine_ColoredOutput_Multiple() {
        WriteLine(["Hello " * Color.Green, "David" * Color.Yellow, "!"]);
        Assert.Equal("Hello David!".WithNewLine(), _writer.ToStringAndFlush());
    }

    [Fact]
    public void WriteLineError_ColoredOutput_Single() {
        WriteLine("Hello world!" * Color.Green, OutputPipe.Error);
        Assert.Equal("Hello world!".WithNewLine(), _errorWriter.ToStringAndFlush());
    }

    [Fact]
    public void WriteLineError_ColoredOutput_Multiple() {
        WriteLine(["Hello " * Color.Green, "David" * Color.Yellow, "!"], OutputPipe.Error);
        Assert.Equal("Hello David!".WithNewLine(), _errorWriter.ToStringAndFlush());
    }
}