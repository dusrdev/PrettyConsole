using System.Globalization;

namespace PrettyConsole.Tests.Unit;

public class WriteExtensionsTests {
    private readonly StringWriter _writer;
    private readonly StringWriter _errorWriter;

    public WriteExtensionsTests() {
        Out = Utilities.GetWriter(out _writer);
        Error = Utilities.GetWriter(out _errorWriter);
    }

    [Fact]
    public void Write_Interpolated_WritesFormattedContent_ToOutPipe() {
        var originalOut = Out;
        var writer = new StringWriter();
        Out = writer;

        try {
            Console.WriteInterpolated(OutputPipe.Out, $"Hello {42}");
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
            Console.WriteInterpolated(OutputPipe.Error, $"Error {123}");
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
            Console.WriteInterpolated(OutputPipe.Out,
                $"Colors {Black / Green}Green{ConsoleColor.Default} {Red}Red");

            var normalized = Utilities.StripAnsiSequences(writer.ToString());

            Assert.Equal("Colors Green Red", normalized);
        } finally {
            Out = originalOut;
        }
    }

    [Fact]
    public void Write_SpanFormattable_NoColors() {
        Console.Write<double>(3.14);
        Assert.Equal("3.14", _writer.ToStringAndFlush());
    }

    [Fact]
    public void Write_SpanFormattable_ForegroundColor() {
        Console.Write(3.14, OutputPipe.Out, White);
        Assert.Equal("3.14", _writer.ToStringAndFlush());
    }

    [Fact]
    public void Write_SpanFormattable_ForegroundAndBackgroundColor() {
        Console.Write(3.14, OutputPipe.Out, White, Black);
        Assert.Equal("3.14", _writer.ToStringAndFlush());
    }

    [Fact]
    public void Write_SpanFormattable_VeryLongObjectFormat() {
        var obj = new LongFormatStud();
        Console.Write<LongFormatStud>(obj);
        Assert.Equal(new string('X', LongFormatStud.Length), _writer.ToStringAndFlush());
    }

    [Fact]
    public void Write_SpanFormattable_WithFormatAndProvider() {
        Console.Write(12.345, OutputPipe.Out, White, Black, "F2", CultureInfo.InvariantCulture);
        Assert.Equal("12.35", _writer.ToStringAndFlush());
    }

    [Fact]
    public void Write_ReadOnlySpan_WithColors_WritesToSelectedPipe() {
        Console.Write("Data".AsSpan(), OutputPipe.Error, ConsoleColor.Green, ConsoleColor.Black);
        Assert.Equal("Data", _errorWriter.ToStringAndFlush());
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
