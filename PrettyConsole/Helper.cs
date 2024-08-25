using System.Buffers;

namespace PrettyConsole;

internal static class Helper {
    // Returns an elapsed time representation
    internal static ReadOnlySpan<char> FormatElapsedTime(TimeSpan elapsed, Span<char> buffer) {
        int bytesWritten;
        if (elapsed.TotalSeconds < 60) {
            elapsed.TotalSeconds.TryFormat(buffer, out bytesWritten, "[Elapsed: 0.##s]");
            return buffer.Slice(0, bytesWritten);
        }
        if (elapsed.Minutes < 60) {
            elapsed.TotalMinutes.TryFormat(buffer, out bytesWritten, "[Elapsed: 0.##m]");
            return buffer.Slice(0, bytesWritten);
        }
        elapsed.TotalHours.TryFormat(buffer, out bytesWritten, "[Elapsed: 0.##h]");
        return buffer.Slice(0, bytesWritten);
    }

    // Returns a percentage representation
    internal static ReadOnlySpan<char> FormatPercentage(double percentage, Span<char> buffer) {
        percentage.TryFormat(buffer, out int bytesWritten, "0,5:##0.##%");
        return buffer.Slice(0, bytesWritten);
    }

    // Rents a memory owner from the shared memory pool
    internal static IMemoryOwner<char> ObtainMemory(int length) => MemoryPool<char>.Shared.Rent(length);
}
