using System;
using System.Threading.Tasks;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Clears the current line and overrides it with <paramref name="output"/>
    /// </summary>
    /// <param name="output"></param>
    public static void OverrideCurrentLine(string output) {
        OverrideCurrentLine(output, Colors.Default);
    }

    /// <summary>
    /// Clears the current line and overrides it with <paramref name="output"/>
    /// </summary>
    /// <param name="output"></param>
    /// <param name="color">foreground color (i.e text color)</param>
    public static void OverrideCurrentLine(string output, ConsoleColor color) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = color;
            var currentLine = ogConsole.CursorTop;
            ogConsole.SetCursorPosition(0, currentLine);
            ogConsole.Write(EmptyLine);
            ogConsole.SetCursorPosition(0, currentLine);
            ogConsole.Write(output);
        } finally {
            ogConsole.ResetColor();
        }
    }

    /// <summary>
    /// Types out the string <paramref name="output"/> character by character with a delay of <paramref name="delay"/> milliseconds between each character.
    /// </summary>
    /// <param name="output"></param>
    /// <param name="delay">Delay in milliseconds between each character. Default is 50ms.</param>
    public static async Task TypeWrite(string output, int delay = 50) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = Colors.Default;
            var length = output.Length - 1;
            int i = 0;
            while (i < length) {
                ogConsole.Write(output[i]);
                await Task.Delay(delay);
                i++;
            }
            ogConsole.Write(output[length]);
        } finally {
            ogConsole.ResetColor();
        }
    }

    /// <summary>
    /// Types out the string <paramref name="output"/> character by character with a delay of <paramref name="delay"/> milliseconds between each character and adds a new line at the end.
    /// </summary>
    /// <param name="output"></param>
    /// <param name="delay">Delay in milliseconds between each character. Default is 50ms.</param>
    public static async Task TypeWriteLine(string output, int delay = 50) {
        await TypeWrite(output, delay);
        ogConsole.WriteLine();
    }
}