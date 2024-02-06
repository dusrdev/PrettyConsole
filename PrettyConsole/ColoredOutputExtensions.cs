namespace PrettyConsole;

/// <summary>
/// Provides extension methods for <see cref="ColoredOutput"/>.
/// </summary>
public static class ColoredOutputExtensions {
	/// <summary>
	/// Creates a new instance of <see cref="ColoredOutput"/>.
	/// </summary>
	public static ColoredOutput InColor(this string value, ConsoleColor foregroundColor, ConsoleColor backgroundColor = ConsoleColor.Black) => new(value, foregroundColor, backgroundColor);
}