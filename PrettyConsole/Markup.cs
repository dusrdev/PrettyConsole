namespace PrettyConsole;

/// <summary>
/// Provides guarded ANSI tokens for simple inline decorations.
/// </summary>
public static class Markup {
    /// <summary>
    /// Resets all decorations and colors.
    /// </summary>
    public static readonly AnsiToken Reset = new("\e[0m");

    /// <summary>
    /// Enables underlined text.
    /// </summary>
    public static readonly AnsiToken Underline = new("\e[4m");

    /// <summary>
    /// Disables underlined text.
    /// </summary>
    public static readonly AnsiToken ResetUnderline = new("\e[24m");

    /// <summary>
    /// Enables bold text.
    /// </summary>
    public static readonly AnsiToken Bold = new("\e[1m");

    /// <summary>
    /// Disables bold text.
    /// </summary>
    public static readonly AnsiToken ResetBold = new("\e[22m");

    /// <summary>
    /// Enables italic text.
    /// </summary>
    public static readonly AnsiToken Italic = new("\e[3m");

    /// <summary>
    /// Disables italic text.
    /// </summary>
    public static readonly AnsiToken ResetItalic = new("\e[23m");

    /// <summary>
    /// Enables strikethrough text.
    /// </summary>
    public static readonly AnsiToken Strikethrough = new("\e[9m");

    /// <summary>
    /// Disables strikethrough text.
    /// </summary>
    public static readonly AnsiToken ResetStrikethrough = new("\e[29m");
}
