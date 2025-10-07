namespace PrettyConsole.Tests.Unit;

public class ColorTests {
    [Fact]
    public void Color_StaticFields_CoverAllConsoleColor() {
        HashSet<ConsoleColor> colors = [
            Color.Black,
            Color.Gray,
            Color.DarkGray,
            Color.White,
            Color.Red,
            Color.Blue,
            Color.Green,
            Color.Yellow,
            Color.Cyan,
            Color.Magenta,
            Color.DarkRed,
            Color.DarkBlue,
            Color.DarkGreen,
            Color.DarkYellow,
            Color.DarkCyan,
            Color.DarkMagenta,
        ];

        var stock = Enum.GetValues<ConsoleColor>().ToHashSet();
        stock.SymmetricExceptWith(colors);
        Assert.Empty(stock); // If stock isn't empty, Color doesn't cover all values
    }

    [Fact]
    public void Color_StaticField_EqualsConsoleColor() {
        (ConsoleColor, Color)[] colors = [
            (ConsoleColor.Black, Color.Black),
            (ConsoleColor.Gray, Color.Gray),
            (ConsoleColor.DarkGray, Color.DarkGray),
            (ConsoleColor.White, Color.White),
            (ConsoleColor.Red, Color.Red),
            (ConsoleColor.Blue, Color.Blue),
            (ConsoleColor.Green, Color.Green),
            (ConsoleColor.Yellow, Color.Yellow),
            (ConsoleColor.Cyan, Color.Cyan),
            (ConsoleColor.Magenta, Color.Magenta),
            (ConsoleColor.DarkRed, Color.DarkRed),
            (ConsoleColor.DarkBlue, Color.DarkBlue),
            (ConsoleColor.DarkGreen, Color.DarkGreen),
            (ConsoleColor.DarkYellow, Color.DarkYellow),
            (ConsoleColor.DarkCyan, Color.DarkCyan),
            (ConsoleColor.DarkMagenta, Color.DarkMagenta),
        ];

        foreach (var (consoleColor, color) in colors) {
            Assert.Equal(consoleColor, color);
        }
    }

    [Fact]
    public void Color_AsteriskOperator() {
        var coloredOutput = "Hello" * Color.Green;
        Assert.Equal(ConsoleColor.Green, coloredOutput.ForegroundColor);
        Assert.Equal(System.Console.BackgroundColor, coloredOutput.BackgroundColor);
    }

    [Fact]
    public void Color_DivideOperator() {
        var coloredOutput = "Hello" / Color.Red;
        Assert.Equal(System.Console.ForegroundColor, coloredOutput.ForegroundColor);
        Assert.Equal(ConsoleColor.Red, coloredOutput.BackgroundColor);
    }

    [Fact]
    public void Color_ObjectOperator() {
        var coloredOutput = 3 * Color.Green;
        Assert.Equal("3", coloredOutput.Value);
    }

    [Fact]
    public void ColoredOutput_ForegroundCtor() {
        var coloredOutput = new ColoredOutput("Hello", Color.Red);
        Assert.Equal(ConsoleColor.Red, coloredOutput.ForegroundColor);
        Assert.Equal(System.Console.BackgroundColor, coloredOutput.BackgroundColor);
    }

    [Fact]
    public void ColoredOutput_StringOperator() {
        ColoredOutput coloredOutput = "Hello";
        Assert.Equal("Hello", coloredOutput.Value);
        Assert.Equal(System.Console.ForegroundColor, coloredOutput.ForegroundColor);
        Assert.Equal(System.Console.BackgroundColor, coloredOutput.BackgroundColor);
    }

    [Fact]
    public void ColoredOutput_ReadOnlySpanOperator() {
        ColoredOutput coloredOutput = "Hello".AsSpan();
        Assert.Equal("Hello", coloredOutput.Value);
        Assert.Equal(System.Console.ForegroundColor, coloredOutput.ForegroundColor);
        Assert.Equal(System.Console.BackgroundColor, coloredOutput.BackgroundColor);
    }

    [Fact]
    public void ColoredOutput_DivideOperator() {
        var coloredOutput = "Hello" * Color.Red / Color.Blue;
        Assert.Equal(ConsoleColor.Red, coloredOutput.ForegroundColor);
        Assert.Equal(ConsoleColor.Blue, coloredOutput.BackgroundColor);
    }
}
