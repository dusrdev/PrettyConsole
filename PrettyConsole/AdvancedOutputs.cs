using System.Buffers;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Clears the current line and overrides it with <paramref name="output"/>
    /// </summary>
    /// <param name="output"></param>
    public static void OverrideCurrentLine(ColoredOutput output) {
        var array = ArrayPool<char>.Shared.Rent(ogConsole.BufferWidth);
        Span<char> emptyLine = array.AsSpan(0, ogConsole.BufferWidth);
        emptyLine.Fill(' ');
        var currentLine = ogConsole.CursorTop;
        ogConsole.SetCursorPosition(0, currentLine);
        ogConsole.Out.Write(emptyLine);
        ogConsole.SetCursorPosition(0, currentLine);
        ArrayPool<char>.Shared.Return(array);
        Write(output);
    }

    /// <summary>
    /// Types out the <see cref="ColoredOutput"/> character by character with a delay of <paramref name="delay"/> milliseconds between each character.
    /// </summary>
    /// <param name="output"></param>
    /// <param name="delay">Delay in milliseconds between each character. Default is 200ms.</param>
    public static async Task TypeWrite(ColoredOutput output, int delay = 200) {
        ogConsole.ForegroundColor = output.ForegroundColor;
        ogConsole.BackgroundColor = output.BackgroundColor;
        for (int i = 0; i < output.Value.Length - 1; i++) {
            ogConsole.Write(output.Value[i]);
            await Task.Delay(delay);
        }

        ogConsole.Write(output.Value[^1]);
        ResetColors();
    }

    /// <summary>
    /// Types out the <see cref="ColoredOutput"/> character by character with a delay of <paramref name="delay"/> milliseconds between each character.
    /// </summary>
    /// <param name="output"></param>
    /// <param name="delay">Delay in milliseconds between each character. Default is 200ms.</param>
    public static async Task TypeWriteLine(ColoredOutput output, int delay = 200) {
        await TypeWrite(output, delay);
        ogConsole.WriteLine();
    }
}