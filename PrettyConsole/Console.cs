using System.Runtime.Versioning;

namespace PrettyConsole;

/// <summary>
/// The static class the provides the abstraction over System.Console
/// </summary>
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
public static partial class Console {
	internal static readonly char[] WhiteSpace = new char[256];

	static Console() {
		WhiteSpace.AsSpan().Fill(' ');
	}
}
