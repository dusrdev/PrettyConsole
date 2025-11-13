using static System.Console;
using System.Runtime.InteropServices;

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
        if (!string.IsNullOrEmpty(value)) {
            _writer.Write(value);
        }
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
        if (alignment == 0) {
            _writer.Write(value);
            return;
        }

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
    public readonly void AppendFormatted(TimeSpan timeSpan, string? format = null) {
        if (format != "hr") {
            AppendSpanFormattable(timeSpan, 0, format);
            return;
        }
        if (timeSpan.TotalSeconds < 1) {
            AppendSpanFormattable(timeSpan.Milliseconds, 0, null);
            AppendSpan("ms", 0);
        } else if (timeSpan.TotalSeconds < 60) {
            AppendSpanFormattable(timeSpan.Seconds, 0, "00");
            AppendFormatted(':');
            AppendSpanFormattable(timeSpan.Milliseconds, 0, "00");
            AppendFormatted('s');
        } else if (timeSpan.TotalSeconds < 3600) {
            AppendSpanFormattable(timeSpan.Minutes, 0, "00");
            AppendFormatted(':');
            AppendSpanFormattable(timeSpan.Seconds, 0, "00");
            AppendFormatted('m');
        } else if (timeSpan.TotalSeconds < 86400) {
            AppendSpanFormattable(timeSpan.Hours, 0, "00");
            AppendFormatted(':');
            AppendSpanFormattable(timeSpan.Minutes, 0, "00");
            AppendSpan("hr", 0);
        } else {
            AppendSpanFormattable(timeSpan.Days, 0, "00");
            AppendFormatted(':');
            AppendSpanFormattable(timeSpan.Hours, 0, "00");
            AppendSpan("d", 0);
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
        if (value is null) {
            AppendSpan(ReadOnlySpan<char>.Empty, alignment);
            return;
        }

        if (value is ConsoleColor consoleColor) {
            AppendFormatted(consoleColor);
            return;
        }

        if (value is string str) {
            AppendString(str, alignment);
            return;
        }

        if (value is ISpanFormattable spanFormattable) {
            AppendSpanFormattable(spanFormattable, alignment, null);
            return;
        }

        if (value is IFormattable formattable) {
            AppendString(formattable.ToString(format, _provider), alignment);
            return;
        }

        AppendString(value.ToString(), alignment);
    }

    private readonly void AppendSpanFormattable<T>(T value, int alignment, string? format)
    where T : ISpanFormattable {
        using var owner = BufferPool.Shared.Rent(out var buffer);
        int upperBound = BufferPool.ListStartingSize;
        var formatSpan = format is null ? ReadOnlySpan<char>.Empty : format.AsSpan();

        while (true) {
            buffer.EnsureCapacity(upperBound);
            CollectionsMarshal.SetCount(buffer, upperBound);
            var span = CollectionsMarshal.AsSpan(buffer);
            if (value.TryFormat(span, out int charsWritten, formatSpan, _provider)) {
                AppendSpan(span.Slice(0, charsWritten), alignment);
                break;
            }

            upperBound *= 2;
        }
    }

    private readonly void AppendString(string? value, int alignment) {
        if (string.IsNullOrEmpty(value)) {
            AppendSpan(ReadOnlySpan<char>.Empty, alignment);
            return;
        }

        AppendSpan(value.AsSpan(), alignment);
    }

    private readonly void AppendSpan(scoped ReadOnlySpan<char> span, int alignment) {
        if (alignment != 0) {
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
            return;
        }

        if (!span.IsEmpty) {
            _writer.Write(span);
        }
    }

    private readonly void WritePadding(int count) {
        if (count <= 0) {
            return;
        }

        _writer.WriteWhiteSpaces(count);
    }
}
#pragma warning restore CA1822 // Mark members as static