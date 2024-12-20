using System.Runtime.Versioning;

namespace PrettyConsole;

/// <summary>
/// Represents a colored output with string value, foreground color, and background color.
/// </summary>
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
public readonly record struct ColoredOutput(string Value, ConsoleColor ForegroundColor, ConsoleColor BackgroundColor) {
	/// <summary>
	/// Creates a new instance of <see cref="ColoredOutput"/> with default colors
	/// </summary>
	/// <param name="value"></param>
	public ColoredOutput(string value) : this(value, Color.DefaultForegroundColor, Color.DefaultBackgroundColor) { }

	/// <summary>
	/// Creates a new instance of <see cref="ColoredOutput"/> with default background color
	/// </summary>
	/// <param name="value"></param>
	/// <param name="foregroundColor"></param>
	public ColoredOutput(string value, ConsoleColor foregroundColor) : this(value, foregroundColor, Color.DefaultBackgroundColor) { }

    /// <summary>
    /// Implicitly converts a string to a <see cref="ColoredOutput"/> with default colors.
    /// </summary>
    public static implicit operator ColoredOutput(string value) {
        return new(value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="ReadOnlySpan{T}"/> to a <see cref="ColoredOutput"/> with default colors.
    /// </summary>
    public static implicit operator ColoredOutput(ReadOnlySpan<char> buffer) {
        return new(new string(buffer));
    }

    /// <summary>
    /// Creates a new instance of <see cref="ColoredOutput"/> with a different background color
    /// </summary>
    public static ColoredOutput operator /(ColoredOutput output, Color color) {
        return output with { BackgroundColor = color };
    }
}