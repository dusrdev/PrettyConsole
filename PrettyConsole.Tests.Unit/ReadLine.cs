namespace PrettyConsole.Tests.Unit;

public class ReadLine {
    [Fact]
    public void ReadLine_String_NoOutput() {
        Out = Utilities.GetWriter(out var _);
        var reader = Utilities.GetReader("Hello world!");
        In = reader;
        ReadLine().Should().Be("Hello world!");
    }

    [Fact]
    public void ReadLine_String_WithOutput() {
        Out = Utilities.GetWriter(out var _);
        var reader = Utilities.GetReader("Hello world!");
        In = reader;
        ReadLine(["Enter something:"]).Should().Be("Hello world!");
    }

    [Fact]
    public void ReadLine_Int() {
        Out = Utilities.GetWriter(out var _);
        var reader = Utilities.GetReader("5");
        In = reader;
        ReadLine<int>(["Enter num:"]).Should().Be(5);
    }

    [Fact]
    public void ReadLine_Int_InvalidWithDefault() {
        Out = Utilities.GetWriter(out var _);
        var reader = Utilities.GetReader("Hello");
        In = reader;
        ReadLine(["Enter num:"], 5).Should().Be(5);
    }

    [Fact]
    public void TryReadLine_Int_InvalidWithDefault() {
        Out = Utilities.GetWriter(out var _);
        var reader = Utilities.GetReader("Hello");
        In = reader;
        TryReadLine(["Enter num:"], 5, out int num).Should().BeFalse();
        num.Should().Be(5);
    }

    [Fact]
    public void TryReadLine_Enum_IgnoreCase() {
        Out = Utilities.GetWriter(out var _);
        var reader = Utilities.GetReader("bLack");
        In = reader;
        TryReadLine(["Enter color:"], true, out ConsoleColor color).Should().BeTrue();
        color.Should().Be(ConsoleColor.Black);
    }
}