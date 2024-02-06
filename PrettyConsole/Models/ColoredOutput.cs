namespace PrettyConsole.Models;

/// <summary>
/// Represents a colored output with string value, foreground color, and background color.
/// </summary>
public readonly record struct ColoredOutput(string Value, ConsoleColor ForegroundColor, ConsoleColor BackgroundColor) {
	/// <summary>
	/// Initializes a new instance of the <see cref="ColoredOutput"/> struct with the default colors
	/// </summary>
	public ColoredOutput(string value) : this(value, ConsoleColor.Gray, ConsoleColor.Black) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="ColoredOutput"/> struct with the default background color.
	/// </summary>
	public ColoredOutput(string value, ConsoleColor foregroundColor) : this(value, foregroundColor, ConsoleColor.Black) { }

	/// <summary>
	/// Implicitly converts a string to a <see cref="ColoredOutput"/> with default colors.
	/// </summary>
	public static implicit operator ColoredOutput(string value) => new(value);

	/// <summary>
	/// Implicitly converts a <see cref="ReadOnlySpan{T}"/> to a <see cref="ColoredOutput"/> with default colors.
	/// </summary>

	public static implicit operator ColoredOutput(ReadOnlySpan<char> buffer) => new(new string(buffer));

	/// <summary>
	/// Creates a new instance of <see cref="ColoredOutput"/> with a different background color
	/// </summary>
	public static ColoredOutput operator |(ColoredOutput output, Color color) => new(output.Value, output.ForegroundColor, color);
}