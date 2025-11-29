namespace PrettyConsole.UnitTests;

using System.Globalization;

public class WriteLineExtensionsTests {
    private readonly StringWriter _writer;
    private readonly StringWriter _errorWriter;

    public WriteLineExtensionsTests() {
        Out = Utilities.GetWriter(out _writer);
        Error = Utilities.GetWriter(out _errorWriter);
    }

    [Test]
    public async Task WriteLine_Interpolated_AppendsNewLine() {
        var originalOut = Out;
        var writer = new StringWriter();
        Out = writer;

        try {
            Console.WriteLineInterpolated(OutputPipe.Out, $"Line {7}");
            await Assert.That(writer.ToString()).IsEqualTo($"Line 7{writer.NewLine}");
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task WriteLine_ReadOnlySpan_WithColors_AppendsNewLine() {
        Console.WriteLine("SpanLine".AsSpan(), OutputPipe.Out, ConsoleColor.Yellow, ConsoleColor.Black);
        await Assert.That(_writer.ToStringAndFlush()).IsEqualTo($"SpanLine{_writer.NewLine}");
    }

    [Test]
    public async Task WriteLine_ISpanFormattable_Overloads() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);

            Console.WriteLine<double>(3.14, OutputPipe.Out);
            Console.WriteLine(2.71, OutputPipe.Out, ConsoleColor.Yellow);
            Console.WriteLine(1.23, OutputPipe.Out, ConsoleColor.Yellow, ConsoleColor.Black);
            Console.WriteLine(9.99, OutputPipe.Out, ConsoleColor.White, ConsoleColor.Black, "F1", CultureInfo.InvariantCulture);

            var expected = $"3.14{writer.NewLine}2.71{writer.NewLine}1.23{writer.NewLine}10.0{writer.NewLine}";
            await Assert.That(writer.ToString()).IsEqualTo(expected);
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task WriteLine_ReadOnlySpan_DefaultAndForeground() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);

            Console.WriteLine("span".AsSpan(), OutputPipe.Out);
            Console.WriteLine("more".AsSpan(), OutputPipe.Out, ConsoleColor.Cyan);

            var expected = $"span{writer.NewLine}more{writer.NewLine}";
            await Assert.That(writer.ToString()).IsEqualTo(expected);
        } finally {
            Out = originalOut;
        }
    }
}