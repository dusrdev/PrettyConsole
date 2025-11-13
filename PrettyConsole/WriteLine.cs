namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Writes interpolated content using <see cref="PrettyConsoleInterpolatedStringHandler"/> to <see cref="OutputPipe.Out"/>.
    /// </summary>
    /// <param name="handler">Interpolated string handler that streams the content.</param>
    public static void WriteLine([InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) {
        ResetColors();
        NewLine(OutputPipe.Out);
    }

    /// <summary>
    /// Writes interpolated content using <see cref="PrettyConsoleInterpolatedStringHandler"/>.
    /// </summary>
    /// <param name="pipe">Destination pipe. Defaults to <see cref="OutputPipe.Out"/>.</param>
    /// <param name="handler">Interpolated string handler that streams the content.</param>
    public static void WriteLine(OutputPipe pipe, [InterpolatedStringHandlerArgument(nameof(pipe))] PrettyConsoleInterpolatedStringHandler handler = default) {
        ResetColors();
        NewLine(pipe);
    }

    /// <summary>
    /// WriteLine an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pipe">The output pipe to use</param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
    /// </remarks>
    public static void WriteLine<T>(T item, OutputPipe pipe = OutputPipe.Out)
    where T : ISpanFormattable, allows ref struct {
        WriteLine(item, pipe, Color.DefaultForegroundColor, Color.DefaultBackgroundColor, ReadOnlySpan<char>.Empty, null);
    }

    /// <summary>
    /// WriteLine an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pipe">The output pipe to use</param>
    /// <param name="foreground">foreground color</param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
    /// </remarks>
    public static void WriteLine<T>(T item, OutputPipe pipe, ConsoleColor foreground)
    where T : ISpanFormattable, allows ref struct {
        WriteLine(item, pipe, foreground, Color.DefaultBackgroundColor, ReadOnlySpan<char>.Empty, null);
    }

    /// <summary>
    /// WriteLine an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pipe">The output pipe to use</param>
    /// <param name="foreground">foreground color</param>
    /// <param name="background">background color</param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
    /// </remarks>
    public static void WriteLine<T>(T item, OutputPipe pipe, ConsoleColor foreground,
        ConsoleColor background) where T : ISpanFormattable, allows ref struct {
        WriteLine(item, pipe, foreground, background, ReadOnlySpan<char>.Empty, null);
    }

    /// <summary>
    /// WriteLine an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pipe">The output pipe to use</param>
    /// <param name="foreground">foreground color</param>
    /// <param name="background">background color</param>
    /// <param name="format">item format</param>
    /// <param name="formatProvider">format provider</param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
    /// </remarks>
    public static void WriteLine<T>(T item, OutputPipe pipe, ConsoleColor foreground,
        ConsoleColor background, ReadOnlySpan<char> format, IFormatProvider? formatProvider)
    where T : ISpanFormattable, allows ref struct {
        Write(item, pipe, foreground, background, format, formatProvider);
        NewLine(pipe);
    }

    /// <summary>
    /// WriteLine a <see cref="ReadOnlySpan{Char}"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="span"></param>
    /// <param name="pipe">The output pipe to use</param>
    /// <param name="foreground">foreground color</param>
    public static void WriteLine(ReadOnlySpan<char> span, OutputPipe pipe, ConsoleColor foreground) {
        Write(span, pipe, foreground, Color.DefaultBackgroundColor);
        NewLine(pipe);
    }

    /// <summary>
    /// WriteLine a <see cref="ReadOnlySpan{Char}"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="span"></param>
    /// <param name="pipe">The output pipe to use</param>
    /// <param name="foreground">foreground color</param>
    /// <param name="background">background color</param>
    public static void WriteLine(ReadOnlySpan<char> span, OutputPipe pipe, ConsoleColor foreground, ConsoleColor background) {
        Write(span, pipe, foreground, background);
        NewLine(pipe);
    }
}