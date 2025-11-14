namespace PrettyConsole.Tests.Unit;

public class ReadLineExtensionsTests {
    [Fact]
    public void ReadLine_InterpolatedPrompt_WritesPromptAndReadsValue() {
        Out = Utilities.GetWriter(out _);
        var reader = Utilities.GetReader("123");
        In = reader;

        var result = Console.ReadLine($"Enter number: ");

        Assert.Equal("123", result);
    }

    [Fact]
    public void TryReadLine_InterpolatedWithDefault_ReturnsDefaultOnFailure() {
        Out = Utilities.GetWriter(out _);
        var reader = Utilities.GetReader("not-a-number");
        In = reader;

        var parsed = Console.TryReadLine(out int result, 42, $"Enter number: ");

        Assert.False(parsed);
        Assert.Equal(42, result);
    }

    [Fact]
    public void TryReadLine_Enum_InterpolatedPrompt_IgnoreCase() {
        Out = Utilities.GetWriter(out _);
        var reader = Utilities.GetReader("yElLoW");
        In = reader;

        var parsed = Console.TryReadLine(out ConsoleColor color, true, $"Enter enum: ");

        Assert.True(parsed);
        Assert.Equal(Yellow, color);
    }

    [Fact]
    public void TryReadLine_Generic_ParsesValue() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("42");

        var parsed = Console.TryReadLine(out int value, $"Number: ");

        Assert.True(parsed);
        Assert.Equal(42, value);
    }

    [Fact]
    public void TryReadLine_Enum_WithDefault_ReturnsDefaultWhenInvalid() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("not-a-color");

        var parsed = Console.TryReadLine(out ConsoleColor color, ignoreCase: true, ConsoleColor.Blue, $"Enum: ");

        Assert.False(parsed);
        Assert.Equal(ConsoleColor.Blue, color);
    }

    [Fact]
    public void ReadLine_Generic_ReturnsParsedValue() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("3.14");

        var value = Console.ReadLine<double>($"Value: ");

        Assert.Equal(3.14, value);
    }

    [Fact]
    public void ReadLine_GenericWithDefault_ReturnsDefaultWhenInvalid() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("not-number");

        var value = Console.ReadLine(5, $"Value: ");

        Assert.Equal(5, value);
    }
}
