namespace PrettyConsole.Tests.Unit;

public class ReadLine {
    [Fact]
    public void ReadLine_String_NoOutput() {
        Out = Utilities.GetWriter(out var _);
        var reader = Utilities.GetReader("Hello world!");
        In = reader;
        Assert.Equal("Hello world!", ReadLine());
    }

    [Fact]
    public void ReadLine_String_WithOutput() {
        Out = Utilities.GetWriter(out var _);
        var reader = Utilities.GetReader("Hello world!");
        In = reader;
        Assert.Equal("Hello world!", ReadLine(["Enter something:"]));
    }

    [Fact]
    public void ReadLine_Int() {
        Out = Utilities.GetWriter(out var _);
        var reader = Utilities.GetReader("5");
        In = reader;
        Assert.Equal(5, ReadLine<int>(["Enter num:"]));
    }

    [Fact]
    public void ReadLine_Int_InvalidWithDefault() {
        Out = Utilities.GetWriter(out var _);
        var reader = Utilities.GetReader("Hello");
        In = reader;
        Assert.Equal(5, ReadLine(["Enter num:"], 5));
    }

    [Fact]
    public void TryReadLine_Int_InvalidWithDefault() {
        Out = Utilities.GetWriter(out var _);
        var reader = Utilities.GetReader("Hello");
        In = reader;
        Assert.False(TryReadLine(["Enter num:"], 5, out int num));
        Assert.Equal(5, num);
    }

    [Fact]
    public void TryReadLine_Enum_IgnoreCase() {
        Out = Utilities.GetWriter(out var _);
        var reader = Utilities.GetReader("bLack");
        In = reader;
        Assert.True(TryReadLine(["Enter color:"], true, out ConsoleColor color));
        Assert.Equal(ConsoleColor.Black, color);
    }

    [Fact]
    public void ReadLine_InterpolatedPrompt_WritesPromptAndReadsValue() {
        Out = Utilities.GetWriter(out _);
        var reader = Utilities.GetReader("123");
        In = reader;

        var result = ReadLine($"Enter number: ");

        Assert.Equal("123", result);
    }

    [Fact]
    public void TryReadLine_InterpolatedWithDefault_ReturnsDefaultOnFailure() {
        Out = Utilities.GetWriter(out _);
        var reader = Utilities.GetReader("not-a-number");
        In = reader;

        var parsed = TryReadLine(out int result, 42, $"Enter number: ");

        Assert.False(parsed);
        Assert.Equal(42, result);
    }

    [Fact]
    public void TryReadLine_Enum_InterpolatedPrompt_IgnoreCase() {
        Out = Utilities.GetWriter(out _);
        var reader = Utilities.GetReader("yElLoW");
        In = reader;

        var parsed = TryReadLine(out ConsoleColor color, true, $"Enter enum: ");

        Assert.True(parsed);
        Assert.Equal(ConsoleColor.Yellow, color);
    }
}