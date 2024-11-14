namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// The size of the buffer used for <see cref="ISpanFormattable"/> items
    /// </summary>
    private const int SpanFormattableBufferSize = 256;

    /// <summary>
    /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pipe">The output pipe to use</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException">If the result of formatted item length is > 256 characters</exception>
    public static void Write<T>(T item, OutputPipe pipe = OutputPipe.Out) where T : ISpanFormattable {
        Write(item, pipe, Color.DefaultForegroundColor, Color.DefaultBackgroundColor, ReadOnlySpan<char>.Empty, null);
    }

    /// <summary>
    /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pipe">The output pipe to use</param>
    /// <param name="foreground">foreground color</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException">If the result of formatted item length is > 256 characters</exception>
    public static void Write<T>(T item, OutputPipe pipe, ConsoleColor foreground) where T : ISpanFormattable {
        Write(item, pipe, foreground, Color.DefaultBackgroundColor, ReadOnlySpan<char>.Empty, null);
    }

    /// <summary>
    /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pipe">The output pipe to use</param>
    /// <param name="foreground">foreground color</param>
    /// <param name="background">background color</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException">If the result of formatted item length is > 256 characters</exception>
    public static void Write<T>(T item, OutputPipe pipe, ConsoleColor foreground,
        ConsoleColor background) where T : ISpanFormattable {
        Write(item, pipe, foreground, background, ReadOnlySpan<char>.Empty, null);
    }

    /// <summary>
    /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer,
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
    public static void Write<T>(T item, OutputPipe pipe, ConsoleColor foreground,
        ConsoleColor background, ReadOnlySpan<char> format, IFormatProvider? formatProvider)
    where T : ISpanFormattable {
        using var memoryOwner = Utils.ObtainMemory(SpanFormattableBufferSize);
        var span = memoryOwner.Memory.Span;
        if (!item.TryFormat(span, out int charsWritten, format, formatProvider)) {
            throw new ArgumentException($"Formatted item length > {SpanFormattableBufferSize}, please use a different overload", nameof(item));
        }
        Write(span.Slice(0, charsWritten), pipe, foreground, background);
    }

    /// <summary>
    /// Writes a <see cref="ReadOnlySpan{Char}"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="span"></param>
    /// <param name="pipe">The output pipe to use</param>
    /// <param name="foreground">foreground color</param>
    public static void Write(ReadOnlySpan<char> span, OutputPipe pipe, ConsoleColor foreground) {
        Write(span, pipe, foreground, Color.DefaultBackgroundColor);
    }

    /// <summary>
    /// Writes a <see cref="ReadOnlySpan{Char}"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="span"></param>
    /// <param name="pipe">The output pipe to use</param>
    /// <param name="foreground">foreground color</param>
    /// <param name="background">background color</param>
    public static void Write(ReadOnlySpan<char> span, OutputPipe pipe, ConsoleColor foreground, ConsoleColor background) {
        SetColors(foreground, background);
        if (pipe == OutputPipe.Out) {
            Out.Write(span);
        } else {
            Error.Write(span);
        }
        ResetColors();
    }

    /// <summary>
    /// Write a <see cref="ColoredOutput"/> to the error console
    /// </summary>
    /// <param name="output"/>
    /// <param name="pipe">The output pipe to use</param>
    /// <remarks>
    /// To end line, use <see cref="WriteLine(ColoredOutput, OutputPipe)"/>
    /// </remarks>
    public static void Write(ColoredOutput output, OutputPipe pipe = OutputPipe.Out) {
        SetColors(output.ForegroundColor, output.BackgroundColor);
        if (pipe == OutputPipe.Out) {
            Out.Write(output.Value);
        } else {
            Error.Write(output.Value);
        }
        ResetColors();
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    /// <param name="outputs"></param>
    /// <param name="pipe">The output pipe to use</param>
    public static void Write(ReadOnlySpan<ColoredOutput> outputs, OutputPipe pipe = OutputPipe.Out) {
        if (outputs.Length is 0) {
            return;
        }
        foreach (var output in outputs) {
            Write(output, pipe);
        }
    }
}