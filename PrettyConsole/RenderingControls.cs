using System.Buffers;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Clears the next <paramref name="lines"/>
    /// </summary>
    /// <param name="lines">Amount of lines to clear</param>
    /// <remarks>
    /// Useful for clearing output of overriding functions, like the ProgressBar
    /// </remarks>
    public static void ClearNextLines(int lines) {
        ResetColors();
        var array = ArrayPool<char>.Shared.Rent(ogConsole.BufferWidth);
        Span<char> emptyLine = array.AsSpan(0, ogConsole.BufferWidth);
        emptyLine.Fill(' ');
        var currentLine = ogConsole.CursorTop;
        ogConsole.SetCursorPosition(0, currentLine);
        for (int i = 0; i < lines; i++) {
            ogConsole.Out.WriteLine(emptyLine);
        }
        ogConsole.SetCursorPosition(0, currentLine);
        ArrayPool<char>.Shared.Return(array);
    }

    /// <summary>
    /// Used to clear all previous outputs to the console
    /// </summary>
    public static void Clear() => ogConsole.Clear();

    /// <summary>
    /// Used to end current line or write an empty one, depends whether the current line has any text
    /// </summary>
    public static void NewLine() => ogConsole.Out.WriteLine();
}