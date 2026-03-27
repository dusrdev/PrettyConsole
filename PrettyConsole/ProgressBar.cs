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

    /// <summary>
    /// Gets or sets an optional total width for the rendered bar line (includes brackets, spacing, and percentage).
    /// </summary>
    public int? MaxLineWidth { get; set; }

    private readonly Lock _lock = new();

    /// <summary>
    /// Updates the progress bar with the specified percentage.
    /// </summary>
    /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
    /// <remarks>
    /// Please remember to clear the used lines after the last call to this method, you can use Console.ClearNextLines.
    /// </remarks>
    public void Update(int percentage) => Update(percentage, string.Empty);

    /// <summary>
    /// Updates the progress bar with the specified percentage.
    /// </summary>
    /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
    /// <remarks>
    /// Please remember to clear the used lines after the last call to this method, you can use Console.ClearNextLines.
    /// </remarks>
    public void Update(double percentage) => Update((int)percentage, string.Empty);

    /// <summary>
    /// Updates the progress bar with the specified percentage and header text.
    /// </summary>
    /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
    /// <param name="status">The status text to be displayed after the progress bar.</param>
    /// <param name="sameLine">Whether to display the status before the progress bar on the same line. If not it will be displayed above the progress bar, if set to false, the progress bar will use 2 lines.</param>
    /// <remarks>Remember to clear the used lines after the last call (e.g., with Console.ClearNextLines).</remarks>
    public void Update(double percentage, string status, bool sameLine = true)
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
    public void Update(int percentage, string status, bool sameLine = true) {
        if (status.Length == 0) Update(percentage, null, sameLine);
        else Update(percentage, (builder, out handler) => handler = builder.Build(OutputPipe.Error, $"{status}"), sameLine);
    }

    /// <summary>
    /// Updates the progress bar with the specified percentage and header text.
    /// </summary>
    /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
    /// <param name="factory">Optional header factory invoked on each render; use it to emit dynamic status text with <see cref="PrettyConsoleInterpolatedStringHandler"/> (same pattern as <see cref="Spinner"/>).</param>
    /// <param name="sameLine">Whether to display the status before the progress bar on the same line. If not it will be displayed above the progress bar, if set to false, the progress bar will use 2 lines.</param>
    /// <remarks>
    /// Please remember to clear the used lines after the last call to this method, you can use Console.ClearNextLines.
    /// </remarks>
    public void Update(double percentage, PrettyConsoleInterpolatedStringHandlerFactory? factory = null, bool sameLine = true)
        => Update((int)percentage, factory, sameLine);

    /// <summary>
    /// Updates the progress bar with the specified percentage and header text.
    /// </summary>
    /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
    /// <param name="factory">Optional header factory invoked on each render; use it to emit dynamic status text with <see cref="PrettyConsoleInterpolatedStringHandler"/> (same pattern as <see cref="Spinner"/>).</param>
    /// <param name="sameLine">Whether to display the status before the progress bar on the same line. If not it will be displayed above the progress bar, if set to false, the progress bar will use 2 lines.</param>
    /// <remarks>
    /// Please remember to clear the used lines after the last call to this method, you can use Console.ClearNextLines.
    /// </remarks>
    [OverloadResolutionPriority(int.MaxValue)]
    public void Update(int percentage, PrettyConsoleInterpolatedStringHandlerFactory? factory = null, bool sameLine = true) {
        lock (_lock) {
            var currentLine = Console.GetCurrentLine();
            if (sameLine) {
                Console.ClearNextLines(1);
                if (factory is not null) {
                    factory(PrettyConsoleInterpolatedStringHandlerBuilder.Singleton, out var handler);
                    handler.Flush();
                    ConsoleContext.GetPipeTarget(OutputPipe.Error).WriteWhiteSpaces(1);
                    Render(OutputPipe.Error, percentage, ProgressColor, ProgressChar, MaxLineWidth);
                }
            } else {
                if (factory is not null) {
                    Console.ClearNextLines(2);
                    factory(PrettyConsoleInterpolatedStringHandlerBuilder.Singleton, out var handler);
                    handler.AppendNewLine();
                    handler.Flush();
                    Render(OutputPipe.Error, percentage, ProgressColor, ProgressChar, MaxLineWidth);
                } else {
                    Console.ClearNextLines(1);
                    Render(OutputPipe.Error, percentage, ProgressColor, ProgressChar, MaxLineWidth);
                }
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
    public static void Render(OutputPipe pipe, double percentage, ConsoleColor progressColor, char progressChar = DefaultProgressChar, int? maxLineWidth = null)
        => Render(pipe, (int)percentage, progressColor, progressChar, maxLineWidth);

    /// <summary>
    /// Writes a single progress bar segment without tracking state.
    /// </summary>
    /// <param name="pipe">The output pipe to write to.</param>
    /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
    /// <param name="progressColor">The color used for the filled segment of the bar.</param>
    /// <param name="progressChar">The character used to render the filled portion of the bar.</param>
    /// <param name="maxLineWidth">Optional total line length (including brackets and percentage). When provided, the rendered output will not exceed this width unless the decorations already require more characters.</param>
    public static void Render(OutputPipe pipe, int percentage, ConsoleColor progressColor, char progressChar = DefaultProgressChar, int? maxLineWidth = null) {
        var handler = new PrettyConsoleInterpolatedStringHandler(pipe);
        AppendTo(ref handler, percentage, progressColor, Console.CursorLeft, progressChar, maxLineWidth);
        handler.Flush();
    }

    internal static void AppendTo(
        ref PrettyConsoleInterpolatedStringHandler handler,
        int percentage,
        ConsoleColor progressColor,
        int cursorLeft,
        char progressChar = DefaultProgressChar,
        int? maxLineWidth = null) {
        int p = Math.Clamp(percentage, 0, 100);
        int bufferWidth = Math.Max(0, ConsoleContext.GetWidthOrDefault() - cursorLeft);

        const int bracketsAndSpacing = 3; // '[' + ']' + ' '
        const int percentageWidth = 3; // numeric portion width
        const int percentSymbolLength = 1; // '%' character
        const int decorationWidth = bracketsAndSpacing + percentageWidth + percentSymbolLength;

        int constrainedWidth = bufferWidth;
        if (maxLineWidth.HasValue && maxLineWidth.Value > 0) {
            constrainedWidth = Math.Min(bufferWidth, Math.Max(maxLineWidth.Value, decorationWidth));
        }

        int barLength = Math.Max(0, constrainedWidth - decorationWidth);

        int filled = Math.Min((int)(barLength * p * 0.01), barLength);
        Span<char> progress = filled > 0
            ? stackalloc char[filled]
            : Span<char>.Empty;
        progress.Fill(progressChar);
        int remaining = barLength - filled;

        handler.AppendLiteral("[");
        handler.AppendFormatted(progressColor);
        if (filled > 0) {
            handler.AppendFormatted(progress);
        }
        handler.AppendFormatted(ConsoleColor.DefaultForeground);
        if (remaining > 0) {
            handler.AppendFormatted(new WhiteSpace(remaining));
        }
        handler.AppendLiteral("] ");
        handler.AppendFormatted(p, 3);
        handler.AppendFormatted('%');
    }
}
