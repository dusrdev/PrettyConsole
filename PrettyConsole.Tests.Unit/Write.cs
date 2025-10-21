namespace PrettyConsole.Tests.Unit;

public class Write {
    private readonly StringWriter _writer;
    private readonly StringWriter _errorWriter;

    public Write() {
        Out = Utilities.GetWriter(out _writer);
        Error = Utilities.GetWriter(out _errorWriter);
    }

    [Fact]
    public void Write_Interpolated_WritesFormattedContent_ToOutPipe() {
        var originalOut = Out;
        var writer = new StringWriter();
        Out = writer;

        try {
            Write(OutputPipe.Out, $"Hello {42}");
            Assert.Equal("Hello 42", writer.ToString());
        } finally {
            Out = originalOut;
        }
    }

    [Fact]
    public void Write_Interpolated_WritesFormattedContent_ToErrorPipe() {
        var originalError = Error;
        var writer = new StringWriter();
        Error = writer;

        try {
            Write(OutputPipe.Error, $"Error {123}");
            Assert.Equal("Error 123", writer.ToString());
        } finally {
            Error = originalError;
        }
    }

    [Fact]
    public void Write_Interpolated_IgnoresColorTokensInOutput() {
        var originalOut = Out;
        var writer = new StringWriter();
        Out = writer;

        try {
            Write(OutputPipe.Out,
                $"Colors {Color.Black / Color.Green}Green{Color.Default} {Color.Red}Red{Color.Default}");

            Assert.Equal("Colors Green Red", writer.ToString());
        } finally {
            Out = originalOut;
        }
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

    [Fact]
    public void Write_Interpolated_RightAlignmentPadsWithSpaces() {
        Write($"Value {42,5}");
        Assert.Equal("Value    42", _writer.ToStringAndFlush());
    }

    [Fact]
    public void Write_Interpolated_LeftAlignmentPadsWithSpaces() {
        Write($"Value {42,-5}");
        Assert.Equal("Value 42   ", _writer.ToStringAndFlush());
    }

    [Fact]
    public void Write_Interpolated_TimeSpanHumanReadableFormat() {
        Write($"Elapsed {TimeSpan.FromSeconds(75):hr}");
        Assert.Equal("Elapsed 01:15m", _writer.ToStringAndFlush());
    }

    [Fact]
    public void Write_Interpolated_ColoredOutputSpan_WritesValues() {
        ReadOnlySpan<ColoredOutput> outputs = [
            "Hi " * Color.Green,
            "There" * Color.Yellow
        ];

        Write($"Span {outputs}");

        Assert.Equal("Span Hi There", _writer.ToStringAndFlush());
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