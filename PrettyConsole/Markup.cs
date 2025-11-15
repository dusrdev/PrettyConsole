namespace PrettyConsole;

/// <summary>
/// Provides ANSI escape sequences for simple inline decorations.
/// </summary>
public static class Markup {
    /// <summary>
    /// Gets a value indicating whether markup sequences are emitted.
    /// </summary>
    public static readonly bool Enabled;

    /// <summary>
    /// Resets all decorations and colors.
    /// </summary>
    public static readonly string Reset = string.Empty;

    /// <summary>
    /// Enables underlined text.
    /// </summary>
    public static readonly string Underline = string.Empty;

    /// <summary>
    /// Disables underlined text.
    /// </summary>
    public static readonly string ResetUnderline = string.Empty;

    /// <summary>
    /// Enables bold text.
    /// </summary>
    public static readonly string Bold = string.Empty;

    /// <summary>
    /// Disables bold text.
    /// </summary>
    public static readonly string ResetBold = string.Empty;

    /// <summary>
    /// Enables italic text.
    /// </summary>
    public static readonly string Italic = string.Empty;

    /// <summary>
    /// Disables italic text.
    /// </summary>
    public static readonly string ResetItalic = string.Empty;

    /// <summary>
    /// Enables strikethrough text.
    /// </summary>
    public static readonly string Strikethrough = string.Empty;

    /// <summary>
    /// Disables strikethrough text.
    /// </summary>
    public static readonly string ResetStrikethrough = string.Empty;

    static Markup() {
        Enabled = !Console.IsOutputRedirected && !Console.IsErrorRedirected;
        if (Enabled) {
            Reset = "\e[0m";
            Underline = "\e[4m";
            ResetUnderline = "\e[24m";
            Bold = "\e[1m";
            ResetBold = "\e[22m";
            Italic = "\e[3m";
            ResetItalic = "\e[23m";
            Strikethrough = "\e[9m";
            ResetStrikethrough = "\e[29m";
        }
    }
}