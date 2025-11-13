namespace PrettyConsole.Tests.Unit;

public class ConsoleColorTests {
    [Fact]
    public void ConsoleColor_DivideOperator() {
        var (fg, bg) = Red / Blue;
        Assert.Equal(Red, fg);
        Assert.Equal(Blue, bg);
    }

    [Fact]
    public void ConsoleColor_DivideTupleOperator() {
        var (fg, bg) = Red / (Blue / Green);
        Assert.Equal(Red, fg);
        Assert.Equal(Green, bg);
    }

    [Fact]
    public void ConsoleColor_DefaultColors() {
        var (fg, bg) = ConsoleColor.Default;
        Assert.Equal(ConsoleColor.DefaultForeground, fg);
        Assert.Equal(ConsoleColor.DefaultBackground, bg);
    }
}