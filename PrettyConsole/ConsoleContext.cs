using System.Runtime.Versioning;

namespace PrettyConsole;

/// <summary>
/// The static class that provides the abstraction over <see cref="Console"/> and other extensions.
/// </summary>
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
public static partial class ConsoleContext {
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
    internal static TextWriter GetPipeTarget(OutputPipe pipe)
        => pipe switch {
            OutputPipe.Error => Error,
            _ => Out
        };

    /// <summary>
    /// Gets the appropriate <see cref="TextWriter"/> based on <paramref name="pipe"/>
    /// </summary>
    /// <param name="pipe"></param>
    internal static (TextWriter Writer, bool IsRedirected) GetPipeTargetAndState(OutputPipe pipe) {
        return pipe switch {
            OutputPipe.Out => (Out, !ReferenceEquals(Out, Console.Out) || Console.IsOutputRedirected),
            OutputPipe.Error => (Error, !ReferenceEquals(Error, Console.Error) || Console.IsErrorRedirected),
            _ => throw new InvalidOperationException("A pipe that isn't Out or Error is not supported."),
        };
    }

    /// <summary>
    /// Returns the current console buffer width or <paramref name="defaultWidth"/> if <see cref="Console.IsOutputRedirected"/>
    /// </summary>
    /// <param name="defaultWidth"></param>
    internal static int GetWidthOrDefault(int defaultWidth = 120) {
        // If output is redirected or a custom writer is injected, fall back to the provided default.
        if (Console.IsOutputRedirected || !ReferenceEquals(Out, Console.Out)) {
            return defaultWidth;
        }
        return Console.BufferWidth;
    }

    extension(TextWriter @this) {
        /// <summary>
        /// Writes whitespace to this <see cref="TextWriter"/> in chunks up to the requested length.
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
