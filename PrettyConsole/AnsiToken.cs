namespace PrettyConsole;

/// <summary>
/// Represents a guarded ANSI token that emits its escape sequence through <see cref="PrettyConsoleInterpolatedStringHandler"/>.
/// </summary>
/// <param name="Value">The ANSI sequence</param>
public record AnsiToken(string Value) {
    /// <summary>
    /// Converts a <see cref="ConsoleColor"/> to its corresponding foreground ANSI token.
    /// </summary>
    public static implicit operator AnsiToken(ConsoleColor color) => AnsiColors.Foreground(color);
}
