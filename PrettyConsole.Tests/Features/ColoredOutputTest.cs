using static PrettyConsole.Console;

namespace PrettyConsole.Tests.Features;

public sealed class ColoredOutputTest : IPrettyConsoleTest {
    public string FeatureName => "ColoredOutput";

    public ValueTask Implementation() {
        WriteLine(["foreground = Red, background = White\t", "Test" * Color.Red / Color.White]);
        WriteLine(["foreground = Green, background = Black\t", "Test" * Color.Green / Color.Black]);
        WriteLine(["foreground = Blue, background = Yellow\t", "Test" * Color.Blue / Color.Yellow]);
        WriteLine(["foreground = Yellow, background = Blue\t", "Test" * Color.Yellow / Color.Blue]);
        WriteLine(["foreground = White, background = Red\t", "Test" * Color.White / Color.Red]);
        WriteLine(["foreground = Black, background = Green\t", "Test" * Color.Black / Color.Green]);
        WriteLine(["foreground = Cyan, background = Magenta\t", "Test" * Color.Cyan / Color.Magenta]);
        WriteLine(["foreground = Magenta, background = Cyan\t", "Test" * Color.Magenta / Color.Cyan]);
        WriteLine(["foreground = Gray, background = DarkGray\t", "Test" * Color.Gray / Color.DarkGray]);
        WriteLine(["foreground = DarkGray, background = Gray\t", "Test" * Color.DarkGray / Color.Gray]);
        WriteLine(["foreground = DarkRed, background = DarkGreen\t", "Test" * Color.DarkRed / Color.DarkGreen]);
        WriteLine(["foreground = DarkGreen, background = DarkRed\t", "Test" * Color.DarkGreen / Color.DarkRed]);
        WriteLine(["foreground = DarkBlue, background = DarkYellow\t", "Test" * Color.DarkBlue / Color.DarkYellow]);
        WriteLine(["foreground = DarkYellow, background = DarkBlue\t", "Test" * Color.DarkYellow / Color.DarkBlue]);
        WriteLine(["foreground = DarkMagenta, background = DarkCyan\t", "Test" * Color.DarkMagenta / Color.DarkCyan]);
        WriteLine(["foreground = DarkCyan, background = DarkMagenta\t", "Test" * Color.DarkCyan / Color.DarkMagenta]);
        WriteLine(["foreground = Black, background = White\t", "Test" * Color.Black / Color.White]);
        WriteLine(["foreground = White, background = Black\t", "Test" * Color.White / Color.Black]);
        WriteLine(["foreground = Red, background = Green\t", "Test" * Color.Red / Color.Green]);
        WriteLine(["foreground = Green, background = Red\t", "Test" * Color.Green / Color.Red]);
        WriteLine(["foreground = Blue, background = Yellow\t", "Test" * Color.Blue / Color.Yellow]);
        WriteLine(["foreground = Yellow, background = Blue\t", "Test" * Color.Yellow / Color.Blue]);
        WriteLine(["foreground = White, background = Red\t", "Test" * Color.White / Color.Red]);
        WriteLine(["foreground = Black, background = Green\t", "Test" * Color.Black / Color.Green]);
        WriteLine(["foreground = Cyan, background = Magenta\t", "Test" * Color.Cyan / Color.Magenta]);
        WriteLine(["foreground = Magenta, background = Cyan\t", "Test" * Color.Magenta / Color.Cyan]);
        return ValueTask.CompletedTask;
    }
}