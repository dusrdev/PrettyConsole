namespace PrettyConsole;

public readonly partial record struct Color {
	/// <summary>
	/// Represents the default color for the shell (changes based on platform)
	/// </summary>
	public static readonly ConsoleColor DefaultForegroundColor;

	/// <summary>
	/// Represents the default color for the shell (changes based on platform)
	/// </summary>
	public static readonly ConsoleColor DefaultBackgroundColor;

	static Color() {
		baseConsole.ResetColor();
		DefaultForegroundColor = baseConsole.ForegroundColor;
		DefaultBackgroundColor = baseConsole.BackgroundColor;
	}
}