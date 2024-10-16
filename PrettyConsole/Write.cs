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
    /// <exception cref="ArgumentException">If the result of formatted item length is > 50 characters</exception>
    public static void Write<T>(T item, ConsoleColor foreground,
        ConsoleColor background, ReadOnlySpan<char> format, IFormatProvider? formatProvider)
    where T : ISpanFormattable {
        const int bufferSize = 50;
        using var memoryOwner = Utils.ObtainMemory(bufferSize);
        var span = memoryOwner.Memory.Span;
        if (!item.TryFormat(span, out int charsWritten, format, formatProvider)) {
            throw new ArgumentException($"Formatted item length > {bufferSize}, please use a different overload", nameof(item));
        }
        Write(span.Slice(0, charsWritten), foreground, background);
    }

    /// <summary>
    /// Writes a <see cref="ReadOnlySpan{Char}"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="span"></param>
    /// <param name="foreground">foreground color</param>
    public static void Write(ReadOnlySpan<char> span, ConsoleColor foreground)
        => Write(span, foreground, Color.DefaultBackgroundColor);

    /// <summary>
    /// Writes a <see cref="ReadOnlySpan{Char}"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="span"></param>
    /// <param name="foreground">foreground color</param>
    /// <param name="background">background color</param>
    public static void Write(ReadOnlySpan<char> span, ConsoleColor foreground, ConsoleColor background) {
        SetColors(foreground, background);
        Out.Write(span);
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
        SetColors(output.ForegroundColor, output.BackgroundColor);
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
        foreach (var output in outputs) {
            Write(output);
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
        SetColors(output.ForegroundColor, output.BackgroundColor);
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

        foreach (var output in outputs) {
            WriteError(output);
        }
    }

    /// <summary>
    /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="item"></param>
    /// <param name="foreground">foreground color</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException">If the result of formatted item length is > 50 characters</exception>
    public static void WriteError<T>(T item, ConsoleColor foreground) where T : ISpanFormattable
        => WriteError(item, foreground, Color.DefaultBackgroundColor);

    /// <summary>
    /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="item"></param>
    /// <param name="foreground">foreground color</param>
    /// <param name="background">background color</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException">If the result of formatted item length is > 50 characters</exception>
    public static void WriteError<T>(T item, ConsoleColor foreground, ConsoleColor background) where T : ISpanFormattable {
        const int bufferSize = 50;
        using var memoryOwner = Utils.ObtainMemory(bufferSize);
        var span = memoryOwner.Memory.Span;
        if (!item.TryFormat(span, out int charsWritten, ReadOnlySpan<char>.Empty, null)) {
            throw new ArgumentException($"Formatted item length > {bufferSize}, please use a different overload", nameof(item));
        }
        WriteError(span.Slice(0, charsWritten), foreground, background);
    }

    /// <summary>
    /// Writes a <see cref="ReadOnlySpan{Char}"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="span"></param>
    /// <param name="foreground">foreground color</param>
    public static void WriteError(ReadOnlySpan<char> span, ConsoleColor foreground)
        => WriteError(span, foreground, Color.DefaultBackgroundColor);

    /// <summary>
    /// Writes a <see cref="ReadOnlySpan{Char}"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="span"></param>
    /// <param name="foreground">foreground color</param>
    /// <param name="background">background color</param>
    public static void WriteError(ReadOnlySpan<char> span, ConsoleColor foreground, ConsoleColor background) {
        SetColors(foreground, background);
        Error.Write(span);
        ResetColors();
    }
}