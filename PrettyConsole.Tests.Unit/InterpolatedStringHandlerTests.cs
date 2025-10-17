namespace PrettyConsole.Tests.Unit;

public class InterpolatedStringHandlerTests {
    [Fact]
    public void WriteInterpolated_WritesFormattedContent_ToOutPipe() {
        var originalOut = Out;
        var writer = new StringWriter();
        Out = writer;

        try {
            WriteInterpolated(OutputPipe.Out, $"Hello {42}");
            Assert.Equal("Hello 42", writer.ToString());
        } finally {
            Out = originalOut;
        }
    }

    [Fact]
    public void WriteInterpolated_WritesFormattedContent_ToErrorPipe() {
        var originalError = Error;
        var writer = new StringWriter();
        Error = writer;

        try {
            WriteInterpolated(OutputPipe.Error, $"Error {123}");
            Assert.Equal("Error 123", writer.ToString());
        } finally {
            Error = originalError;
        }
    }

    [Fact]
    public void WriteLineInterpolated_AppendsNewLine() {
        var originalOut = Out;
        var writer = new StringWriter();
        Out = writer;

        try {
            WriteLineInterpolated(OutputPipe.Out, $"Line {7}");
            Assert.Equal($"Line 7{writer.NewLine}", writer.ToString());
        } finally {
            Out = originalOut;
        }
    }

    [Fact]
    public void WriteInterpolated_IgnoresColorTokensInOutput() {
        var originalOut = Out;
        var writer = new StringWriter();
        Out = writer;

        try {
            WriteInterpolated(OutputPipe.Out,
                $"Colors {Color.Black / Color.Green}Green{Color.Default} {Color.Red}Red{Color.Default}");

            Assert.Equal("Colors Green Red", writer.ToString());
        } finally {
            Out = originalOut;
        }
    }
}
