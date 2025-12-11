namespace PrettyConsole;

/// <summary>
/// Provides methods extending <see cref="Console"/> with advanced output extensions.
/// </summary>
public static class AdvancedOutputExtensions {
    private const int TypeWriteDefaultDelay = 200;

    extension(Console) {
        /// <summary>
        /// Runs <paramref name="action"/> that should involve some form of outputting to the console. Set <paramref name="lines"/> to the number of lines your output consumes and choose the appropriate <paramref name="pipe"/>.
        /// </summary>
        /// <param name="action">The output action.</param>
        /// <param name="lines">The number of lines to clear.</param>
        /// <param name="pipe">The output pipe to use.</param>
        /// <remarks>
        /// Remember to clear the used lines after the last call to this method (for example with Console.ClearNextLines).
        /// </remarks>
        public static void Overwrite(Action action, int lines = 1, OutputPipe pipe = OutputPipe.Error) {
            var currentLine = Console.GetCurrentLine();
            Console.ClearNextLines(lines, pipe);
            action();
            Console.GoToLine(currentLine);
        }

        /// <summary>
        /// Runs <paramref name="action"/> that should involve some form of outputting to the console and uses <paramref name="state"/> to prevent closure allocation. Set <paramref name="lines"/> to the number of lines your output consumes and choose the appropriate <paramref name="pipe"/>.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state">The parameters that <paramref name="action"/> needs to use.</param>
        /// <param name="action">The output action.</param>
        /// <param name="lines">The number of lines to clear.</param>
        /// <param name="pipe">The output pipe to use.</param>
        /// <remarks>
        /// Remember to clear the used lines after the last call to this method (for example with Console.ClearNextLines).
        /// </remarks>
        public static void Overwrite<TState>(TState state, Action<TState> action, int lines = 1, OutputPipe pipe = OutputPipe.Error) where TState : allows ref struct {
            var currentLine = Console.GetCurrentLine();
            Console.ClearNextLines(lines, pipe);
            action(state);
            Console.GoToLine(currentLine);
        }

        /// <summary>
        /// Types out <paramref name="output"/> character by character with a delay of <paramref name="delay"/> milliseconds between each character, styled using <paramref name="colorTuple"/>.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="colorTuple"></param>
        /// <param name="delay">Delay in milliseconds between each character.</param>
        public static async Task TypeWrite(string output, (ConsoleColor foregroundColor, ConsoleColor backgroundColor) colorTuple, int delay = TypeWriteDefaultDelay) {
            foreach (char c in output) {
                Console.Write(c, OutputPipe.Out, colorTuple.foregroundColor, colorTuple.backgroundColor);
                await Task.Delay(delay);
            }
        }

        /// <summary>
        /// Types out <paramref name="output"/> character by character with a delay of <paramref name="delay"/> milliseconds between each character, styled using <paramref name="colorTuple"/> followed by a line terminator.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="colorTuple"></param>
        /// <param name="delay">Delay in milliseconds between each character.</param>
        public static async Task TypeWriteLine(string output, (ConsoleColor foregroundColor, ConsoleColor backgroundColor) colorTuple, int delay = TypeWriteDefaultDelay) {
            await TypeWrite(output, colorTuple, delay);
            Console.NewLine();
        }
    }
}