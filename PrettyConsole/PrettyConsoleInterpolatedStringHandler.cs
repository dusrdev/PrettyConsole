using System.Buffers;

using static System.Console;

namespace PrettyConsole;

#pragma warning disable CA1822 // Mark members as static
/// <summary>
/// Interpolated string handler that streams segments directly to an <see cref="OutputPipe"/> while allowing inline color changes.
/// </summary>
[InterpolatedStringHandler]
public readonly ref struct PrettyConsoleInterpolatedStringHandler {
    private readonly TextWriter _writer;
    private readonly IFormatProvider? _provider;

    /// <summary>
    /// Creates a new handler that writes to <see cref="OutputPipe.Out"/> .
    /// </summary>
    /// <param name="literalLength">Estimated literal length supplied by the compiler.</param>
    /// <param name="formattedCount">Formatted item count supplied by the compiler.</param>
    /// <param name="shouldAppend">Always <see langword="true"/>; reserved for future short-circuiting.</param>
    public PrettyConsoleInterpolatedStringHandler(int literalLength, int formattedCount, out bool shouldAppend)
        : this(literalLength, formattedCount, OutputPipe.Out, provider: null, out shouldAppend) {
    }

    /// <summary>
    /// Creates a new handler that writes to <paramref name="pipe"/>.
    /// </summary>
    /// <param name="literalLength">Estimated literal length supplied by the compiler.</param>
    /// <param name="formattedCount">Formatted item count supplied by the compiler.</param>
    /// <param name="pipe">The pipe to stream the output to.</param>
    /// <param name="shouldAppend">Always <see langword="true"/>; reserved for future short-circuiting.</param>
    public PrettyConsoleInterpolatedStringHandler(int literalLength, int formattedCount, OutputPipe pipe, out bool shouldAppend)
        : this(literalLength, formattedCount, pipe, provider: null, out shouldAppend) {
    }

    /// <summary>
    /// Creates a new handler that writes to <paramref name="pipe"/> using <paramref name="provider"/> for formatting.
    /// </summary>
    /// <param name="literalLength">Estimated literal length supplied by the compiler.</param>
    /// <param name="formattedCount">Formatted item count supplied by the compiler.</param>
    /// <param name="pipe">The pipe to stream the output to.</param>
    /// <param name="provider">Optional format provider used when formatting values.</param>
    /// <param name="shouldAppend">Always <see langword="true"/>; reserved for future short-circuiting.</param>
    public PrettyConsoleInterpolatedStringHandler(int literalLength, int formattedCount, OutputPipe pipe, IFormatProvider? provider, out bool shouldAppend) {
        _writer = PrettyConsoleExtensions.GetWriter(pipe);
        _provider = provider;
        shouldAppend = true;
    }

    /// <summary>
    /// Appends a literal segment supplied by the compiler.
    /// </summary>
    public readonly void AppendLiteral(string value) {
        _writer.Write(value);
    }

    /// <summary>
    /// Appends a formatted string value.
    /// </summary>
    /// <param name="value">Formatted string.</param>
    /// <param name="alignment">Optional alignment as provided by the interpolation.</param>
    /// <param name="format">Unused string format specifier.</param>
    public readonly void AppendFormatted(string? value, int alignment = 0, string? format = null) {
        AppendString(value, alignment);
    }

    /// <summary>
    /// Appends a span segment without allocations.
    /// </summary>
    /// <param name="value">Characters to write.</param>
    /// <param name="alignment">Optional alignment as provided by the interpolation.</param>
    public readonly void AppendFormatted(scoped ReadOnlySpan<char> value, int alignment = 0) {
        AppendSpan(value, alignment);
    }

    /// <summary>
    /// Appends a single character.
    /// </summary>
    /// <param name="value">Character to write.</param>
    /// <param name="alignment">Optional alignment as provided by the interpolation.</param>
    public readonly void AppendFormatted(char value, int alignment = 0) {
        Span<char> buffer = [value];
        AppendSpan(buffer, alignment);
    }

    /// <summary>
    /// Sets the console foreground color to <paramref name="color"/>.
    /// </summary>
    public readonly void AppendFormatted(ConsoleColor color) {
        Console.SetColors(color, BackgroundColor);
    }

    /// <summary>
    /// Sets the foreground and background colors of the console
    /// </summary>
    /// <param name="colors"></param>
    /// <param name="alignment"></param>
    public readonly void AppendFormatted((ConsoleColor foreground, ConsoleColor background) colors, int alignment = 0) {
        Console.SetColors(colors.foreground, colors.background);
        if (alignment != 0) {
            AppendSpan(ReadOnlySpan<char>.Empty, alignment);
        }
    }

    /// <summary>
    /// Append timeSpan with or without elapsed time formatting (human readable)
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="format"></param>
    public readonly void AppendFormatted(TimeSpan timeSpan, string? format = null)
        => AppendFormatted(timeSpan, alignment: 0, format);

    /// <summary>
    /// Append timeSpan with optional alignment support.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="alignment"></param>
    /// <param name="format"></param>
    public readonly void AppendFormatted(TimeSpan timeSpan, int alignment, string? format = null) {
        if (format != "hr") {
            AppendSpanFormattable(timeSpan, alignment, format);
            return;
        }

        Span<char> buffer = stackalloc char[128];
        if (buffer.TryWrite($"{(int)timeSpan.TotalHours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}", out int written)) {
            AppendSpan(buffer.Slice(0, written), alignment);
            return;
        }

        int lowerBound = 4096;
        var pool = ArrayPool<char>.Shared;

        while (true) {
            var array = pool.Rent(lowerBound);
            buffer = new Span<char>(array);
            if (buffer.TryWrite($"{(int)timeSpan.TotalHours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}", out written)) {
                AppendSpan(buffer.Slice(0, written), alignment);
                pool.Return(array);
                break;
            }
            pool.Return(array);
            lowerBound *= 2;
        }
    }

    /// <summary>
    /// Appends a value type that implements <see cref="ISpanFormattable"/> without boxing.
    /// </summary>
    public readonly void AppendFormatted<T>(T value) where T : ISpanFormattable {
        AppendSpanFormattable(value, alignment: 0, format: null);
    }

    /// <summary>
    /// Appends a value type that implements <see cref="ISpanFormattable"/> without boxing while respecting alignment.
    /// </summary>
    public readonly void AppendFormatted<T>(T value, int alignment) where T : ISpanFormattable {
        AppendSpanFormattable(value, alignment, format: null);
    }

    /// <summary>
    /// Appends a value type that implements <see cref="ISpanFormattable"/> without boxing using the provided format string.
    /// </summary>
    public readonly void AppendFormatted<T>(T value, string? format) where T : ISpanFormattable {
        AppendSpanFormattable(value, alignment: 0, format);
    }

    /// <summary>
    /// Appends a value type that implements <see cref="ISpanFormattable"/> without boxing using alignment and format string.
    /// </summary>
    public readonly void AppendFormatted<T>(T value, int alignment, string? format) where T : ISpanFormattable {
        AppendSpanFormattable(value, alignment, format);
    }

    /// <summary>
    /// Appends an object value when the compiler cannot resolve a more specific overload.
    /// </summary>
    /// <param name="value">Value to write.</param>
    /// <param name="alignment">Optional alignment as provided by the interpolation.</param>
    /// <param name="format">Optional format specifier.</param>
    public readonly void AppendFormatted(object? value, int alignment = 0, string? format = null) {
        switch (value) {
            case null: {
                    AppendSpan(ReadOnlySpan<char>.Empty, alignment);
                    break;
                }
            case ConsoleColor consoleColor: {
                    AppendFormatted(consoleColor);
                    break;
                }
            case ISpanFormattable spanFormattable: {
                    AppendSpanFormattable(spanFormattable, alignment, format);
                    break;
                }
            case IFormattable formattable: {
                    AppendString(formattable.ToString(format, _provider), alignment);
                    break;
                }
            case string str: {
                    AppendString(str, alignment);
                    break;
                }
            default: {
                    AppendString(value.ToString(), alignment);
                    break;
                }
        }
    }

    private readonly void AppendSpanFormattable<T>(T value, int alignment, string? format)
    where T : ISpanFormattable {
        ReadOnlySpan<char> formatSpan = format.AsSpan();
        Span<char> buffer = stackalloc char[128];
        if (value.TryFormat(buffer, out int charsWritten, formatSpan, _provider)) {
            AppendSpan(buffer.Slice(0, charsWritten), alignment);
            return;
        }

        int lowerBound = 4096;
        var pool = ArrayPool<char>.Shared;

        while (true) {
            var array = pool.Rent(lowerBound);
            buffer = new Span<char>(array);
            if (value.TryFormat(buffer, out charsWritten, formatSpan, _provider)) {
                AppendSpan(buffer.Slice(0, charsWritten), alignment);
                pool.Return(array);
                break;
            }
            pool.Return(array);
            lowerBound *= 2;
        }
    }

    private readonly void AppendString(string? value, int alignment) {
        // AppendSpan handles null and empty spans
        AppendSpan(value.AsSpan(), alignment);
    }

    private readonly void AppendSpan(scoped ReadOnlySpan<char> span, int alignment) {
        if (span.IsEmpty) return;
        else if (alignment == 0) {
            _writer.Write(span);
        } else {
            bool leftAlign = alignment < 0;
            int width = Math.Abs(alignment);
            int padding = width - span.Length;
            if (padding > 0 && !leftAlign) {
                WritePadding(padding);
            }

            if (!span.IsEmpty) {
                _writer.Write(span);
            }

            if (padding > 0 && leftAlign) {
                WritePadding(padding);
            }
        }
    }

    private readonly void WritePadding(int count) {
        _writer.WriteWhiteSpaces(count);
    }
}
#pragma warning restore CA1822 // Mark members as static
