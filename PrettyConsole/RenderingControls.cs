namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Clears the next <paramref name="lines"/> (regular output)
    /// </summary>
    /// <param name="lines">Amount of lines to clear</param>
    /// <param name="pipe">The output pipe to use</param>
    /// <remarks>
    /// Useful for clearing output of overriding functions, like the ProgressBar
    /// </remarks>
    public static void ClearNextLines(int lines, OutputPipe pipe = OutputPipe.Error) {
        if (pipe == OutputPipe.Error) {
            InternalClearNextLines(lines, Error);
            return;
        }
        InternalClearNextLines(lines, Out);
        return;

        static void InternalClearNextLines(int lines, TextWriter writer) {
            ReadOnlySpan<char> emptyLine = WhiteSpace.AsSpan(0, baseConsole.BufferWidth);
            var currentLine = GetCurrentLine();
            for (int i = 0; i < lines; i++) {
                writer.Write(emptyLine);
            }
            GoToLine(currentLine);
        }
    }

    /// <summary>
    /// Used to clear all previous outputs to the console
    /// </summary>
    public static void Clear() {
        baseConsole.Clear();
    }

    /// <summary>
    /// Used to end current line or write an empty one, depends whether the current line has any text
    /// </summary>
    public static void NewLine(OutputPipe pipe = OutputPipe.Out) {
        if (pipe == OutputPipe.Out) {
            Out.WriteLine();
            return;
        }
        Error.WriteLine();
    }

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
    public static void ResetColors() {
        baseConsole.ResetColor();
    }

    /// <summary>
    /// Gets the current line number
    /// </summary>
    /// <returns></returns>
    public static int GetCurrentLine() {
        return baseConsole.CursorTop;
    }

    /// <summary>
    /// Moves the cursor to the specified line
    /// </summary>
    /// <param name="line"></param>
    public static void GoToLine(int line) {
        baseConsole.SetCursorPosition(0, line);
    }
}