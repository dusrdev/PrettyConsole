using System.Buffers;

namespace PrettyConsole;

/// <summary>
/// A static class containing utility methods
/// </summary>
internal static class Utils {
    /// <summary>
    /// Returns a formatted percentage string, i.e 0,5:##0.##%
    /// </summary>
    /// <param name="percentage"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    internal static ReadOnlySpan<char> FormatPercentage(double percentage, Span<char> buffer) {
        const int length = 5;
        percentage = Math.Round(Math.Clamp(percentage, 0, 100), 2, MidpointRounding.AwayFromZero);

        percentage.TryFormat(buffer, out int written);
        if (written == length) {
            return buffer.Slice(0, written);
        }

        var padding = length - written;
        buffer.Slice(0, padding).Fill(' ');
        percentage.TryFormat(buffer.Slice(padding), out written);

        return buffer.Slice(0, padding + written);
    }

    /// <summary>
    /// Formats <paramref name="timeSpan"/>
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="buffer"></param>
    /// <returns>The number of characters written to <paramref name="buffer"/></returns>
    internal static int FormatTimeSpan(TimeSpan timeSpan, Span<char> buffer) {
        // < 1s  → "500ms"
        int written;

        if (timeSpan.TotalSeconds < 1) {
            if (!timeSpan.Milliseconds.TryFormat(buffer, out written)) {
                return 0;
            }
            "ms".CopyTo(buffer.Slice(written));
            return written + 2;
        }

        // < 60s → "SS:MMMs" (zero-padded)
        if (timeSpan.TotalSeconds < 60) {
            if (!buffer.TryWrite($"{timeSpan.Seconds:00}:{timeSpan.Milliseconds:000}s", out written)) {
                return 0;
            }
            return written;
        }

        // < 1h  → "MM:SSm"
        if (timeSpan.TotalSeconds < 3600) {
            if (!buffer.TryWrite($"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}m", out written)) {
                return 0;
            }
            return written;
        }

        // < 1d  → "HH:MMhr"
        if (timeSpan.TotalSeconds < 86400) {
            if (!buffer.TryWrite($"{timeSpan.Hours:00}:{timeSpan.Minutes:00}hr", out written)) {
                return 0;
            }
            return written;
        }

        // ≥ 1d  → "DD:HHd"
        if (!buffer.TryWrite($"{timeSpan.Days:00}:{timeSpan.Hours:00}d", out written)) {
            return 0;
        }
        return written;
    }

    /// <summary>
    /// Constant buffer filled with whitespaces
    /// </summary>
    private static readonly string WhiteSpaces = new(' ', 256);

    /// <summary>
    /// Writes whitespace to a <see cref="TextWriter"/> up to length by chucks
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="length"></param>
    internal static void WriteWhiteSpaces(this TextWriter writer, int length) {
        if (length <= 0) {
            return;
        }

        // Fast path: single call when length fits in the buffer
        if (length <= WhiteSpaces.Length) {
            writer.Write(WhiteSpaces.AsSpan(0, length));
            return;
        }

        // Write full chunks
        var full = WhiteSpaces;
        while (length >= full.Length) {
            writer.Write(full); // writes all 256 in one call
            length -= full.Length;
        }

        // Write the remainder
        if (length > 0) {
            writer.Write(full.AsSpan(0, length));
        }
    }

    /// <summary>
    /// Rents a memory owner from the shared memory pool
    /// </summary>
    /// <param name="length">The minimum length</param>
    internal static IMemoryOwner<char> ObtainMemory(int length) => MemoryPool<char>.Shared.Rent(length);
}
