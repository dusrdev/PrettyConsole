namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Clears the current line and overrides it with <paramref name="output"/>
    /// </summary>
    /// <param name="output"></param>
    /// <param name="pipe">The output pipe to use</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void OverwriteCurrentLine(ReadOnlySpan<ColoredOutput> output, OutputPipe pipe = OutputPipe.Error) {
        var currentLine = GetCurrentLine();
        ClearNextLines(1, pipe);
        Write(output, pipe);
        GoToLine(currentLine);
    }

    /// <summary>
    /// Runs <paramref name="action"/> that should involve some form of outputting to the console. Set <paramref name="lines"/> according to the outputs you use in <paramref name="action"/> and configure the appropriate <paramref name="pipe"/>
    /// </summary>
    /// <param name="action">The output action.</param>
    /// <param name="lines">The amount of lines to clear.</param>
    /// <param name="pipe">The output pipe to use.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void Overwrite(Action action, int lines = 1, OutputPipe pipe = OutputPipe.Error) {
        var currentLine = GetCurrentLine();
        ClearNextLines(lines, pipe);
        action();
        GoToLine(currentLine);
    }

    /// <summary>
    /// Runs <paramref name="action"/> that should involve some form of outputting to the console and use <paramref name="state"/> to prevent closure allocation. Set <paramref name="lines"/> according to the outputs you use in <paramref name="action"/> and configure the appropriate <paramref name="pipe"/>
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state">The parameters that <paramref name="action"/> needs to use.</param>
    /// <param name="action">The output action.</param>
    /// <param name="lines">The amount of lines to clear.</param>
    /// <param name="pipe">The output pipe to use.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void Overwrite<TState>(TState state, Action<TState> action, int lines = 1, OutputPipe pipe = OutputPipe.Error) where TState : allows ref struct {
        var currentLine = GetCurrentLine();
        ClearNextLines(lines, pipe);
        action(state);
        GoToLine(currentLine);
    }

    private const int TypeWriteDefaultDelay = 200;

    /// <summary>
    /// Types out the <see cref="ColoredOutput"/> character by character with a delay of <paramref name="delay"/> milliseconds between each character.
    /// </summary>
    /// <param name="output"></param>
    /// <param name="delay">Delay in milliseconds between each character.</param>
    public static async Task TypeWrite(ColoredOutput output, int delay = TypeWriteDefaultDelay) {
        SetColors(output.ForegroundColor, output.BackgroundColor);
        for (int i = 0; i < output.Value.Length - 1; i++) {
            Out.Write(output.Value[i]);
            await Task.Delay(delay);
        }

        Out.Write(output.Value[output.Value.Length - 1]);
        ResetColors();
    }

    /// <summary>
    /// Types out the <see cref="ColoredOutput"/> character by character with a delay of <paramref name="delay"/> milliseconds between each character.
    /// </summary>
    /// <param name="output"></param>
    /// <param name="delay">Delay in milliseconds between each character.</param>
    public static async Task TypeWriteLine(ColoredOutput output, int delay = TypeWriteDefaultDelay) {
        await TypeWrite(output, delay);
        NewLine();
    }
}