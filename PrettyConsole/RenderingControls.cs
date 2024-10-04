using System.Runtime.CompilerServices;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Clears the next <paramref name="lines"/>
    /// </summary>
    /// <param name="lines">Amount of lines to clear</param>
    /// <remarks>
    /// Useful for clearing output of overriding functions, like the ProgressBar
    /// </remarks>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void ClearNextLines(int lines) {
        ResetColors();
        using var memoryOwner = Utils.ObtainMemory(baseConsole.BufferWidth);
        Span<char> emptyLine = memoryOwner.Memory.Span.Slice(0, baseConsole.BufferWidth);
        emptyLine.Fill(' ');
        var currentLine = baseConsole.CursorTop;
        baseConsole.SetCursorPosition(0, currentLine);
        for (int i = 0; i < lines; i++) {
            Out.WriteLine(emptyLine);
        }
        baseConsole.SetCursorPosition(0, currentLine);
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
    /// Reset the colors of the console output
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ResetColors() => baseConsole.ResetColor();
}