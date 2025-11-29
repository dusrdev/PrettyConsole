namespace PrettyConsole;

/// <summary>
/// Provides ANSI escape sequences for simple inline decorations.
/// </summary>
public static class Markup {
    /// <summary>
    /// Resets all decorations and colors.
    /// </summary>
    public const string Reset = "\e[0m";

    /// <summary>
    /// Enables underlined text.
    /// </summary>
    public const string Underline = "\e[4m";

    /// <summary>
    /// Disables underlined text.
    /// </summary>
    public const string ResetUnderline = "\e[24m";

    /// <summary>
    /// Enables bold text.
    /// </summary>
    public const string Bold = "\e[1m";

    /// <summary>
    /// Disables bold text.
    /// </summary>
    public const string ResetBold = "\e[22m";

    /// <summary>
    /// Enables italic text.
    /// </summary>
    public const string Italic = "\e[3m";

    /// <summary>
    /// Disables italic text.
    /// </summary>
    public const string ResetItalic = "\e[23m";

    /// <summary>
    /// Enables strikethrough text.
    /// </summary>
    public const string Strikethrough = "\e[9m";

    /// <summary>
    /// Disables strikethrough text.
    /// </summary>
    public const string ResetStrikethrough = "\e[29m";
}