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

	/// <summary>
	/// Gets the appropriate <see cref="TextWriter"/> based on <paramref name="pipe"/> 
	/// </summary>
	/// <param name="pipe"></param>
	/// <returns></returns>
	internal static TextWriter GetWriter(OutputPipe pipe)
		=> pipe switch {
			OutputPipe.Error => Error,
			_ => Out
		};
}
