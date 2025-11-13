namespace PrettyConsole;

/// <summary>
/// Provides methods extending <see cref="Console"/> with more rendering methods.
/// </summary>
public static partial class RenderingExtensions {
    extension(Console) {
        /// <summary>
        /// Clears the next <paramref name="lines"/> (regular output)
        /// </summary>
        /// <param name="lines">Amount of lines to clear</param>
        /// <param name="pipe">The output pipe to use</param>
        /// <remarks>
        /// Useful for clearing output of overriding functions, like the ProgressBar
        /// </remarks>
        public static void ClearNextLines(int lines, OutputPipe pipe = OutputPipe.Error) {
            var textWriter = PrettyConsoleExtensions.GetWriter(pipe);
            var lineLength = PrettyConsoleExtensions.GetWidthOrDefault();
            var currentLine = GetCurrentLine();
            GoToLine(currentLine);
            for (int i = 0; i < lines; i++) {
                textWriter.WriteWhiteSpaces(lineLength);
            }
            GoToLine(currentLine);
        }

        /// <summary>
        /// Used to end current line or write an empty one, depends whether the current line has any text
        /// </summary>
        public static void NewLine(OutputPipe pipe = OutputPipe.Out) {
            PrettyConsoleExtensions.GetWriter(pipe).WriteLine();
        }

        /// <summary>
        /// Sets the colors of the console output
        /// </summary>
        public static void SetColors(ConsoleColor foreground, ConsoleColor background) {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        /// <summary>
        /// Gets the current line number
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentLine() {
            return Console.CursorTop;
        }

        /// <summary>
        /// Moves the cursor to the specified line
        /// </summary>
        /// <param name="line"></param>
        public static void GoToLine(int line) {
            Console.SetCursorPosition(0, line);
        }
    }
}