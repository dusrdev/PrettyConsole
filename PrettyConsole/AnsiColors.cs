namespace PrettyConsole;

/// <summary>
/// Provides cached ANSI tokens for the common <see cref="ConsoleColor"/>s.
/// </summary>
public static class AnsiColors {
    private static readonly AnsiToken[] ForegroundCodes = [
        Color.Black,
        Color.DarkBlue,
        Color.DarkGreen,
        Color.DarkCyan,
        Color.DarkRed,
        Color.DarkMagenta,
        Color.DarkYellow,
        Color.Gray,
        Color.DarkGray,
        Color.Blue,
        Color.Green,
        Color.Cyan,
        Color.Red,
        Color.Magenta,
        Color.Yellow,
        Color.White
    ];

    private static readonly AnsiToken[] BackgroundCodes = [
        Color.BlackBackground,
        Color.DarkBlueBackground,
        Color.DarkGreenBackground,
        Color.DarkCyanBackground,
        Color.DarkRedBackground,
        Color.DarkMagentaBackground,
        Color.DarkYellowBackground,
        Color.GrayBackground,
        Color.DarkGrayBackground,
        Color.BlueBackground,
        Color.GreenBackground,
        Color.CyanBackground,
        Color.RedBackground,
        Color.MagentaBackground,
        Color.YellowBackground,
        Color.WhiteBackground
    ];

    /// <summary>
    /// Gets the cached ANSI token for the specified foreground color.
    /// </summary>
    public static AnsiToken Foreground(ConsoleColor color) {
        int index = (int)color;
        if (index == -1) return Color.DefaultForeground;
        return ForegroundCodes[index];
    }


    /// <summary>
    /// Gets the cached ANSI token for the specified background color.
    /// </summary>
    public static AnsiToken Background(ConsoleColor color) {
        int index = (int)color;
        if (index == -1) return Color.DefaultBackground;
        return BackgroundCodes[index];
    }
}
