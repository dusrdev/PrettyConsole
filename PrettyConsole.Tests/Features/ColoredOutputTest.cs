using static System.ConsoleColor;

namespace PrettyConsole.Tests.Features;

public sealed class ColoredOutputTest : IPrettyConsoleTest {
    public string FeatureName => "ColoredOutput";

    public ValueTask Implementation() {
        Console.WriteLineInterpolated($"foreground = Red, background = White\t{Red / White}Test");
        Console.WriteLineInterpolated($"foreground = Green, background = Black\t{Green / Black}Test");
        Console.WriteLineInterpolated($"foreground = Yellow, background = Blue\t{Yellow / Blue}Test");
        Console.WriteLineInterpolated($"foreground = Cyan, background = Magenta\t{Cyan / Magenta}Test");
        Console.WriteLineInterpolated($"foreground = Magenta, background = Cyan\t{Magenta / Cyan}Test");
        Console.WriteLineInterpolated($"foreground = Gray, background = DarkGray\t{Gray / DarkGray}Test");
        Console.WriteLineInterpolated($"foreground = DarkGray, background = Gray\t{DarkGray / Gray}Test");
        Console.WriteLineInterpolated($"foreground = DarkRed, background = DarkGreen\t{DarkRed / DarkGreen}Test");
        Console.WriteLineInterpolated($"foreground = DarkGreen, background = DarkRed\t{DarkGreen / DarkRed}Test");
        Console.WriteLineInterpolated($"foreground = DarkBlue, background = DarkYellow\t{DarkBlue / DarkYellow}Test");
        Console.WriteLineInterpolated($"foreground = DarkYellow, background = DarkBlue\t{DarkYellow / DarkBlue}Test");
        Console.WriteLineInterpolated($"foreground = DarkMagenta, background = DarkCyan\t{DarkMagenta / DarkCyan}Test");
        Console.WriteLineInterpolated($"foreground = DarkCyan, background = DarkMagenta\t{DarkCyan / DarkMagenta}Test");
        Console.WriteLineInterpolated($"foreground = Black, background = White\t{Black / White}Test");
        Console.WriteLineInterpolated($"foreground = White, background = Black\t{White / Black}Test");
        Console.WriteLineInterpolated($"foreground = Red, background = Green\t{Red / Green}Test");
        Console.WriteLineInterpolated($"foreground = Green, background = Red\t{Green / Red}Test");
        Console.WriteLineInterpolated($"foreground = Blue, background = Yellow\t{Blue / Yellow}Test");
        Console.WriteLineInterpolated($"foreground = Yellow, background = Blue\t{Yellow / Blue}Test");
        Console.WriteLineInterpolated($"foreground = White, background = Red\t{White / Red}Test");
        Console.WriteLineInterpolated($"foreground = Black, background = Green\t{Black / Green}Test");
        return ValueTask.CompletedTask;
    }
}