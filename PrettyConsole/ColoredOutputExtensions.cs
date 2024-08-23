namespace PrettyConsole;

/// <summary>
/// Provides extension methods for <see cref="ColoredOutput"/>.
/// </summary>
public static class ColoredOutputExtensions {
	/// <summary>
	/// Creates a new instance of <see cref="ColoredOutput"/>.
	/// </summary>
	public static ColoredOutput InColor(this string value, ConsoleColor foregroundColor,
        ConsoleColor backgroundColor = Console.UnknownColor) => new(value, foregroundColor, backgroundColor);
}