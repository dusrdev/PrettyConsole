// ReSharper disable InconsistentNaming
namespace PrettyConsole;

public readonly partial record struct Color {
	/// <summary>
	/// Represents the default color for the shell (changes based on platform)
	/// </summary>
	public const ConsoleColor DefaultForegroundColor = (ConsoleColor)(-1);

	/// <summary>
	/// Represents the default color for the shell (changes based on platform)
	/// </summary>
	public const ConsoleColor DefaultBackgroundColor = (ConsoleColor)(-1);
}