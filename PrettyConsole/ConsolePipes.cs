namespace PrettyConsole;

public static partial class PrettyConsoleExtensions {
    /// <summary>
    /// The standard input stream.
    /// </summary>
    public static TextWriter Out { get; internal set; } = Console.Out;

    /// <summary>
    /// The error output stream.
    /// </summary>
    public static TextWriter Error { get; internal set; } = Console.Error;

    /// <summary>
    /// The standard input stream.
    /// </summary>
    public static TextReader In { get; internal set; } = Console.In;

    /// <summary>
    /// Gets the appropriate <see cref="TextWriter"/> based on <paramref name="pipe"/>
    /// </summary>
    /// <param name="pipe"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal static TextWriter GetWriter(OutputPipe pipe)
        => pipe switch {
            OutputPipe.Error => Error,
            _ => Out
        };

    /// <summary>
    /// Returns the current console buffer width or <paramref name="defaultWidth"/> if <see cref="Console.IsOutputRedirected"/>
    /// </summary>
    /// <param name="defaultWidth"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal static int GetWidthOrDefault(int defaultWidth = 120) {
        if (Console.IsOutputRedirected) {
            return defaultWidth;
        }
        return Console.BufferWidth;
    }
}