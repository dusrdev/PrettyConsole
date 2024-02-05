using System.Threading.Tasks;

using PrettyConsole.Models;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Clears the current line and overrides it with <paramref name="output"/>
    /// </summary>
    /// <param name="output"></param>
    public static void OverrideCurrentLine(ColoredOutput output) {
        var currentLine = ogConsole.CursorTop;
        ogConsole.SetCursorPosition(0, currentLine);
        ogConsole.Write(EmptyLine);
        ogConsole.SetCursorPosition(0, currentLine);
        Write(output);
    }

    /// <summary>
    /// Types out the <see cref="ColoredOutput"/> character by character with a delay of <paramref name="delay"/> milliseconds between each character.
    /// </summary>
    /// <param name="output"></param>
    /// <param name="delay">Delay in milliseconds between each character. Default is 200ms.</param>
    public static async Task TypeWrite(ColoredOutput output, int delay = 200) {
        try {
            ogConsole.ForegroundColor = output.ForegroundColor;
            ogConsole.BackgroundColor = output.BackgroundColor;
            for (int i = 0; i < output.Value.Length - 1; i++) {
                ogConsole.Write(output.Value[i]);
                await Task.Delay(delay);
            }
            ogConsole.Write(output.Value[^1]);
        } finally {
            ogConsole.ResetColor();
        }
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