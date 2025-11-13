namespace PrettyConsole;

/// <summary>
/// Provides a set of convenient extensions
/// </summary>
public static class PrettyConsoleExtensions {
    extension(TextWriter @this) {
        /// <summary>
        /// Writes whitespace to this <see cref="TextWriter"/> up to length by chucks
        /// </summary>
        /// <param name="length"></param>
        public void WriteWhiteSpaces(int length) {
            ReadOnlySpan<char> whiteSpaces = WhiteSpaces;

            while (length > 0) {
                int cur_length = Math.Min(length, 256);
                @this.Write(whiteSpaces.Slice(0, cur_length));
                length -= cur_length;
            }
        }
    }

    private static readonly string WhiteSpaces = new(' ', 256);
}