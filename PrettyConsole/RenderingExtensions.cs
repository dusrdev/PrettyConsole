namespace PrettyConsole;

/// <summary>
/// Provides methods extending <see cref="Console"/> with more rendering methods.
/// </summary>
public static class RenderingExtensions {
    private static readonly Func<int> DefaultCursorTopAccessor = static () => Console.CursorTop;
    private static readonly Action<int, int> DefaultSetCursorPosition = Console.SetCursorPosition;

    private static Func<int> s_cursorTopAccessor = DefaultCursorTopAccessor;
    private static Action<int, int> s_setCursorPosition = DefaultSetCursorPosition;

    /// <summary>
    /// Allows tests to override how cursor information is retrieved.
    /// </summary>
    /// <param name="cursorTopAccessor">Delegate that returns the current cursor row.</param>
    /// <param name="setCursorPosition">Delegate that positions the cursor.</param>
    internal static void ConfigureCursorAccessors(Func<int>? cursorTopAccessor, Action<int, int>? setCursorPosition) {
        s_cursorTopAccessor = cursorTopAccessor ?? DefaultCursorTopAccessor;
        s_setCursorPosition = setCursorPosition ?? DefaultSetCursorPosition;
    }

    extension(Console) {
        /// <summary>
		/// Write white spaces to <paramref name="pipe"/>
		/// </summary>
		/// <param name="length"></param>
		/// <param name="pipe"></param>
		public static void WriteWhiteSpaces(int length, OutputPipe pipe = OutputPipe.Out) {
            ConsoleContext.GetPipeTarget(pipe).WriteWhiteSpaces(length);
        }

        /// <summary>
        /// Clears the next <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Number of lines to clear.</param>
        /// <param name="pipe">The output pipe to use</param>
        /// <remarks>
        /// Useful for clearing output from overwriting functions like the progress bar.
        /// </remarks>
        public static void ClearNextLines(int lines, OutputPipe pipe = OutputPipe.Error) {
            var textWriter = ConsoleContext.GetPipeTarget(pipe);
            var lineLength = ConsoleContext.GetWidthOrDefault();
            var currentLine = GetCurrentLine();
            GoToLine(currentLine);
            for (int i = 0; i < lines; i++) {
                textWriter.WriteWhiteSpaces(lineLength);
            }
            GoToLine(currentLine);
        }

        /// <summary>
        /// Moves the cursor forward by <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Number of lines to skip.</param>
        /// <remarks>
        /// Useful for keeping overwritten lines, like progress bar or dashboards after the task is done.
        /// </remarks>
        public static void SkipLines(int lines) {
            var currentLine = GetCurrentLine();
            GoToLine(currentLine + lines);
        }

        /// <summary>
        /// Used to end current line or write an empty one, depends whether the current line has any text.
        /// </summary>
        public static void NewLine(OutputPipe pipe = OutputPipe.Out) {
            ConsoleContext.GetPipeTarget(pipe).WriteLine();
        }

        /// <summary>
        /// Sets the colors of the console output.
        /// </summary>
        public static void SetColors(ConsoleColor foreground, ConsoleColor background) {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentLine() {
            return s_cursorTopAccessor();
        }

        /// <summary>
        /// Moves the cursor to the start of the specified line.
        /// </summary>
        /// <param name="line"></param>
        public static void GoToLine(int line) {
            s_setCursorPosition(0, line);
        }
    }
}