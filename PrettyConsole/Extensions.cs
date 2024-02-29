namespace PrettyConsole;

internal static class Extensions {
    private static readonly char[] TimeSpanBuffer = GC.AllocateUninitializedArray<char>(20);

    // Returns a elapsed time representation
    internal static ReadOnlySpan<char> FormattedElapsedTime(this TimeSpan elapsed) {
        int bytesWritten;
        if (elapsed.TotalSeconds < 60) {
            elapsed.TotalSeconds.TryFormat(TimeSpanBuffer, out bytesWritten, "[Elapsed: 0.##s]");
            return TimeSpanBuffer.AsSpan(0, bytesWritten);
        }
        if (elapsed.Minutes < 60) {
            elapsed.TotalMinutes.TryFormat(TimeSpanBuffer, out bytesWritten, "[Elapsed: 0.##m]");
            return TimeSpanBuffer.AsSpan(0, bytesWritten);
        }
        elapsed.TotalHours.TryFormat(TimeSpanBuffer, out bytesWritten, "[Elapsed: 0.##h]");
        return TimeSpanBuffer.AsSpan(0, bytesWritten);
    }

    private static readonly char[] PercentageBuffer = GC.AllocateUninitializedArray<char>(6);

    // Returns a percentage representation
    internal static ReadOnlySpan<char> FormattedPercentage(this double percentage) {
        percentage.TryFormat(PercentageBuffer, out int bytesWritten, "0,5:##0.##%");
        return PercentageBuffer.AsSpan(0, bytesWritten);
    }
}
