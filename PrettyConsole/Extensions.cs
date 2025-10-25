namespace PrettyConsole;

internal static class Extensions {
    private static readonly string WhiteSpaces = new(' ', 256);

    /// <summary>
    /// Writes whitespace to a <see cref="TextWriter"/> up to length by chucks
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="length"></param>
    internal static void WriteWhiteSpaces(this TextWriter writer, int length) {
        ReadOnlySpan<char> whiteSpaces = WhiteSpaces;

        while (length > 0) {
            int cur_length = Math.Min(length, 256);
            writer.Write(whiteSpaces.Slice(0, cur_length));
            length -= cur_length;
        }
    }
}