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
        _writer.ToStringAndFlush().Should().Be("Hello world!".WithNewLine());
    }

    [Fact]
    public void WriteLine_ColoredOutput_Multiple() {
        WriteLine(["Hello " * Color.Green, "David" * Color.Yellow, "!"]);
        _writer.ToStringAndFlush().Should().Be("Hello David!".WithNewLine());
    }

    [Fact]
    public void WriteLineError_ColoredOutput_Single() {
        WriteLineError("Hello world!" * Color.Green);
        _errorWriter.ToStringAndFlush().Should().Be("Hello world!".WithNewLine());
    }

    [Fact]
    public void WriteLineError_ColoredOutput_Multiple() {
        WriteLineError(["Hello " * Color.Green, "David" * Color.Yellow, "!"]);
        _errorWriter.ToStringAndFlush().Should().Be("Hello David!".WithNewLine());
    }
}