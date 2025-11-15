using System.Buffers;

namespace PrettyConsole;

/// <summary>
/// Interpolated string handler that streams segments directly to an <see cref="OutputPipe"/> while allowing inline color changes.
/// </summary>
[InterpolatedStringHandler]
public struct PrettyConsoleInterpolatedStringHandler {
    private readonly TextWriter _writer;
    private readonly IFormatProvider? _provider;
    private static readonly Action<TextWriter, ConsoleColor> ChangeFg;
    private static readonly Action<TextWriter, ConsoleColor> ChangeBg;
    private ConsoleColor _currentForeground;
    private ConsoleColor _currentBackground;

    static PrettyConsoleInterpolatedStringHandler() {
        if (AnsiColors.Enabled) {
            ChangeFg = static (writer, color) => writer.Write(AnsiColors.Foreground(color));
            ChangeBg = static (writer, color) => writer.Write(AnsiColors.Background(color));
        } else {
            ChangeFg = static (_, color) => Console.ForegroundColor = color;
            ChangeBg = static (_, color) => Console.BackgroundColor = color;
        }
    }

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
        _currentForeground = ConsoleColor.DefaultForeground;
        _currentBackground = ConsoleColor.DefaultBackground;
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
    public void AppendFormatted(ConsoleColor color) {
        if (_currentForeground != color) {
            _currentForeground = color;
            ChangeFg(_writer, _currentForeground);
        }
    }

    /// <summary>
    /// Sets the foreground and background colors of the console
    /// </summary>
    /// <param name="colors"></param>
    public void AppendFormatted((ConsoleColor Foreground, ConsoleColor Background) colors) {
        if (_currentForeground != colors.Foreground) {
            _currentForeground = colors.Foreground;
            ChangeFg(_writer, _currentForeground);
        }
        if (_currentBackground != colors.Background) {
            _currentBackground = colors.Background;
            ChangeBg(_writer, _currentBackground);
        }
    }

    /// <summary>
    /// Sets the foreground and background colors of the console
    /// </summary>
    /// <param name="colors"></param>
    /// <param name="alignment"></param>
    public void AppendFormatted((ConsoleColor Foreground, ConsoleColor Background) colors, int alignment) {
        AppendFormatted(colors);
        AppendSpan(ReadOnlySpan<char>.Empty, alignment);
    }

    /// <summary>
    /// Append timeSpan with optional formatting.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="format"></param>
    public readonly void AppendFormatted(TimeSpan timeSpan, string? format = null)
        => AppendFormatted(timeSpan, alignment: 0, format);

    /// <summary>
    /// Append timeSpan with optional alignment support and formatting.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="alignment"></param>
    /// <param name="format"></param>
    public readonly void AppendFormatted(TimeSpan timeSpan, int alignment, string? format = null) {
        if (format != "duration") {
            AppendSpanFormattable(timeSpan, alignment, format);
            return;
        }

        Span<char> buffer = stackalloc char[32];
        if (buffer.TryWrite($"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m {timeSpan.Seconds}s", out int written)) {
            AppendSpan(buffer.Slice(0, written), alignment);
        }
    }

    private static readonly string[] FileSizeSuffix = ["B", "KB", "MB", "GB", "TB", "PB"];

    /// <summary>
    /// Append double with optional formatting.
    /// </summary>
    /// <param name="num"></param>
    /// <param name="format"></param>
    public readonly void AppendFormatted(double num, string? format = null)
        => AppendFormatted(num, alignment: 0, format);

    /// <summary>
    /// Append double with optional alignment and formatting.
    /// </summary>
    /// <param name="num"></param>
    /// <param name="alignment"></param>
    /// <param name="format"></param>
    public readonly void AppendFormatted(double num, int alignment, string? format = null) {
        if (format != "bytes") {
            AppendSpanFormattable(num, alignment, format);
            return;
        }

        const double formatBytesKb = 1024d;
        const double formatBytesDivisor = 1 / formatBytesKb;
        var suffix = 0;
        while (suffix < FileSizeSuffix.Length - 1 && num >= formatBytesKb) {
            num *= formatBytesDivisor;
            suffix++;
        }
        var unit = FileSizeSuffix[suffix];

        const double defaultThreshold = 1e90;

        Span<char> buffer = num <= defaultThreshold
                ? stackalloc char[128]
                : stackalloc char[512];
        if (buffer.TryWrite($"{num:#,##0.##} {unit}", out int written)) {
            AppendSpan(buffer.Slice(0, written), alignment);
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
            try {
                buffer = new Span<char>(array);
                if (value.TryFormat(array, out charsWritten, formatSpan, _provider)) {
                    AppendSpan(buffer.Slice(0, charsWritten), alignment);
                    return;
                }
            } finally {
                pool.Return(array);
            }
            lowerBound *= 2;
        }
    }

    private readonly void AppendString(string? value, int alignment) {
        // AppendSpan handles null and empty spans
        AppendSpan(value.AsSpan(), alignment);
    }

    private readonly void AppendSpan(scoped ReadOnlySpan<char> span, int alignment) {
        if (alignment == 0) {
            if (!span.IsEmpty) {
                _writer.Write(span);
            }
            return;
        }

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

    private readonly void WritePadding(int count) {
        _writer.WriteWhiteSpaces(count);
    }

    /// <summary>
	/// Resets the console colors if they changed.
	/// </summary>
    public void ResetColors() {
        if (_currentForeground != ConsoleColor.DefaultForeground) {
            _currentForeground = ConsoleColor.DefaultForeground;
            ChangeFg(_writer, _currentForeground);
        }
        if (_currentBackground != ConsoleColor.DefaultBackground) {
            _currentBackground = ConsoleColor.DefaultBackground;
            ChangeBg(_writer, _currentBackground);
        }
    }

    /// <summary>
	/// Writes a new line to the <see cref="TextWriter"/> used internally.
	/// </summary>
    public readonly void AppendNewLine() => _writer.WriteLine();
}