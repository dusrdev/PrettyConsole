using System.Runtime.Versioning;

namespace PrettyConsole;

/// <summary>
/// The static class the provides the abstraction over <see cref="Console"/> and other extensions.
/// </summary>
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
public static class ConsoleContext {
    /// <summary>
    /// The standard output stream.
    /// </summary>
    public static TextWriter Out { get; set; } = Console.Out;

    /// <summary>
    /// The standard error stream.
    /// </summary>
    public static TextWriter Error { get; set; } = Console.Error;

    /// <summary>
    /// The standard input stream.
    /// </summary>
    public static TextReader In { get; set; } = Console.In;

    /// <summary>
    /// Gets the appropriate <see cref="TextWriter"/> based on <paramref name="pipe"/>
    /// </summary>
    /// <param name="pipe"></param>
    internal static TextWriter GetWriter(OutputPipe pipe)
        => pipe switch {
            OutputPipe.Error => Error,
            _ => Out
        };

    /// <summary>
    /// Returns the current console buffer width or <paramref name="defaultWidth"/> if <see cref="Console.IsOutputRedirected"/>
    /// </summary>
    /// <param name="defaultWidth"></param>
    internal static int GetWidthOrDefault(int defaultWidth = 120) {
        if (Console.IsOutputRedirected) {
            return defaultWidth;
        }
        return Console.BufferWidth;
    }

    extension(TextWriter @this) {
        /// <summary>
        /// Writes whitespace to this <see cref="TextWriter"/> up to length by chucks
        /// </summary>
        /// <param name="length"></param>
        public void WriteWhiteSpaces(int length) {
            ReadOnlySpan<char> whiteSpaces = WhiteSpaces;

            while (length > 0) {
                int curLength = Math.Min(length, 256);
                @this.Write(whiteSpaces.Slice(0, curLength));
                length -= curLength;
            }
        }
    }

    private static readonly string WhiteSpaces = new(' ', 256);
}