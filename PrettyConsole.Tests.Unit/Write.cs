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
    public void Write_SpanFormattable_VeryLongObjectFormat() {
        var obj = new LongFormatStud();
        Write(obj);
        Assert.Equal(new string('X', LongFormatStud.Length), _writer.ToStringAndFlush());
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

    private readonly ref struct LongFormatStud : ISpanFormattable {
        public const int Length = 1024;

        public string ToString(string? format, IFormatProvider? formatProvider) {
            return new string('X', Length);
        }

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) {
            if (destination.Length < Length) {
                charsWritten = 0;
                return false;
            }
            var slice = destination.Slice(0, Length);
            slice.Fill('X');
            charsWritten = Length;
            return true;
        }
    }
}