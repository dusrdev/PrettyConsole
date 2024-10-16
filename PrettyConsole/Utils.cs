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
        builder = StringBuffer.Create(buffer);
        while (padding-- > 0) {
            builder.Append(' ');
        }
        builder.Append(rounded);
        return builder.WrittenSpan;
    }

    /// <summary>
    /// Rents a memory owner from the shared memory pool
    /// </summary>
    /// <param name="length">The minimum length</param>
    internal static IMemoryOwner<char> ObtainMemory(int length) => MemoryPool<char>.Shared.Rent(length);
}
