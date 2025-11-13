// ReSharper disable InconsistentNaming
using System.Runtime.Versioning;

namespace PrettyConsole;

/// <summary>
/// Represents a color used for console output.
/// </summary>
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
public readonly partial record struct Color(ConsoleColor ConsoleColor) {
    /// <summary>
    /// Implicitly converts a <see cref="Color"/> to a <see cref="ConsoleColor"/>.
    /// </summary>
    /// <param name="color">The <see cref="Color"/> to convert.</param>
    /// <returns>The <see cref="ConsoleColor"/> value associated with the specified <see cref="Color"/>.</returns>
    public static implicit operator ConsoleColor(Color color) {
        return color.ConsoleColor;
    }

    /// <summary>
    /// Creates a tuple for foreground and background color
    /// </summary>
    /// <param name="foreground"></param>
    /// <param name="background"></param>
    /// <returns></returns>
    public static (ConsoleColor fg, ConsoleColor bg) operator /(Color foreground, Color background) {
        return (foreground, background);
    }

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color black.
    /// </summary>
    public static readonly Color Black = new(ConsoleColor.Black);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color dark blue.
    /// </summary>
    public static readonly Color DarkBlue = new(ConsoleColor.DarkBlue);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color dark green.
    /// </summary>
    public static readonly Color DarkGreen = new(ConsoleColor.DarkGreen);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color dark cyan.
    /// </summary>
    public static readonly Color DarkCyan = new(ConsoleColor.DarkCyan);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color dark red.
    /// </summary>
    public static readonly Color DarkRed = new(ConsoleColor.DarkRed);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color dark magenta.
    /// </summary>
    public static readonly Color DarkMagenta = new(ConsoleColor.DarkMagenta);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color dark yellow.
    /// </summary>
    public static readonly Color DarkYellow = new(ConsoleColor.DarkYellow);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color gray.
    /// </summary>
    public static readonly Color Gray = new(ConsoleColor.Gray);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color dark gray.
    /// </summary>
    public static readonly Color DarkGray = new(ConsoleColor.DarkGray);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color blue.
    /// </summary>
    public static readonly Color Blue = new(ConsoleColor.Blue);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color green.
    /// </summary>
    public static readonly Color Green = new(ConsoleColor.Green);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color cyan.
    /// </summary>
    public static readonly Color Cyan = new(ConsoleColor.Cyan);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color red.
    /// </summary>
    public static readonly Color Red = new(ConsoleColor.Red);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color magenta.
    /// </summary>
    public static readonly Color Magenta = new(ConsoleColor.Magenta);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color yellow.
    /// </summary>
    public static readonly Color Yellow = new(ConsoleColor.Yellow);

    /// <summary>
    /// Gets a <see cref="Color"/> object representing the color white.
    /// </summary>
    public static readonly Color White = new(ConsoleColor.White);
}