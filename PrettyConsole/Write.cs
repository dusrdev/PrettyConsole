using System.Runtime.InteropServices;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pipe">The output pipe to use</param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
    /// </remarks>
    public static void Write<T>(T item, OutputPipe pipe = OutputPipe.Out)
    where T : ISpanFormattable, allows ref struct {
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
    /// <remarks>
    /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
    /// </remarks>
    public static void Write<T>(T item, OutputPipe pipe, ConsoleColor foreground)
    where T : ISpanFormattable, allows ref struct {
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
    /// <remarks>
    /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
    /// </remarks>
    public static void Write<T>(T item, OutputPipe pipe, ConsoleColor foreground, ConsoleColor background)
    where T : ISpanFormattable, allows ref struct {
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
    /// <remarks>
    /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
    /// </remarks>
    public static void Write<T>(T item, OutputPipe pipe, ConsoleColor foreground,
        ConsoleColor background, ReadOnlySpan<char> format, IFormatProvider? formatProvider)
    where T : ISpanFormattable, allows ref struct {
        using var listOwner = BufferPool.Shared.Rent(out var lst);
        int upperBound = BufferPool.ListStartingSize;
        while (true) {
            lst.EnsureCapacity(upperBound);
            CollectionsMarshal.SetCount(lst, upperBound);
            var span = CollectionsMarshal.AsSpan(lst);
            if (item.TryFormat(span, out int charsWritten, format, formatProvider)) {
                Write(span.Slice(0, charsWritten), pipe, foreground, background);
                break;
            } else {
                upperBound *= 2;
            }
        }
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
        GetWriter(pipe).Write(span);
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
        GetWriter(pipe).Write(output.Value);
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