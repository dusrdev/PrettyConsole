namespace PrettyConsole.Tests.Unit;

public class Write {
    private readonly StringWriter _writer;
    private readonly StringWriter _errorWriter;

    public Write() {
        Out = Utilities.GetWriter(out _writer);
        Error = Utilities.GetWriter(out _errorWriter);
    }

    [Fact]
    public void Write_SpanFormattable_NoColors() {
        Write(3.14);
        _writer.ToStringAndFlush().Should().Be("3.14");
    }

    [Fact]
    public void Write_SpanFormattable_ForegroundColor() {
        Write(3.14, OutputPipe.Out, Color.White);
        _writer.ToStringAndFlush().Should().Be("3.14");
    }

    [Fact]
    public void Write_SpanFormattable_ForegroundAndBackgroundColor() {
        Write(3.14, OutputPipe.Out, Color.White, Color.Black);
        _writer.ToStringAndFlush().Should().Be("3.14");
    }

    [Fact]
    public void Write_ColoredOutput_Single() {
        Write("Hello world!" * Color.Green);
        _writer.ToStringAndFlush().Should().Be("Hello world!");
    }

    [Fact]
    public void Write_ColoredOutput_Multiple() {
        Write(["Hello " * Color.Green, "David" * Color.Yellow, "!"]);
        _writer.ToStringAndFlush().Should().Be("Hello David!");
    }

    [Fact]
    public void WriteError_ColoredOutput_Single() {
        Write("Hello world!" * Color.Yellow, OutputPipe.Error);
        _errorWriter.ToStringAndFlush().Should().Be("Hello world!");
    }

    [Fact]
    public void WriteError_ColoredOutput_Single2() {
        Write(["Hello world!" * Color.Green], OutputPipe.Error);
        _errorWriter.ToStringAndFlush().Should().Be("Hello world!");
    }

    [Fact]
    public void WriteError_ColoredOutput_Multiple() {
        Write(["Hello " * Color.Green, "David" * Color.Yellow, "!"], OutputPipe.Error);
        _errorWriter.ToStringAndFlush().Should().Be("Hello David!");
    }
}