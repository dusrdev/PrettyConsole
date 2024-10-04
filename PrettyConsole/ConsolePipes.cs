namespace PrettyConsole;

public static partial class Console {
	/// <summary>
	/// The standard input stream.
	/// </summary>
	public static TextWriter Out { get; internal set; } = baseConsole.Out;

	/// <summary>
	/// The error output stream.
	/// </summary>
	public static TextWriter Error { get; internal set; } = baseConsole.Error;

	/// <summary>
	/// The standard input stream.
	/// </summary>
	public static TextReader In { get; internal set; } = baseConsole.In;
}
