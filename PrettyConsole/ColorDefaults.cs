// ReSharper disable InconsistentNaming
using System.Runtime.InteropServices;

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
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
			const ConsoleColor unknown = (ConsoleColor)(-1);
			DefaultForegroundColor = unknown;
			DefaultBackgroundColor = unknown;
			return;
		}

		baseConsole.ResetColor();
		DefaultForegroundColor = baseConsole.ForegroundColor;
		DefaultBackgroundColor = baseConsole.BackgroundColor;
	}
}