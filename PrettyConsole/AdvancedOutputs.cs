using System.Runtime.CompilerServices;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Clears the current line and overrides it with <paramref name="output"/>
    /// </summary>
    /// <param name="output"></param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void OverrideCurrentLine(ReadOnlySpan<ColoredOutput> output) {
        using var memoryOwner = Utils.ObtainMemory(baseConsole.BufferWidth);
        Span<char> emptyLine = memoryOwner.Memory.Span.Slice(0, baseConsole.BufferWidth);
        emptyLine.Fill(' ');
        var currentLine = baseConsole.CursorTop;
        baseConsole.SetCursorPosition(0, currentLine);
        Error.Write(emptyLine);
        baseConsole.SetCursorPosition(0, currentLine);
        WriteError(output);
        baseConsole.SetCursorPosition(0, currentLine);
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