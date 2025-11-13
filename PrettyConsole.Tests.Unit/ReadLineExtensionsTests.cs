namespace PrettyConsole.Tests.Unit;

public class ReadLineExtensionsTests {
    [Fact]
    public void ReadLine_String_NoOutput() {
        Out = Utilities.GetWriter(out var _);
        var reader = Utilities.GetReader("Hello world!");
        In = reader;
        Assert.Equal("Hello world!", Console.ReadLine());
    }

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
}