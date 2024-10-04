using System.Buffers;

namespace PrettyConsole;

/// <summary>
/// A static class containing utility methods
/// </summary>
public static class Utils {
    /// <summary>
    /// Returns an elapsed time formatted string, i.e 0.##s or 0.##m or 0.##h
    /// </summary>
    /// <param name="elapsed">The elapsed TimeSpan</param>
    /// <param name="buffer">The buffer to use - ensure sufficient capacity (</param>
    /// <returns></returns>
    public static ReadOnlySpan<char> FormatElapsedTime(TimeSpan elapsed, Span<char> buffer) {
        int bytesWritten;
        if (elapsed.TotalSeconds < 60) {
            elapsed.TotalSeconds.TryFormat(buffer, out bytesWritten, "0.##s");
            return buffer.Slice(0, bytesWritten);
        }
        if (elapsed.Minutes < 60) {
            elapsed.TotalMinutes.TryFormat(buffer, out bytesWritten, "0.##m");
            return buffer.Slice(0, bytesWritten);
        }
        elapsed.TotalHours.TryFormat(buffer, out bytesWritten, "0.##h");
        return buffer.Slice(0, bytesWritten);
    }

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
