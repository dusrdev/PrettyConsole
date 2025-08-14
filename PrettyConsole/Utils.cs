using System.Buffers;

using Sharpify.Collections;

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
        var rounded = Math.Round(percentage, 2);
        var builder = StringBuffer.Create(buffer);
        builder.Append(rounded);
        if (builder.Position is length) {
            return buffer.Slice(0, length);
        }
        var padding = length - builder.Position;
        builder.Reset();
        while (padding-- > 0) {
            builder.Append(' ');
        }
        builder.Append(rounded);
        return builder.WrittenSpan;
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
    internal static void WriteWhiteSpace(this TextWriter writer, int length) {
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
