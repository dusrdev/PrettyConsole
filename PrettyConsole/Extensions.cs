using System.Runtime.CompilerServices;

namespace PrettyConsole;

internal static class Extensions {
    // Returns a elapsed time representation
    internal static ReadOnlySpan<char> FormattedElapsedTime(this TimeSpan elapsed, Span<char> buffer) {
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
    internal static ReadOnlySpan<char> FormattedPercentage(this double percentage, Span<char> buffer) {
        percentage.TryFormat(buffer, out int bytesWritten, "0,5:##0.##%");
        return buffer.Slice(0, bytesWritten);
    }
    
    // Directly writes the span to the TextWriter
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void WriteDirect(this TextWriter writer, scoped ReadOnlySpan<char> span) {
        foreach (char c in span) {
            writer.Write(c);
        }
    }
}
