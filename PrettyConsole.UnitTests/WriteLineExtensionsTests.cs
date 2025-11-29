namespace PrettyConsole.UnitTests;

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
}
