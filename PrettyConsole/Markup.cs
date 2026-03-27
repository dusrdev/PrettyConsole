namespace PrettyConsole;

/// <summary>
/// Represents a guarded markup token that emits an ANSI escape sequence through <see cref="PrettyConsoleInterpolatedStringHandler"/>.
/// </summary>
/// <param name="Value">The ANSI sequence</param>
public record MarkupToken(string Value);

/// <summary>
/// Provides guarded markup tokens for simple inline decorations.
/// </summary>
public static class Markup {
    /// <summary>
    /// Resets all decorations and colors.
    /// </summary>
    public static readonly MarkupToken Reset = new("\e[0m");

    /// <summary>
    /// Enables underlined text.
    /// </summary>
    public static readonly MarkupToken Underline = new("\e[4m");

    /// <summary>
    /// Disables underlined text.
    /// </summary>
    public static readonly MarkupToken ResetUnderline = new("\e[24m");

    /// <summary>
    /// Enables bold text.
    /// </summary>
    public static readonly MarkupToken Bold = new("\e[1m");

    /// <summary>
    /// Disables bold text.
    /// </summary>
    public static readonly MarkupToken ResetBold = new("\e[22m");

    /// <summary>
    /// Enables italic text.
    /// </summary>
    public static readonly MarkupToken Italic = new("\e[3m");

    /// <summary>
    /// Disables italic text.
    /// </summary>
    public static readonly MarkupToken ResetItalic = new("\e[23m");

    /// <summary>
    /// Enables strikethrough text.
    /// </summary>
    public static readonly MarkupToken Strikethrough = new("\e[9m");

    /// <summary>
    /// Disables strikethrough text.
    /// </summary>
    public static readonly MarkupToken ResetStrikethrough = new("\e[29m");
}
