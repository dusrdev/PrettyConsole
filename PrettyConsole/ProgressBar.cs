namespace PrettyConsole;

/// <summary>
/// Represents a progress bar that can be displayed in the console.
/// </summary>
/// <remarks>
/// <para>
/// The progress bar update isn't tied to unit of time, it's up to the user to update it as needed. By managing the when the Update method is called, the user can have a more precise control over the progress bar. More calls, means more frequent rendering but at the cost of performance (very frequent updates may cause the terminal to lose sync with the method and will produce visual bugs, such as items not rendering in the right place).
/// </para>
/// <para>
/// Please remember to clear the used lines after the last call to this method, you can use Console.ClearNextLines.
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
    public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.DefaultForeground;

    /// <summary>
    /// Gets or sets the color of the progress portion of the bar.
    /// </summary>
    public ConsoleColor ProgressColor { get; set; } = ConsoleColor.DefaultForeground;

    private readonly Lock _lock = new();

    /// <summary>
    /// Updates the progress bar with the specified percentage.
    /// </summary>
    /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
    /// <remarks>
    /// Please remember to clear the used lines after the last call to this method, you can use Console.ClearNextLines.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update(int percentage) => Update(percentage, ReadOnlySpan<char>.Empty, true);

    /// <summary>
    /// Updates the progress bar with the specified percentage.
    /// </summary>
    /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
    /// <remarks>
    /// Please remember to clear the used lines after the last call to this method, you can use Console.ClearNextLines.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update(double percentage) => Update((int)percentage, ReadOnlySpan<char>.Empty, true);

    /// <summary>
    /// Updates the progress bar with the specified percentage and header text.
    /// </summary>
    /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
    /// <param name="status">The status text to be displayed after the progress bar.</param>
    /// <param name="sameLine">Whether to display the status before the progress bar on the same line. If not it will be displayed above the progress bar, if set to false, the progress bar will use 2 lines.</param>
    /// <remarks>
    /// Please remember to clear the used lines after the last call to this method, you can use Console.ClearNextLines.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update(double percentage, ReadOnlySpan<char> status, bool sameLine = true)
        => Update((int)percentage, status, sameLine);

    /// <summary>
    /// Updates the progress bar with the specified percentage and header text.
    /// </summary>
    /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
    /// <param name="status">The status text to be displayed after the progress bar.</param>
    /// <param name="sameLine">Whether to display the status before the progress bar on the same line. If not it will be displayed above the progress bar, if set to false, the progress bar will use 2 lines.</param>
    /// <remarks>
    /// Please remember to clear the used lines after the last call to this method, you can use Console.ClearNextLines.
    /// </remarks>
    public void Update(int percentage, ReadOnlySpan<char> status, bool sameLine = true) {
        lock (_lock) {
            var currentLine = Console.GetCurrentLine();
            if (sameLine) {
                Console.ClearNextLines(1, OutputPipe.Error);
                if (status.Length > 0) {
                    Console.Write(status, OutputPipe.Error, ForegroundColor);
                    PrettyConsoleExtensions.GetWriter(OutputPipe.Error).WriteWhiteSpaces(1);
                    WriteProgressBar(OutputPipe.Error, percentage, ProgressColor, ProgressChar);
                }
            } else {
                bool hasStatus = status.Length > 0;
                int lines = hasStatus ? 2 : 1;
                Console.ClearNextLines(lines, OutputPipe.Error);
                if (hasStatus) Console.WriteLine(status, OutputPipe.Error, ForegroundColor);
                WriteProgressBar(OutputPipe.Error, percentage, ProgressColor, ProgressChar);
            }
            Console.GoToLine(currentLine);
        }
    }

    /// <summary>
    /// Writes a single progress bar segment without tracking state.
    /// </summary>
    /// <param name="pipe">The output pipe to write to.</param>
    /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
    /// <param name="progressColor">The color used for the filled segment of the bar.</param>
    /// <param name="progressChar">The character used to render the filled portion of the bar.</param>
    /// <param name="maxLineWidth">Optional total line length (including brackets and percentage). When provided, the rendered output will not exceed this width unless the decorations already require more characters.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteProgressBar(OutputPipe pipe, double percentage, ConsoleColor progressColor, char progressChar = DefaultProgressChar, int? maxLineWidth = null)
        => WriteProgressBar(pipe, (int)percentage, progressColor, progressChar, maxLineWidth);

    /// <summary>
    /// Writes a single progress bar segment without tracking state.
    /// </summary>
    /// <param name="pipe">The output pipe to write to.</param>
    /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
    /// <param name="progressColor">The color used for the filled segment of the bar.</param>
    /// <param name="progressChar">The character used to render the filled portion of the bar.</param>
    /// <param name="maxLineWidth">Optional total line length (including brackets and percentage). When provided, the rendered output will not exceed this width unless the decorations already require more characters.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    public static void WriteProgressBar(OutputPipe pipe, int percentage, ConsoleColor progressColor, char progressChar = DefaultProgressChar, int? maxLineWidth = null) {
        Console.ResetColor();

        int p = Math.Clamp(percentage, 0, 100);
        int bufferWidth = Math.Max(0, PrettyConsoleExtensions.GetWidthOrDefault() - Console.CursorLeft);

        const int bracketsAndSpacing = 3; // '[' + ']' + ' '
        const int percentageWidth = 3; // numeric portion width
        const int percentSymbolLength = 1; // '%' character
        const int decorationWidth = bracketsAndSpacing + percentageWidth + percentSymbolLength;

        int constrainedWidth = bufferWidth;
        if (maxLineWidth.HasValue && maxLineWidth.Value > 0) {
            constrainedWidth = Math.Min(bufferWidth, Math.Max(maxLineWidth.Value, decorationWidth));
        }

        int barLength = Math.Max(0, constrainedWidth - decorationWidth);

        var writer = PrettyConsoleExtensions.GetWriter(pipe);
        Console.Write<char>('[', pipe);

        if (barLength > 0) {
            int filled = Math.Min((int)(barLength * p * 0.01), barLength);

            if (filled > 0) {
                Console.SetColors(progressColor, Console.BackgroundColor);
                Span<char> s = stackalloc char[filled];
                s.Fill(progressChar);
                writer.Write(s);
                Console.ResetColor();
            }

            int remaining = barLength - filled;
            if (remaining > 0) {
                writer.WriteWhiteSpaces(remaining);
            }
        }

        Console.WriteInterpolated(pipe, $"] {p,3}%");
    }
}
