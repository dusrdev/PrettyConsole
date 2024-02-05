using System;

namespace PrettyConsole.Models;

/// <summary>
/// Represents a color used for console output.
/// </summary>
public readonly ref struct Color {
	/// <summary>
	/// Gets the <see cref="ConsoleColor"/> value associated with this color.
	/// </summary>
	public ConsoleColor ConsoleColor { get; init; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Color"/> struct with the specified <see cref="ConsoleColor"/>.
	/// </summary>
	/// <param name="color">The <see cref="ConsoleColor"/> value to associate with this color.</param>
	public Color(ConsoleColor color) {
		ConsoleColor = color;
	}

	/// <summary>
	/// Implicitly converts a <see cref="Color"/> to a <see cref="ConsoleColor"/>.
	/// </summary>
	/// <param name="color">The <see cref="Color"/> to convert.</param>
	/// <returns>The <see cref="ConsoleColor"/> value associated with the specified <see cref="Color"/>.</returns>
	public static implicit operator ConsoleColor(Color color) => color.ConsoleColor;

	/// <summary>
	/// Creates a <see cref="ColoredOutput"/> object by combining a string value with a color.
	/// </summary>
	/// <param name="value">The string value to combine with the color.</param>
	/// <param name="color">The color to apply to the string value.</param>
	/// <returns>A <see cref="ColoredOutput"/> object representing the combination of the string value and color.</returns>
	public static ColoredOutput operator &(string value, Color color) => new(value, color);

	// Other members...

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color black.
	/// </summary>
	public static Color Black => new(ConsoleColor.Black);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color dark blue.
	/// </summary>
	public static Color DarkBlue => new(ConsoleColor.DarkBlue);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color dark green.
	/// </summary>
	public static Color DarkGreen => new(ConsoleColor.DarkGreen);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color dark cyan.
	/// </summary>
	public static Color DarkCyan => new(ConsoleColor.DarkCyan);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color dark red.
	/// </summary>
	public static Color DarkRed => new(ConsoleColor.DarkRed);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color dark magenta.
	/// </summary>
	public static Color DarkMagenta => new(ConsoleColor.DarkMagenta);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color dark yellow.
	/// </summary>
	public static Color DarkYellow => new(ConsoleColor.DarkYellow);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color gray.
	/// </summary>
	public static Color Gray => new(ConsoleColor.Gray);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color dark gray.
	/// </summary>
	public static Color DarkGray => new(ConsoleColor.DarkGray);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color blue.
	/// </summary>
	public static Color Blue => new(ConsoleColor.Blue);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color green.
	/// </summary>
	public static Color Green => new(ConsoleColor.Green);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color cyan.
	/// </summary>
	public static Color Cyan => new(ConsoleColor.Cyan);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color red.
	/// </summary>
	public static Color Red => new(ConsoleColor.Red);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color magenta.
	/// </summary>
	public static Color Magenta => new(ConsoleColor.Magenta);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color yellow.
	/// </summary>
	public static Color Yellow => new(ConsoleColor.Yellow);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the color white.
	/// </summary>
	public static Color White => new(ConsoleColor.White);

	/// <summary>
	/// Gets a <see cref="Color"/> object representing the default color (gray).
	/// </summary>
	public static Color Default => Gray;
}