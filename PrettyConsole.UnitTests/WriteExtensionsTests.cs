using System.Globalization;

namespace PrettyConsole.UnitTests;

public class WriteExtensionsTests {
    private readonly StringWriter _writer;
    private readonly StringWriter _errorWriter;

    public WriteExtensionsTests() {
        Out = Utilities.GetWriter(out _writer);
        Error = Utilities.GetWriter(out _errorWriter);
    }

    [Test]
    public async Task Write_Interpolated_WritesFormattedContent_ToOutPipe() {
        var originalOut = Out;
        var writer = new StringWriter();
        Out = writer;

        try {
            Console.WriteInterpolated(OutputPipe.Out, $"Hello {42}");
            await Assert.That(writer.ToString()).IsEqualTo("Hello 42");
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task Write_Interpolated_WritesFormattedContent_ToErrorPipe() {
        var originalError = Error;
        var writer = new StringWriter();
        Error = writer;

        try {
            Console.WriteInterpolated(OutputPipe.Error, $"Error {123}");
            await Assert.That(writer.ToString()).IsEqualTo("Error 123");
        } finally {
            Error = originalError;
        }
    }

    [Test]
    public async Task Write_Interpolated_IgnoresColorTokensInOutput() {
        var originalOut = Out;
        var writer = new StringWriter();
        Out = writer;

        try {
            Console.WriteInterpolated(OutputPipe.Out,
                $"Colors {Black / Green}Green{ConsoleColor.Default} {Red}Red");

            var normalized = Utilities.StripAnsiSequences(writer.ToString());

            await Assert.That(normalized).IsEqualTo("Colors Green Red");
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task Write_SpanFormattable_NoColors() {
        Console.Write<double>(3.14);
        await Assert.That(_writer.ToStringAndFlush()).IsEqualTo("3.14");
    }

    [Test]
    public async Task Write_SpanFormattable_ForegroundColor() {
        Console.Write(3.14, OutputPipe.Out, White);
        await Assert.That(_writer.ToStringAndFlush()).IsEqualTo("3.14");
    }

    [Test]
    public async Task Write_SpanFormattable_ForegroundAndBackgroundColor() {
        Console.Write(3.14, OutputPipe.Out, White, Black);
        await Assert.That(_writer.ToStringAndFlush()).IsEqualTo("3.14");
    }

    [Test]
    public async Task Write_SpanFormattable_VeryLongObjectFormat() {
        var obj = new LongFormatStud();
        Console.Write<LongFormatStud>(obj);
        await Assert.That(_writer.ToStringAndFlush()).IsEqualTo(new string('X', LongFormatStud.Length));
    }

    [Test]
    public async Task Write_SpanFormattable_WithFormatAndProvider() {
        Console.Write(12.345, OutputPipe.Out, White, Black, "F2", CultureInfo.InvariantCulture);
        await Assert.That(_writer.ToStringAndFlush()).IsEqualTo("12.35");
    }

    [Test]
    public async Task Write_ReadOnlySpan_WithColors_WritesToSelectedPipe() {
        Console.Write("Data".AsSpan(), OutputPipe.Error, ConsoleColor.Green, ConsoleColor.Black);
        await Assert.That(_errorWriter.ToStringAndFlush()).IsEqualTo("Data");
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
