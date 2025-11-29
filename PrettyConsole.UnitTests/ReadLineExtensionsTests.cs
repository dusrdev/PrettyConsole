namespace PrettyConsole.UnitTests;

public class ReadLineExtensionsTests {
    [Test]
    public async Task ReadLine_InterpolatedPrompt_WritesPromptAndReadsValue() {
        Out = Utilities.GetWriter(out _);
        var reader = Utilities.GetReader("123");
        In = reader;

        var result = Console.ReadLine($"Enter number: ");

        await Assert.That(result).IsEqualTo("123");
    }

    [Test]
    public async Task TryReadLine_InterpolatedWithDefault_ReturnsDefaultOnFailure() {
        Out = Utilities.GetWriter(out _);
        var reader = Utilities.GetReader("not-a-number");
        In = reader;

        var parsed = Console.TryReadLine(out int result, 42, $"Enter number: ");

        await Assert.That(parsed).IsFalse();
        await Assert.That(result).IsEqualTo(42);
    }

    [Test]
    public async Task TryReadLine_Enum_InterpolatedPrompt_IgnoreCase() {
        Out = Utilities.GetWriter(out _);
        var reader = Utilities.GetReader("yElLoW");
        In = reader;

        var parsed = Console.TryReadLine(out ConsoleColor color, true, $"Enter enum: ");

        await Assert.That(parsed).IsTrue();
        await Assert.That(color).IsEqualTo(Yellow);
    }

    [Test]
    public async Task TryReadLine_Generic_ParsesValue() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("42");

        var parsed = Console.TryReadLine(out int value, $"Number: ");

        await Assert.That(parsed).IsTrue();
        await Assert.That(value).IsEqualTo(42);
    }

    [Test]
    public async Task TryReadLine_Enum_WithDefault_ReturnsDefaultWhenInvalid() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("not-a-color");

        var parsed = Console.TryReadLine(out ConsoleColor color, ignoreCase: true, ConsoleColor.Blue, $"Enum: ");

        await Assert.That(parsed).IsFalse();
        await Assert.That(color).IsEqualTo(ConsoleColor.Blue);
    }

    [Test]
    public async Task ReadLine_Generic_ReturnsParsedValue() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("3.14");

        var value = Console.ReadLine<double>($"Value: ");

        await Assert.That(value).IsEqualTo(3.14);
    }

    [Test]
    public async Task ReadLine_GenericWithDefault_ReturnsDefaultWhenInvalid() {
        Out = Utilities.GetWriter(out _);
        In = Utilities.GetReader("not-number");

        var value = Console.ReadLine(5, $"Value: ");

        await Assert.That(value).IsEqualTo(5);
    }
}