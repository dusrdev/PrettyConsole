using System.Buffers;

namespace PrettyConsole;

//TODO: Check if optional values for alignment can reduce overloads

/// <summary>
/// Interpolated string handler that handles formatting
/// </summary>
[InterpolatedStringHandler]
public struct PrettyConsoleInterpolatedStringHandler {
    private static readonly ArrayPool<char> BufferPool = ArrayPool<char>.Shared;

    private bool _flushed;

    private char[] _buffer;

    private int _index;

    private int _capacity = 4096;

    private readonly TextWriter _writer;
    private readonly bool _isRedirected;
    private readonly IFormatProvider? _provider;

    private ConsoleColor _currentForeground;
    private ConsoleColor _currentBackground;

    /// <summary>
	/// The number of characters written in this instance of <see cref="PrettyConsoleInterpolatedStringHandler"/>.
	/// </summary>
    public int CharsWritten { get; private set; }

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
        _buffer = BufferPool.Rent(_capacity);
        _currentForeground = ConsoleColor.DefaultForeground;
        _currentBackground = ConsoleColor.DefaultBackground;
        (_writer, _isRedirected) = ConsoleContext.GetPipeTargetAndState(pipe);
        _provider = provider;
        shouldAppend = true;
    }

    /// <summary>
    /// Appends a literal segment supplied by the compiler.
    /// </summary>
    public void AppendLiteral(string value) {
        ThrowIfFlushed();
        AppendSpanCore(value);
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
    /// Appends a span segment
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
        Span<char> buffer = [value];
        AppendSpan(buffer, alignment);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ChangeForeground(ConsoleColor foreground) => AppendSpanCore(AnsiColors.Foreground(foreground));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ChangeBackground(ConsoleColor background) => AppendSpanCore(AnsiColors.Background(background));

    /// <summary>
    /// Sets the foreground color to <paramref name="color"/>.
    /// </summary>
    public void AppendFormatted(ConsoleColor color) {
        if (_isRedirected) return;
        if (_currentForeground != color) {
            ThrowIfFlushed();
            _currentForeground = color;
            ChangeForeground(color);
        }
    }

    /// <summary>
    /// Sets the background color to <paramref name="color"/>.
    /// </summary>
    public void AppendFormattedBackground(ConsoleColor color) {
        if (_isRedirected) return;
        if (_currentBackground != color) {
            ThrowIfFlushed();
            _currentBackground = color;
            ChangeBackground(color);
        }
    }

    /// <summary>
    /// Sets the foreground and background colors of the console
    /// </summary>
    /// <param name="colors"></param>
    public void AppendFormatted((ConsoleColor Foreground, ConsoleColor Background) colors) {
        AppendFormatted(colors.Foreground);
        AppendFormattedBackground(colors.Background);
    }

    /// <summary>
    /// Append timeSpan with optional formatting.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="format"></param>
    public void AppendFormatted(TimeSpan timeSpan, string? format = null)
        => AppendFormatted(timeSpan, alignment: 0, format);

    /// <summary>
    /// Append timeSpan with optional alignment support and formatting.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="alignment"></param>
    /// <param name="format"></param>
    public void AppendFormatted(TimeSpan timeSpan, int alignment, string? format = null) {
        if (format != "duration") {
            AppendSpanFormattable(timeSpan, alignment, format);
            return;
        }

        ThrowIfFlushed();

        const int requiredLength = 32;

        EnsureCapacity(requiredLength);

        Span<char> dest = _buffer.AsSpan(_index);

        if (dest.TryWrite($"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m {timeSpan.Seconds}s", out int written)) {
            _index += written;
            CharsWritten += written;
        }
    }

    private static ReadOnlySpan<string> FileSizeSuffix => new[] { "B", "KB", "MB", "GB", "TB", "PB" };

    /// <summary>
    /// Append double with optional formatting.
    /// </summary>
    /// <param name="num"></param>
    /// <param name="format"></param>
    public void AppendFormatted(double num, string? format = null)
        => AppendFormatted(num, alignment: 0, format);

    /// <summary>
    /// Append double with optional alignment and formatting.
    /// </summary>
    /// <param name="num"></param>
    /// <param name="alignment"></param>
    /// <param name="format"></param>
    public void AppendFormatted(double num, int alignment, string? format = null) {
        if (format != "bytes") {
            AppendSpanFormattable(num, alignment, format);
            return;
        }

        ThrowIfFlushed();

        const double formatBytesKb = 1024d;
        const double formatBytesDivisor = 1 / formatBytesKb;
        var suffix = 0;
        while (suffix < FileSizeSuffix.Length - 1 && num >= formatBytesKb) {
            num *= formatBytesDivisor;
            suffix++;
        }
        var unit = FileSizeSuffix[suffix];

        const double defaultThreshold = 1e90;

        int requiredLength = num <= defaultThreshold ? 128 : 512;

        EnsureCapacity(requiredLength);

        Span<char> dest = _buffer.AsSpan(_index);

        if (dest.TryWrite($"{num:#,##0.##} {unit}", out int written)) {
            _index += written;
            CharsWritten += written;
        }
    }

    /// <summary>
    /// Appends a value type that implements <see cref="ISpanFormattable"/> without boxing while respecting alignment.
    /// </summary>
    public void AppendFormatted<T>(T value, int alignment = 0) where T : ISpanFormattable {
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

    private void AppendSpanFormattable<T>(T value, int alignment, string? format)
    where T : ISpanFormattable {
        ThrowIfFlushed();
        ReadOnlySpan<char> formatSpan = format.AsSpan();

        int charsWritten;
        int start = _index;

        while (true) {
            Span<char> dest = _buffer.AsSpan(_index);

            if (value.TryFormat(dest, out charsWritten, formatSpan, _provider)) {
                _index += charsWritten;
                CharsWritten += charsWritten;
                break;
            }

            Grow(_capacity * 2);
        }

        if (alignment == 0) return;

        if (alignment > 0) { // shift forward and prefix whitespaces
            int padding = alignment - charsWritten;
            if (padding <= 0) return;

            EnsureCapacity(padding);
            var written = _buffer.AsSpan(start, charsWritten);
            written.CopyTo(_buffer.AsSpan(start + padding, charsWritten));
            _buffer.AsSpan(start, padding).Fill(' ');
            _index += padding;
            CharsWritten += padding;
        } else { // suffix whitespaces
            int targetWidth = -alignment;
            int trailing = targetWidth - charsWritten;
            if (trailing > 0) {
                WritePadding(trailing);
                CharsWritten += trailing;
            }
        }
    }

    private void AppendString(string? value, int alignment) {
        // AppendSpan handles null and empty spans
        AppendSpan(value.AsSpan(), alignment);
    }

    private void AppendSpan(scoped ReadOnlySpan<char> span, int alignment) {
        ThrowIfFlushed();

        if (alignment == 0) {
            AppendSpanCore(span);
            return;
        }

        bool leftAlign = alignment < 0;
        int width = Math.Abs(alignment);
        int visibleLength = span.Length > 0 && span[0] == '\e' ? 0 : span.Length;
        int padding = width - visibleLength;
        int required = span.Length + padding;
        EnsureCapacity(required);

        if (padding > 0 && !leftAlign) {
            WritePadding(padding);
            CharsWritten += padding;
        }

        AppendSpanCore(span, false);

        if (padding > 0 && leftAlign) {
            WritePadding(padding);
            CharsWritten += padding;
        }
    }

    private void AppendSpanCore(scoped ReadOnlySpan<char> span, bool ensureCapacity = true) {
        int length = span.Length;

        if (length == 0) return;

        bool isEscapeSequence = span[0] == '\e';

        if (isEscapeSequence && _isRedirected) return;

        if (ensureCapacity) EnsureCapacity(length);
        span.CopyTo(_buffer.AsSpan(_index, length));
        _index += length;

        if (!isEscapeSequence) CharsWritten += length;
    }

    private void WritePadding(int count) {
        _buffer.AsSpan(_index, count).Fill(' ');
        _index += count;
    }

    /// <summary>
	/// Writes a new line to the internal buffer.
	/// </summary>
    public void AppendNewLine() {
        ThrowIfFlushed();
        string newline = Environment.NewLine;
        AppendSpanCore(newline);
        CharsWritten -= newline.Length;
    }

    private void EnsureCapacity(int capacity) {
        int available = _buffer.Length - _index;
        if (capacity <= available) return;

        int required = _index + capacity;
        int targetCapacity = _capacity;
        while (targetCapacity < required) {
            targetCapacity *= 2;
        }

        Grow(targetCapacity);
    }

    private void Grow(int targetCapacity) {
        char[] temp = _buffer;

        _capacity = targetCapacity;
        _buffer = BufferPool.Rent(_capacity);
        Span<char> written = temp.AsSpan(0, _index);
        written.CopyTo(_buffer);
        written.Clear();
        BufferPool.Return(temp, false);
    }

    private readonly void ThrowIfFlushed() {
        if (!_flushed) return;

        throw new InvalidOperationException("The handler was consumed and its buffer have been freed.");
    }

    /// <summary>
	/// Writes the underline buffer to the held <see cref="TextWriter"/>.
	/// </summary>
    public void Flush() {
        ThrowIfFlushed();
        AppendFormatted(ConsoleColor.DefaultForeground);
        AppendFormattedBackground(ConsoleColor.DefaultBackground);
        Span<char> written = new(_buffer, 0, _index);
        _writer.Write(written);
        written.Clear();
        BufferPool.Return(_buffer, false);
        _flushed = true;
    }
}
