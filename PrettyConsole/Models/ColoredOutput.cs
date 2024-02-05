using System;

namespace PrettyConsole.Models;

/// <summary>
/// Represents a colored output with string value, foreground color, and background color.
/// </summary>
public readonly ref struct ColoredOutput {
	/// <summary>
	/// Initializes a new instance of the <see cref="ColoredOutput"/> struct with the default colors
	/// </summary>
	public ColoredOutput(string value) : this(value, ConsoleColor.Gray, ConsoleColor.Black) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="ColoredOutput"/> struct with the default background color.
	/// </summary>
	public ColoredOutput(string value, ConsoleColor foregroundColor) : this(value, foregroundColor, ConsoleColor.Black) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="ColoredOutput"/> struct.
	/// </summary>
	/// <param name="value">The string value of the output.</param>
	/// <param name="foregroundColor">The foreground color of the output.</param>
	/// <param name="backgroundColor">The background color of the output.</param>
	public ColoredOutput(string value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) {
		Value = value;
		ForegroundColor = foregroundColor;
		BackgroundColor = backgroundColor;
	}

	/// <summary>
	/// The string value of the output.
	/// </summary>
	public string Value { get; init; }

	/// <summary>
	/// The foreground color of the output.
	/// </summary>
	public ConsoleColor ForegroundColor { get; init; }

	/// <summary>
	/// The background color of the output.
	/// </summary>
	public ConsoleColor BackgroundColor { get; init; }

	/// <summary>
	/// Implicitly converts a string to a <see cref="ColoredOutput"/> with default colors.
	/// </summary>
	public static implicit operator ColoredOutput(string value) => new(value);

	/// <summary>
	/// Creates a new instance of <see cref="ColoredOutput"/> with a different background color
	/// </summary>
	public static ColoredOutput operator |(ColoredOutput output, Color color) => new(output.Value, output.ForegroundColor, color);
}