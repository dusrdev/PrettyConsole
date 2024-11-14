namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Write a <see cref="ColoredOutput"/> to the error console
    /// </summary>
    /// <param name="output"/>
    /// <param name="pipe">The output pipe to use</param>
    /// <remarks>
    /// To not end line, use <see cref="Write(ColoredOutput, OutputPipe)"/>
    /// </remarks>
    public static void WriteLine(ColoredOutput output, OutputPipe pipe = OutputPipe.Out) {
        Write(output, pipe);
        NewLine();
    }

    /// <summary>
    /// WriteLine a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    /// <param name="outputs"></param>
    ///  <param name="pipe">The output pipe to use</param>
    /// <remarks>
    /// In overloads of WriteLine with multiple <see cref="ColoredOutput"/> parameters, only the last <see cref="ColoredOutput"/> will end the line.
    /// </remarks>
    public static void WriteLine(ReadOnlySpan<ColoredOutput> outputs, OutputPipe pipe = OutputPipe.Out) {
        if (outputs.Length is 0) {
            return;
        }

        Write(outputs, pipe);
        NewLine();
    }

    /// <summary>
    /// WriteLine an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pipe">The output pipe to use</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException">If the result of formatted item length is > 256 characters</exception>
    public static void WriteLine<T>(T item, OutputPipe pipe = OutputPipe.Out) where T : ISpanFormattable {
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
    /// <exception cref="ArgumentException">If the result of formatted item length is > 256 characters</exception>
    public static void WriteLine<T>(T item, OutputPipe pipe, ConsoleColor foreground) where T : ISpanFormattable {
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
    /// <exception cref="ArgumentException">If the result of formatted item length is > 256 characters</exception>
    public static void WriteLine<T>(T item, OutputPipe pipe, ConsoleColor foreground,
        ConsoleColor background) where T : ISpanFormattable {
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
    /// <exception cref="ArgumentException">If the result of formatted item length is > 256 characters</exception>
    public static void WriteLine<T>(T item, OutputPipe pipe, ConsoleColor foreground,
        ConsoleColor background, ReadOnlySpan<char> format, IFormatProvider? formatProvider)
    where T : ISpanFormattable {
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