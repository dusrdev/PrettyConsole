namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Represents a progress bar that can be displayed in the console.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The progress bar update isn't tied to unit of time, it's up to the user to update it as needed. By managing the when the Update method is called, the user can have a more precise control over the progress bar. More calls, means more frequent rendering but at the cost of performance (very frequent updates may cause the terminal to lose sync with the method and will produce visual bugs, such as items not rendering in the right place)
    /// </para>
    /// <para>
    /// Please remember to clear the used lines after the last call to this method, you can use <see cref="ClearNextLines"/>
    /// </para>
    /// </remarks>
    public class ProgressBar {
        /// <summary>
		/// The default characters used for the progress bar filled portion.
		/// </summary>
        public const char DefaultProgressChar = '■';

        /// <summary>
        /// Gets or sets the character used to represent the progress.
        /// </summary>
        public char ProgressChar { get; set; } = DefaultProgressChar;

        /// <summary>
        /// Gets or sets the foreground color of the status (if rendered).
        /// </summary>
        public ConsoleColor ForegroundColor { get; set; } = Color.DefaultForegroundColor;

        /// <summary>
        /// Gets or sets the color of the progress portion of the bar.
        /// </summary>
        public ConsoleColor ProgressColor { get; set; } = Color.DefaultForegroundColor;

        private readonly Lock _lock = new();

        /// <summary>
        /// Updates the progress bar with the specified percentage.
        /// </summary>
        /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
        /// <remarks>
        /// Please remember to clear the used lines after the last call to this method, you can use <see cref="ClearNextLines"/>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(int percentage) => Update(percentage, ReadOnlySpan<char>.Empty);

        /// <summary>
        /// Updates the progress bar with the specified percentage.
        /// </summary>
        /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
        /// <remarks>
        /// Please remember to clear the used lines after the last call to this method, you can use <see cref="ClearNextLines"/>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(double percentage) => Update(percentage, ReadOnlySpan<char>.Empty);

        /// <summary>
        /// Updates the progress bar with the specified percentage and header text.
        /// </summary>
        /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
        /// <param name="status">The status text to be displayed after the progress bar.</param>
        /// <param name="sameLine">Whether to display the status before the progress bar on the same line. If not it will be displayed above the progress bar</param>
        /// <remarks>
        /// Please remember to clear the used lines after the last call to this method, you can use <see cref="ClearNextLines"/>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(double percentage, ReadOnlySpan<char> status, bool sameLine = true)
            => Update((int)percentage, status);

        /// <summary>
        /// Updates the progress bar with the specified percentage and header text.
        /// </summary>
        /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
        /// <param name="status">The status text to be displayed after the progress bar.</param>
        /// <param name="sameLine">Whether to display the status before the progress bar on the same line. If not it will be displayed above the progress bar</param>
        /// <remarks>
        /// Please remember to clear the used lines after the last call to this method, you can use <see cref="ClearNextLines"/>
        /// </remarks>
        public void Update(int percentage, ReadOnlySpan<char> status, bool sameLine = true) {
            lock (_lock) {
                var currentLine = GetCurrentLine();
                if (sameLine) {
                    ClearNextLines(1, OutputPipe.Error);
                    if (status.Length > 0) {
                        Write(status, OutputPipe.Error, ForegroundColor);
                        Write(' ');
                        WriteBar(OutputPipe.Error, percentage, ProgressColor, ProgressChar);
                    }
                } else {
                    bool hasStatus = status.Length > 0;
                    int lines = hasStatus ? 2 : 1;
                    ClearNextLines(lines, OutputPipe.Error);
                    if (hasStatus) WriteLine(status, OutputPipe.Error, ForegroundColor);
                    WriteBar(OutputPipe.Error, percentage, ProgressColor, ProgressChar);
                    NewLine(OutputPipe.Error);
                }
                GoToLine(currentLine);
            }
        }

        /// <summary>
        /// Writes a single progress bar segment without tracking state.
        /// </summary>
        /// <param name="pipe">The output pipe to write to.</param>
        /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
        /// <param name="progressColor">The color used for the filled segment of the bar.</param>
        /// <param name="progressChar">The character used to render the filled portion of the bar.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBar(OutputPipe pipe, double percentage, ConsoleColor progressColor, char progressChar = DefaultProgressChar) => WriteBar(pipe, (int)percentage, progressColor, progressChar);

        /// <summary>
        /// Writes a single progress bar segment without tracking state.
        /// </summary>
        /// <param name="pipe">The output pipe to write to.</param>
        /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
        /// <param name="progressColor">The color used for the filled segment of the bar.</param>
        /// <param name="progressChar">The character used to render the filled portion of the bar.</param>
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
        public static void WriteBar(OutputPipe pipe, int percentage, ConsoleColor progressColor, char progressChar = DefaultProgressChar) {
            ResetColors();

            int p = Math.Clamp(percentage, 0, 100);
            int bufferWidth = GetWidthOrDefault() - baseConsole.CursorLeft;

            const int bracketsAndSpacing = 3; // '[' + ']' + ' '
            const int percentageWidth = 3; // numeric portion width
            const int percentSymbolLength = 1; // '%' character
            int barLength = Math.Max(0, bufferWidth - (bracketsAndSpacing + percentageWidth + percentSymbolLength));

            var writer = GetWriter(pipe);
            writer.Write('[');

            if (barLength > 0) {
                int filled = Math.Min((int)(barLength * p * 0.01), barLength);

                if (filled > 0) {
                    SetColors(progressColor, baseConsole.BackgroundColor);
                    Span<char> s = stackalloc char[filled];
                    s.Fill(progressChar);
                    writer.Write(s);
                    ResetColors();
                }

                int remaining = barLength - filled;
                if (remaining > 0) {
                    writer.WriteWhiteSpaces(remaining);
                }
            }

            Write(pipe, $"] {p,3}%");
        }
    }
}
