namespace PrettyConsole;

internal static class AnsiColors {
    private const string ForegroundResetSequence = "\e[39m";
    private const string BackgroundResetSequence = "\e[49m";

    private static readonly string[] ForegroundCodes = null!;
    private static readonly string[] BackgroundCodes = null!;

    /// <summary>
    /// Gets a value indicating whether ANSI color sequences are emitted.
    /// </summary>
    public static readonly bool Enabled;

    static AnsiColors() {
        Enabled = !Console.IsOutputRedirected && !Console.IsErrorRedirected;
        if (!Enabled) return;

        ForegroundCodes = new string[16];
        BackgroundCodes = new string[16];
        foreach (var color in Enum.GetValues<ConsoleColor>()) {
            int index = (int)color;
            ForegroundCodes[index] = BuildForegroundSequence(color);
            BackgroundCodes[index] = BuildBackgroundSequence(color);
        }
    }

    /// <summary>
    /// Gets the ANSI sequence for the specified foreground color or an empty string when disabled.
    /// </summary>
    public static string Foreground(ConsoleColor color) {
        int index = (int)color;
        if (index == -1) return ForegroundResetSequence;
        return ForegroundCodes[index];
    }


    /// <summary>
    /// Gets the ANSI sequence for the specified background color or an empty string when disabled.
    /// </summary>
    public static string Background(ConsoleColor color) {
        int index = (int)color;
        if (index == -1) return BackgroundResetSequence;
        return BackgroundCodes[index];
    }


    private static string BuildForegroundSequence(ConsoleColor color) {
        return color switch {
            ConsoleColor.Black => "\e[30m",
            ConsoleColor.DarkBlue => "\e[34m",
            ConsoleColor.DarkGreen => "\e[32m",
            ConsoleColor.DarkCyan => "\e[36m",
            ConsoleColor.DarkRed => "\e[31m",
            ConsoleColor.DarkMagenta => "\e[35m",
            ConsoleColor.DarkYellow => "\e[33m",
            ConsoleColor.Gray => "\e[37m",
            ConsoleColor.DarkGray => "\e[90m",
            ConsoleColor.Blue => "\e[94m",
            ConsoleColor.Green => "\e[92m",
            ConsoleColor.Cyan => "\e[96m",
            ConsoleColor.Red => "\e[91m",
            ConsoleColor.Magenta => "\e[95m",
            ConsoleColor.Yellow => "\e[93m",
            ConsoleColor.White => "\e[97m",
            _ => ForegroundResetSequence
        };
    }

    private static string BuildBackgroundSequence(ConsoleColor color) {
        return color switch {
            ConsoleColor.Black => "\e[40m",
            ConsoleColor.DarkBlue => "\e[44m",
            ConsoleColor.DarkGreen => "\e[42m",
            ConsoleColor.DarkCyan => "\e[46m",
            ConsoleColor.DarkRed => "\e[41m",
            ConsoleColor.DarkMagenta => "\e[45m",
            ConsoleColor.DarkYellow => "\e[43m",
            ConsoleColor.Gray => "\e[47m",
            ConsoleColor.DarkGray => "\e[100m",
            ConsoleColor.Blue => "\e[104m",
            ConsoleColor.Green => "\e[102m",
            ConsoleColor.Cyan => "\e[106m",
            ConsoleColor.Red => "\e[101m",
            ConsoleColor.Magenta => "\e[105m",
            ConsoleColor.Yellow => "\e[103m",
            ConsoleColor.White => "\e[107m",
            _ => BackgroundResetSequence
        };
    }
}