using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PrettyConsole;

#pragma warning disable CA1822 // Mark members as static
/// <summary>
/// Interpolated string handler that streams segments directly to an <see cref="OutputPipe"/> while allowing inline color changes.
/// </summary>
[InterpolatedStringHandler]
public readonly ref struct PrettyConsoleInterpolatedStringHandler {
    private readonly OutputPipe _pipe;
    private readonly TextWriter _writer;
    private readonly IFormatProvider? _provider;

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
        _pipe = pipe;
        _writer = Console.GetWriter(pipe);
        _provider = provider;
        shouldAppend = true;
    }

    /// <summary>
    /// Appends a literal segment supplied by the compiler.
    /// </summary>
    public void AppendLiteral(string value) {
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
    public void AppendFormatted(string? value, int alignment = 0, string? format = null) {
        AppendString(value, alignment);
    }

    /// <summary>
    /// Appends a span segment without allocations.
    /// </summary>
    /// <param name="value">Characters to write.</param>
    /// <param name="alignment">Optional alignment as provided by the interpolation.</param>
    public void AppendFormatted(scoped ReadOnlySpan<char> value, int alignment = 0) {
        AppendSpan(value, alignment);
    }

    /// <summary>
    /// Appends a single character.
    /// </summary>
    /// <param name="value">Character to write.</param>
    /// <param name="alignment">Optional alignment as provided by the interpolation.</param>
    public void AppendFormatted(char value, int alignment = 0) {
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
    public void AppendFormatted(ConsoleColor color) {
        // _ = _pipe;
        Console.SetColors(color, baseConsole.BackgroundColor);
    }

    /// <summary>
    /// Sets the console foreground color to <paramref name="color"/>.
    /// </summary>
    public void AppendFormatted(Color color) {
        // _ = _pipe;
        Console.SetColors(color, baseConsole.BackgroundColor);
    }

    /// <summary>
	/// Sets the foreground and background colors of the console
	/// </summary>
	/// <param name="colors"></param>
    public void AppendFormatted((ConsoleColor foreground, ConsoleColor background) colors) {
        // _ = _pipe;
        Console.SetColors(colors.foreground, colors.background);
    }

    /// <summary>
    /// Writes a <see cref="ColoredOutput"/> segment and applies its colors for the duration of the write.
    /// </summary>
    /// <param name="output">Segment to write.</param>
    /// <param name="alignment">Optional alignment as provided by the interpolation.</param>
    public void AppendFormatted(ColoredOutput output, int alignment = 0) {
        Console.Write(output, _pipe);
        if (alignment != 0) {
            AppendSpan(ReadOnlySpan<char>.Empty, alignment);
        }
    }

    /// <summary>
    /// Writes a buffer of <see cref="ColoredOutput"/> items.
    /// </summary>
    /// <param name="outputs">Segments to write.</param>
    public void AppendFormatted(ReadOnlySpan<ColoredOutput> outputs) {
        if (outputs.Length is 0) {
            return;
        }
        Console.Write(outputs, _pipe);
    }

    /// <summary>
    /// Appends a value type that implements <see cref="ISpanFormattable"/> without boxing.
    /// </summary>
    public void AppendFormatted<T>(T value) where T : ISpanFormattable {
        AppendSpanFormattable(value, alignment: 0, format: null);
    }

    /// <summary>
    /// Appends a value type that implements <see cref="ISpanFormattable"/> without boxing while respecting alignment.
    /// </summary>
    public void AppendFormatted<T>(T value, int alignment) where T : ISpanFormattable {
        AppendSpanFormattable(value, alignment, format: null);
    }

    /// <summary>
    /// Appends a value type that implements <see cref="ISpanFormattable"/> without boxing using the provided format string.
    /// </summary>
    public void AppendFormatted<T>(T value, string? format) where T : ISpanFormattable {
        AppendSpanFormattable(value, alignment: 0, format);
    }

    /// <summary>
    /// Appends a value type that implements <see cref="ISpanFormattable"/> without boxing using alignment and format string.
    /// </summary>
    public void AppendFormatted<T>(T value, int alignment, string? format) where T : ISpanFormattable {
        AppendSpanFormattable(value, alignment, format);
    }

    /// <summary>
    /// Appends an object value when the compiler cannot resolve a more specific overload.
    /// </summary>
    /// <param name="value">Value to write.</param>
    /// <param name="alignment">Optional alignment as provided by the interpolation.</param>
    /// <param name="format">Optional format specifier.</param>
    public void AppendFormatted(object? value, int alignment = 0, string? format = null) {
        if (value is null) {
            AppendSpan(ReadOnlySpan<char>.Empty, alignment);
            return;
        }

        if (value is ConsoleColor consoleColor) {
            AppendFormatted(consoleColor);
            return;
        }

        if (value is Color color) {
            AppendFormatted(color);
            return;
        }

        if (value is ColoredOutput coloredOutput) {
            AppendFormatted(coloredOutput, alignment);
            return;
        }

        if (value is string str) {
            AppendString(str, alignment);
            return;
        }

        if (value is IFormattable formattable) {
            AppendString(formattable.ToString(format, _provider), alignment);
            return;
        }

        AppendString(value.ToString(), alignment);
    }

    private void AppendSpanFormattable<T>(T value, int alignment, string? format)
    where T : ISpanFormattable {
        using var owner = BufferPool.Shared.Rent();
        var buffer = owner.Buffer;
        int upperBound = BufferPool.ListStartingSize;
        var formatSpan = format is null ? ReadOnlySpan<char>.Empty : format.AsSpan();

        while (true) {
            buffer.EnsureCapacity(upperBound);
            CollectionsMarshal.SetCount(buffer, upperBound);
            var span = CollectionsMarshal.AsSpan(buffer);
            if (value.TryFormat(span, out int charsWritten, formatSpan, _provider)) {
                AppendSpan(span[..charsWritten], alignment);
                break;
            }

            upperBound *= 2;
        }
    }

    private void AppendString(string? value, int alignment) {
        if (string.IsNullOrEmpty(value)) {
            AppendSpan(ReadOnlySpan<char>.Empty, alignment);
            return;
        }

        AppendSpan(value.AsSpan(), alignment);
    }

    private void AppendSpan(scoped ReadOnlySpan<char> span, int alignment) {
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

    private void WritePadding(int count) {
        if (count <= 0) {
            return;
        }

        Span<char> spaces = stackalloc char[Math.Min(count, 32)];
        spaces.Fill(' ');
        while (count > 0) {
            int segmentLength = Math.Min(count, spaces.Length);
            _writer.Write(spaces[..segmentLength]);
            count -= segmentLength;
        }
    }
}
#pragma warning restore CA1822 // Mark members as static
