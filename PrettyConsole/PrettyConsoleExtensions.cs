using System.Runtime.Versioning;

namespace PrettyConsole;

/// <summary>
/// The static class the provides the abstraction over <see cref="Console"/> and other extensions.
/// </summary>
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
public static partial class PrettyConsoleExtensions {
    extension(TextWriter @this) {
        /// <summary>
        /// Writes whitespace to this <see cref="TextWriter"/> up to length by chucks
        /// </summary>
        /// <param name="length"></param>
        public void WriteWhiteSpaces(int length) {
            ReadOnlySpan<char> whiteSpaces = WhiteSpaces;

            while (length > 0) {
                int curLength = Math.Min(length, 256);
                @this.Write(whiteSpaces.Slice(0, curLength));
                length -= curLength;
            }
        }
    }

    private static readonly string WhiteSpaces = new(' ', 256);
}