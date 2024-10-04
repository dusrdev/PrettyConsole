namespace PrettyConsole;

/// <summary>
/// Provides extension methods for <see cref="ColoredOutput"/>.
/// </summary>
public static class ColoredOutputExtensions {
	/// <summary>
	/// Creates a new instance of <see cref="ColoredOutput"/>.
	/// </summary>
	public static ColoredOutput InColor(this string value, ConsoleColor foregroundColor)
        => InColor(value, foregroundColor, Color.DefaultBackgroundColor);

	/// <summary>
	/// Creates a new instance of <see cref="ColoredOutput"/>.
	/// </summary>
	public static ColoredOutput InColor(this string value, ConsoleColor foregroundColor,
        ConsoleColor backgroundColor) => new(value, foregroundColor, backgroundColor);
}