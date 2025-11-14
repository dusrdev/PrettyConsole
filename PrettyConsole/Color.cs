namespace PrettyConsole;

internal static class AnsiColors {
    private static readonly string[] ForegroundCodes = new string[16];
    private static readonly string[] BackgroundCodes = new string[16];

    /// <summary>
    /// Gets a value indicating whether ANSI color sequences are emitted.
    /// </summary>
    public static readonly bool Enabled;

    static AnsiColors() {
        Enabled = !Console.IsOutputRedirected && !Console.IsErrorRedirected;
        for (int i = 0; i < ForegroundCodes.Length; i++) {
            ForegroundCodes[i] = string.Empty;
            BackgroundCodes[i] = string.Empty;
        }

        if (!Enabled) {
            return;
        }

        foreach (var color in Enum.GetValues<ConsoleColor>()) {
            int index = (int)color;
            ForegroundCodes[index] = BuildForegroundSequence(color);
            BackgroundCodes[index] = BuildBackgroundSequence(color);
        }
    }

    /// <summary>
    /// Gets the ANSI sequence for the specified foreground color or an empty string when disabled.
    /// </summary>
    public static string Foreground(ConsoleColor color) => ForegroundCodes[(int)color];

    /// <summary>
    /// Gets the ANSI sequence for the specified background color or an empty string when disabled.
    /// </summary>
    public static string Background(ConsoleColor color) => BackgroundCodes[(int)color];

    private static string BuildForegroundSequence(ConsoleColor color) {
        return color switch {
            ConsoleColor.Black => "\u001b[30m",
            ConsoleColor.DarkBlue => "\u001b[34m",
            ConsoleColor.DarkGreen => "\u001b[32m",
            ConsoleColor.DarkCyan => "\u001b[36m",
            ConsoleColor.DarkRed => "\u001b[31m",
            ConsoleColor.DarkMagenta => "\u001b[35m",
            ConsoleColor.DarkYellow => "\u001b[33m",
            ConsoleColor.Gray => "\u001b[37m",
            ConsoleColor.DarkGray => "\u001b[90m",
            ConsoleColor.Blue => "\u001b[94m",
            ConsoleColor.Green => "\u001b[92m",
            ConsoleColor.Cyan => "\u001b[96m",
            ConsoleColor.Red => "\u001b[91m",
            ConsoleColor.Magenta => "\u001b[95m",
            ConsoleColor.Yellow => "\u001b[93m",
            ConsoleColor.White => "\u001b[97m",
            _ => "\u001b[39m"
        };
    }

    private static string BuildBackgroundSequence(ConsoleColor color) {
        return color switch {
            ConsoleColor.Black => "\u001b[40m",
            ConsoleColor.DarkBlue => "\u001b[44m",
            ConsoleColor.DarkGreen => "\u001b[42m",
            ConsoleColor.DarkCyan => "\u001b[46m",
            ConsoleColor.DarkRed => "\u001b[41m",
            ConsoleColor.DarkMagenta => "\u001b[45m",
            ConsoleColor.DarkYellow => "\u001b[43m",
            ConsoleColor.Gray => "\u001b[47m",
            ConsoleColor.DarkGray => "\u001b[100m",
            ConsoleColor.Blue => "\u001b[104m",
            ConsoleColor.Green => "\u001b[102m",
            ConsoleColor.Cyan => "\u001b[106m",
            ConsoleColor.Red => "\u001b[101m",
            ConsoleColor.Magenta => "\u001b[105m",
            ConsoleColor.Yellow => "\u001b[103m",
            ConsoleColor.White => "\u001b[107m",
            _ => "\u001b[49m"
        };
    }
}
