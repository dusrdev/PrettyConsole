namespace PrettyConsole;

internal static class Extensions {
	private static readonly char[] TimeSpanBuffer = GC.AllocateUninitializedArray<char>(10);

    // Returns a elapsed time representation
    internal static string ToFriendlyString(this TimeSpan elapsed) {
        if (elapsed.TotalSeconds < 60) {
            return string.Create(null, TimeSpanBuffer, $"{elapsed.TotalSeconds:0.##}s");
        }
        if (elapsed.Minutes < 60) {
            return string.Create(null, TimeSpanBuffer, $"{elapsed.TotalMinutes:0.##}m");
        }
        return string.Create(null, TimeSpanBuffer, $"{elapsed.TotalHours:0.###}h");
    }
}
