using System.Diagnostics.Contracts;

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
    [Pure]
    public static void ClearNextLines(int lines) {
        ogConsole.ResetColor();
        var currentLine = ogConsole.CursorTop;
        ogConsole.SetCursorPosition(0, currentLine);
        var emptyLines = new string(' ', lines * ogConsole.BufferWidth);
        ogConsole.Write(emptyLines);
        ogConsole.SetCursorPosition(0, currentLine);
    }

    /// <summary>
    /// Used to clear all previous outputs to the console
    /// </summary>
    public static void Clear() {
        ogConsole.Clear();
    }

    /// <summary>
    /// Used to end current line or write an empty one, depends whether the current line has any text
    /// </summary>
    public static void NewLine() {
        ogConsole.WriteLine();
    }
}