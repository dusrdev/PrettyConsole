namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer
    /// </summary>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException">If the result of formatted item length is > 256 characters</exception>
    public static void Write<T>(T item) where T : ISpanFormattable
        => Write(item, Color.DefaultForegroundColor, Color.DefaultBackgroundColor, ReadOnlySpan<char>.Empty, null);

    /// <summary>
    /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="item"></param>
    /// <param name="foreground">foreground color</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException">If the result of formatted item length is > 256 characters</exception>
    public static void Write<T>(T item, ConsoleColor foreground) where T : ISpanFormattable
        => Write(item, foreground, Color.DefaultBackgroundColor, ReadOnlySpan<char>.Empty, null);

    /// <summary>
    /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="item"></param>
    /// <param name="foreground">foreground color</param>
    /// <param name="background">background color</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException">If the result of formatted item length is > 256 characters</exception>
    public static void Write<T>(T item, ConsoleColor foreground,
        ConsoleColor background) where T : ISpanFormattable
        => Write(item, foreground, background, ReadOnlySpan<char>.Empty, null);

    /// <summary>
    /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="item"></param>
    /// <param name="foreground">foreground color</param>
    /// <param name="background">background color</param>
    /// <param name="format">item format</param>
    /// <param name="formatProvider">format provider</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException">If the result of formatted item length is > 256 characters</exception>
    public static void Write<T>(T item, ConsoleColor foreground,
        ConsoleColor background, ReadOnlySpan<char> format, IFormatProvider? formatProvider)
    where T : ISpanFormattable {
        const int bufferSize = 50;
        using var memoryOwner = Utils.ObtainMemory(bufferSize);
        var span = memoryOwner.Memory.Span;
        if (!item.TryFormat(span, out int charsWritten, format, formatProvider)) {
            throw new ArgumentException($"Formatted item length > {bufferSize}, please use a different overload", nameof(item));
        }
        ResetColors();
        baseConsole.ForegroundColor = foreground;
        baseConsole.BackgroundColor = background;
        Out.Write(span.Slice(0, charsWritten));
        ResetColors();
    }

    /// <summary>
    /// Write a <see cref="ColoredOutput"/> to the error console
    /// </summary>
    /// <param name="output"/>
    /// <remarks>
    /// To end line, use <see cref="WriteLine(ColoredOutput)"/>
    /// </remarks>
    public static void Write(ColoredOutput output) {
        ResetColors();
        baseConsole.ForegroundColor = output.ForegroundColor;
        baseConsole.BackgroundColor = output.BackgroundColor;
        Out.Write(output.Value);
        ResetColors();
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(ReadOnlySpan<ColoredOutput> outputs) {
        if (outputs.Length is 0) {
            return;
        }

        if (outputs.Length is 1) {
            Write(outputs[0]);
        } else {
            foreach (var output in outputs) {
                Write(output);
            }
        }
    }

    /// <summary>
    /// Write a <see cref="ColoredOutput"/> to the error console
    /// </summary>
    /// <param name="output"/>
    /// <remarks>
    /// To end line, use <see cref="WriteLineError(ColoredOutput)"/>
    /// </remarks>
    public static void WriteError(ColoredOutput output) {
        ResetColors();
        baseConsole.ForegroundColor = output.ForegroundColor;
        baseConsole.BackgroundColor = output.BackgroundColor;
        Error.Write(output.Value);
        ResetColors();
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void WriteError(ReadOnlySpan<ColoredOutput> outputs) {
        if (outputs.Length is 0) {
            return;
        }

        if (outputs.Length is 1) {
            WriteError(outputs[0]);
        } else {
            foreach (var output in outputs) {
                WriteError(output);
            }
        }
    }
}