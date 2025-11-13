namespace PrettyConsole.Tests.Unit;

public class WriteLineExtensionsTests {
    private readonly StringWriter _writer;
    private readonly StringWriter _errorWriter;

    public WriteLineExtensionsTests() {
        Out = Utilities.GetWriter(out _writer);
        Error = Utilities.GetWriter(out _errorWriter);
    }

    [Fact]
    public void WriteLine_Interpolated_AppendsNewLine() {
        var originalOut = Out;
        var writer = new StringWriter();
        Out = writer;

        try {
            Console.WriteLineInterpolated(OutputPipe.Out, $"Line {7}");
            Assert.Equal($"Line 7{writer.NewLine}", writer.ToString());
        } finally {
            Out = originalOut;
        }
    }
}