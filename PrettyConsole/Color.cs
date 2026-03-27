namespace PrettyConsole;

/// <summary>
/// Provides guarded ANSI color tokens for interpolation-friendly color usage.
/// </summary>
public static class Color {
    /// <summary>
    /// Resets both foreground and background colors to the terminal defaults.
    /// </summary>
    public static readonly AnsiToken Default = new("\e[39m\e[49m");

    /// <summary>
    /// Resets the foreground color to the terminal default.
    /// </summary>
    public static readonly AnsiToken DefaultForeground = new("\e[39m");

    /// <summary>
    /// Resets the background color to the terminal default.
    /// </summary>
    public static readonly AnsiToken DefaultBackground = new("\e[49m");

    /// <summary>Foreground token for <see cref="ConsoleColor.Black"/>.</summary>
    public static readonly AnsiToken Black = new("\e[30m");

    /// <summary>Foreground token for <see cref="ConsoleColor.DarkBlue"/>.</summary>
    public static readonly AnsiToken DarkBlue = new("\e[34m");

    /// <summary>Foreground token for <see cref="ConsoleColor.DarkGreen"/>.</summary>
    public static readonly AnsiToken DarkGreen = new("\e[32m");

    /// <summary>Foreground token for <see cref="ConsoleColor.DarkCyan"/>.</summary>
    public static readonly AnsiToken DarkCyan = new("\e[36m");

    /// <summary>Foreground token for <see cref="ConsoleColor.DarkRed"/>.</summary>
    public static readonly AnsiToken DarkRed = new("\e[31m");

    /// <summary>Foreground token for <see cref="ConsoleColor.DarkMagenta"/>.</summary>
    public static readonly AnsiToken DarkMagenta = new("\e[35m");

    /// <summary>Foreground token for <see cref="ConsoleColor.DarkYellow"/>.</summary>
    public static readonly AnsiToken DarkYellow = new("\e[33m");

    /// <summary>Foreground token for <see cref="ConsoleColor.Gray"/>.</summary>
    public static readonly AnsiToken Gray = new("\e[37m");

    /// <summary>Foreground token for <see cref="ConsoleColor.DarkGray"/>.</summary>
    public static readonly AnsiToken DarkGray = new("\e[90m");

    /// <summary>Foreground token for <see cref="ConsoleColor.Blue"/>.</summary>
    public static readonly AnsiToken Blue = new("\e[94m");

    /// <summary>Foreground token for <see cref="ConsoleColor.Green"/>.</summary>
    public static readonly AnsiToken Green = new("\e[92m");

    /// <summary>Foreground token for <see cref="ConsoleColor.Cyan"/>.</summary>
    public static readonly AnsiToken Cyan = new("\e[96m");

    /// <summary>Foreground token for <see cref="ConsoleColor.Red"/>.</summary>
    public static readonly AnsiToken Red = new("\e[91m");

    /// <summary>Foreground token for <see cref="ConsoleColor.Magenta"/>.</summary>
    public static readonly AnsiToken Magenta = new("\e[95m");

    /// <summary>Foreground token for <see cref="ConsoleColor.Yellow"/>.</summary>
    public static readonly AnsiToken Yellow = new("\e[93m");

    /// <summary>Foreground token for <see cref="ConsoleColor.White"/>.</summary>
    public static readonly AnsiToken White = new("\e[97m");


    /// <summary>Background token for <see cref="ConsoleColor.Black"/>.</summary>
    public static readonly AnsiToken BlackBackground = new("\e[40m");

    /// <summary>Background token for <see cref="ConsoleColor.DarkBlue"/>.</summary>
    public static readonly AnsiToken DarkBlueBackground = new("\e[44m");

    /// <summary>Background token for <see cref="ConsoleColor.DarkGreen"/>.</summary>
    public static readonly AnsiToken DarkGreenBackground = new("\e[42m");

    /// <summary>Background token for <see cref="ConsoleColor.DarkCyan"/>.</summary>
    public static readonly AnsiToken DarkCyanBackground = new("\e[46m");

    /// <summary>Background token for <see cref="ConsoleColor.DarkRed"/>.</summary>
    public static readonly AnsiToken DarkRedBackground = new("\e[41m");

    /// <summary>Background token for <see cref="ConsoleColor.DarkMagenta"/>.</summary>
    public static readonly AnsiToken DarkMagentaBackground = new("\e[45m");

    /// <summary>Background token for <see cref="ConsoleColor.DarkYellow"/>.</summary>
    public static readonly AnsiToken DarkYellowBackground = new("\e[43m");

    /// <summary>Background token for <see cref="ConsoleColor.Gray"/>.</summary>
    public static readonly AnsiToken GrayBackground = new("\e[47m");

    /// <summary>Background token for <see cref="ConsoleColor.DarkGray"/>.</summary>
    public static readonly AnsiToken DarkGrayBackground = new("\e[100m");

    /// <summary>Background token for <see cref="ConsoleColor.Blue"/>.</summary>
    public static readonly AnsiToken BlueBackground = new("\e[104m");

    /// <summary>Background token for <see cref="ConsoleColor.Green"/>.</summary>
    public static readonly AnsiToken GreenBackground = new("\e[102m");

    /// <summary>Background token for <see cref="ConsoleColor.Cyan"/>.</summary>
    public static readonly AnsiToken CyanBackground = new("\e[106m");

    /// <summary>Background token for <see cref="ConsoleColor.Red"/>.</summary>
    public static readonly AnsiToken RedBackground = new("\e[101m");

    /// <summary>Background token for <see cref="ConsoleColor.Magenta"/>.</summary>
    public static readonly AnsiToken MagentaBackground = new("\e[105m");

    /// <summary>Background token for <see cref="ConsoleColor.Yellow"/>.</summary>
    public static readonly AnsiToken YellowBackground = new("\e[103m");

    /// <summary>Background token for <see cref="ConsoleColor.White"/>.</summary>
    public static readonly AnsiToken WhiteBackground = new("\e[107m");

    /// <summary>
    /// Gets the cached foreground ANSI token for the provided color.
    /// </summary>
    public static AnsiToken Foreground(ConsoleColor color) => AnsiColors.Foreground(color);

    /// <summary>
    /// Gets the cached background ANSI token for the provided color.
    /// </summary>
    public static AnsiToken Background(ConsoleColor color) => AnsiColors.Background(color);
}
