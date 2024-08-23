using System.Runtime.CompilerServices;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Represents a color that was configured as the default of the shell
    /// </summary>
    public const ConsoleColor UnknownColor = (ConsoleColor)(-1);

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
        using var array = new RentedBuffer<char>(ogConsole.BufferWidth);
        Span<char> emptyLine = array.Array.AsSpan(0, ogConsole.BufferWidth);
        emptyLine.Fill(' ');
        var currentLine = ogConsole.CursorTop;
        ogConsole.SetCursorPosition(0, currentLine);
        for (int i = 0; i < lines; i++) {
            ogConsole.Out.WriteDirect(emptyLine);
            ogConsole.WriteLine();
        }
        ogConsole.SetCursorPosition(0, currentLine);
    }

    /// <summary>
    /// Used to clear all previous outputs to the console
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear() => ogConsole.Clear();

    /// <summary>
    /// Used to end current line or write an empty one, depends whether the current line has any text
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NewLine() => ogConsole.WriteLine();

    /// <summary>
    /// Reset the colors of the console output
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ResetColors() => ogConsole.ResetColor();
}