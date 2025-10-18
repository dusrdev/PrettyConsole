namespace PrettyConsole;

/// <summary>
/// A static class containing utility methods
/// </summary>
internal static class Utils {
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
}
