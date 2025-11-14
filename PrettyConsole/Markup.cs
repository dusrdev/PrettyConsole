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
            Reset = "\u001b[0m";
            Underline = "\u001b[4m";
            ResetUnderline = "\u001b[24m";
            Bold = "\u001b[1m";
            ResetBold = "\u001b[22m";
            Italic = "\u001b[3m";
            ResetItalic = "\u001b[23m";
            Strikethrough = "\u001b[9m";
            ResetStrikethrough = "\u001b[29m";
        }
    }
}
