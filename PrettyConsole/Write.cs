using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the output writer,
    /// in the same color convention as ColoredOutput
    /// </summary>
    /// <param name="item"></param>
    /// <param name="foreground">foreground color</param>
    /// <param name="background">background color</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException">If the result of formatted item length is > 256 characters</exception>
    public static void Write<T>(T item, ConsoleColor foreground = UnknownColor,
        ConsoleColor background = UnknownColor) where T : ISpanFormattable
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
        using var memoryOwner = Helper.ObtainMemory(bufferSize);
        var span = memoryOwner.Memory.Span;
        if (!item.TryFormat(span, out int charsWritten, format, formatProvider)) {
            throw new ArgumentException($"Formatted item length > {bufferSize}, please use a different overload", nameof(item));
        }
        ResetColors();
        ogConsole.ForegroundColor = foreground;
        ogConsole.BackgroundColor = background;
        ogConsole.Out.Write(span.Slice(0, charsWritten));
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
        ogConsole.ForegroundColor = output.ForegroundColor;
        ogConsole.BackgroundColor = output.BackgroundColor;
        ogConsole.Write(output.Value);
        ResetColors();
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(ColoredOutput output1, ColoredOutput output2) {
        Write(output1);
        Write(output2);
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(ColoredOutput output1, ColoredOutput output2, ColoredOutput output3) {
        Write(output1);
        Write(output2);
        Write(output3);
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(ColoredOutput output1, ColoredOutput output2, ColoredOutput output3,
        ColoredOutput output4) {
        Write(output1);
        Write(output2);
        Write(output3);
        Write(output4);
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(ColoredOutput output1, ColoredOutput output2, ColoredOutput output3, ColoredOutput output4,
        ColoredOutput output5) {
        Write(output1);
        Write(output2);
        Write(output3);
        Write(output4);
        Write(output5);
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(ColoredOutput output1, ColoredOutput output2, ColoredOutput output3, ColoredOutput output4,
        ColoredOutput output5, ColoredOutput output6) {
        Write(output1);
        Write(output2);
        Write(output3);
        Write(output4);
        Write(output5);
        Write(output6);
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(params ColoredOutput[] outputs) => Write(new ReadOnlySpan<ColoredOutput>(outputs));

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(ReadOnlySpan<ColoredOutput> outputs) {
        if (outputs.Length is 0) {
            return;
        }

        if (outputs.Length is 1) {
            Write(outputs[0]);
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
        ResetColors();
        ogConsole.ForegroundColor = output.ForegroundColor;
        ogConsole.BackgroundColor = output.BackgroundColor;
        ogConsole.Error.Write(output.Value);
        ResetColors();
    }
}