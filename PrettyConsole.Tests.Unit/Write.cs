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
        Assert.Equal("3.14", _writer.ToStringAndFlush());
    }

    [Fact]
    public void Write_SpanFormattable_ForegroundColor() {
        Write(3.14, OutputPipe.Out, Color.White);
        Assert.Equal("3.14", _writer.ToStringAndFlush());
    }

    [Fact]
    public void Write_SpanFormattable_ForegroundAndBackgroundColor() {
        Write(3.14, OutputPipe.Out, Color.White, Color.Black);
        Assert.Equal("3.14", _writer.ToStringAndFlush());
    }

    [Fact]
    public void Write_ColoredOutput_Single() {
        Write("Hello world!" * Color.Green);
        Assert.Equal("Hello world!", _writer.ToStringAndFlush());
    }

    [Fact]
    public void Write_ColoredOutput_Multiple() {
        Write(["Hello " * Color.Green, "David" * Color.Yellow, "!"]);
        Assert.Equal("Hello David!", _writer.ToStringAndFlush());
    }

    [Fact]
    public void WriteError_ColoredOutput_Single() {
        Write("Hello world!" * Color.Yellow, OutputPipe.Error);
        Assert.Equal("Hello world!", _errorWriter.ToStringAndFlush());
    }

    [Fact]
    public void WriteError_ColoredOutput_Single2() {
        Write(["Hello world!" * Color.Green], OutputPipe.Error);
        Assert.Equal("Hello world!", _errorWriter.ToStringAndFlush());
    }

    [Fact]
    public void WriteError_ColoredOutput_Multiple() {
        Write(["Hello " * Color.Green, "David" * Color.Yellow, "!"], OutputPipe.Error);
        Assert.Equal("Hello David!", _errorWriter.ToStringAndFlush());
    }
}