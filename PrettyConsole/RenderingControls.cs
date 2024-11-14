using System.Runtime.CompilerServices;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Clears the next <paramref name="lines"/> (regular output)
    /// </summary>
    /// <param name="lines">Amount of lines to clear</param>
    /// <remarks>
    /// Useful for clearing output of overriding functions, like the ProgressBar
    /// </remarks>
    public static void ClearNextLines(int lines) => ClearNextLines(lines, Out);

    /// <summary>
    /// Clears the next <paramref name="lines"/> (regular output)
    /// </summary>
    /// <param name="lines">Amount of lines to clear</param>
    /// <remarks>
    /// Useful for clearing output of overriding functions, like the ProgressBar
    /// </remarks>
    public static void ClearNextLinesError(int lines) => ClearNextLines(lines, Error);

    /// <summary>
    /// Clears the next <paramref name="lines"/>
    /// </summary>
    /// <param name="lines">Amount of lines to clear</param>
    /// <param name="writer"></param>
    /// <remarks>
    /// Useful for clearing output of overriding functions, like the ProgressBar
    /// </remarks>
    [MethodImpl(MethodImplOptions.Synchronized)]
    private static void ClearNextLines(int lines, TextWriter writer) {
        ResetColors();
        ReadOnlySpan<char> emptyLine = WhiteSpace.AsSpan(0, baseConsole.BufferWidth);
        var currentLine = GetCurrentLine();
        for (int i = 0; i < lines; i++) {
            writer.Write(emptyLine);
        }
        GoToLine(currentLine);
    }

    /// <summary>
    /// Used to clear all previous outputs to the console
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear() => baseConsole.Clear();

    /// <summary>
    /// Used to end current line or write an empty one, depends whether the current line has any text
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NewLine() => Out.WriteLine();

    /// <summary>
    /// Used to end current line or write an empty one, depends whether the current line has any text
    /// </summary>
    /// <remarks>
    /// This is the error console version of <see cref="NewLine"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NewLineError() => Error.WriteLine();

    /// <summary>
    /// Sets the colors of the console output
    /// </summary>
    public static void SetColors(ConsoleColor foreground, ConsoleColor background) {
        baseConsole.ForegroundColor = foreground;
        baseConsole.BackgroundColor = background;
    }

    /// <summary>
    /// Resets the colors of the console output
    /// </summary>
    public static void ResetColors() => baseConsole.ResetColor();

    /// <summary>
    /// Gets the current line number
    /// </summary>
    /// <returns></returns>
    public static int GetCurrentLine() => baseConsole.CursorTop;

    /// <summary>
    /// Moves the cursor to the specified line
    /// </summary>
    /// <param name="line"></param>
    public static void GoToLine(int line) => baseConsole.SetCursorPosition(0, line);
}