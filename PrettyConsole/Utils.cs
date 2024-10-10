using System.Buffers;

namespace PrettyConsole;

/// <summary>
/// A static class containing utility methods
/// </summary>
public static class Utils {
    /// <summary>
    /// Returns a formatted percentage string, i.e 0,5:##0.##%
    /// </summary>
    /// <param name="percentage"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static ReadOnlySpan<char> FormatPercentage(double percentage, Span<char> buffer) {
        percentage.TryFormat(buffer, out int bytesWritten, "0,5:##0.##%");
        return buffer.Slice(0, bytesWritten);
    }

    /// <summary>
    /// Rents a memory owner from the shared memory pool
    /// </summary>
    /// <param name="length">The minimum length</param>
    public static IMemoryOwner<char> ObtainMemory(int length) => MemoryPool<char>.Shared.Rent(length);
}
