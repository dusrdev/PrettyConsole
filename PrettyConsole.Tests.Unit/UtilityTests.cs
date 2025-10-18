namespace PrettyConsole.Tests.Unit;

public class UtilityTests {
    [Fact]
    public void WriteWhiteSpaces_WritesRequestedLength() {
        var writer = new StringWriter();

        writer.WriteWhiteSpaces(10);
        Assert.Equal(new string(' ', 10), writer.ToString());

        writer.GetStringBuilder().Clear();
        writer.WriteWhiteSpaces(300);
        Assert.Equal(300, writer.ToString().Length);
        Assert.True(writer.ToString().All(c => c == ' '));
    }

    [Fact]
    public void InColor_SetsForegroundWithDefaultBackground() {
        var colored = "hello".InColor(ConsoleColor.Cyan);

        Assert.Equal("hello", colored.Value);
        Assert.Equal(ConsoleColor.Cyan, colored.ForegroundColor);
        Assert.Equal(Color.DefaultBackgroundColor, colored.BackgroundColor);
    }

    [Fact]
    public void InColor_WithBackground_SetsBothColors() {
        var colored = "world".InColor(ConsoleColor.Green, ConsoleColor.Black);

        Assert.Equal("world", colored.Value);
        Assert.Equal(ConsoleColor.Green, colored.ForegroundColor);
        Assert.Equal(ConsoleColor.Black, colored.BackgroundColor);
    }
}
