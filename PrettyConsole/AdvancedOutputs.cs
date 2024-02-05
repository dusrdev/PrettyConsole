using System.Runtime.CompilerServices;
using System;
using System.Threading.Tasks;

using PrettyConsole.Models;

using ogConsole = System.Console;
using System.Diagnostics.Contracts;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Clears the current line and overrides it with <paramref name="output"/> using the default text color.
    /// </summary>
    /// <param name="output">The string to override the current line with.</param>
    [Pure]
    public static void OverrideCurrentLine(string output) {
        OverrideCurrentLine(output.AsSpan(), Colors.Default);
    }

    /// <summary>
    /// Clears the current line and overrides it with the specified <paramref name="output"/> using the specified <paramref name="color"/>.
    /// </summary>
    /// <param name="output">The string to override the current line with.</param>
    /// <param name="color">The color to use for the overridden line.</param>
    [Pure]
    public static void OverrideCurrentLine(string output, ConsoleColor color) {
        OverrideCurrentLine(output.AsSpan(), color);
        ColoredOutput a = "Hello" & Color.Red / Colors.White;
    }

    /// <summary>
    /// Clears the current line and overrides it with <paramref name="buffer"/>
    /// </summary>
    /// <param name="buffer"></param>
    [Pure]
    public static void OverrideCurrentLine(ReadOnlySpan<char> buffer) {
        OverrideCurrentLine(buffer, Colors.Default);
    }

    /// <summary>
    /// Clears the current line and overrides it with <paramref name="buffer"/>
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="color">foreground color (i.e text color)</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static void OverrideCurrentLine(ReadOnlySpan<char> buffer, ConsoleColor color) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = color;
            var currentLine = ogConsole.CursorTop;
            ogConsole.SetCursorPosition(0, currentLine);
            ogConsole.Write(EmptyLine);
            ogConsole.SetCursorPosition(0, currentLine);
            ogConsole.Out.Write(buffer);
        } finally {
            ogConsole.ResetColor();
        }
    }

    /// <summary>
    /// Clears the current line and overrides it with the specified <paramref name="scheme"/>
    /// </summary>
    /// <param name="scheme">The <see cref="TextRenderingScheme"/> to render on the current line.</param>
    [Pure]
    public static void OverrideCurrentLine(TextRenderingScheme scheme) {
        try {
            var currentLine = ogConsole.CursorTop;
            ogConsole.SetCursorPosition(0, currentLine);
            ogConsole.Write(EmptyLine);
            ogConsole.SetCursorPosition(0, currentLine);
            foreach (var (text, color) in scheme.Segments) {
                ogConsole.ForegroundColor = color;
                ogConsole.Write(text);
            }
        } finally {
            ogConsole.ResetColor();
        }
    }

    /// <summary>
    /// Types out the string <paramref name="output"/> character by character with a delay of <paramref name="delay"/> milliseconds between each character.
    /// </summary>
    /// <param name="output"></param>
    /// <param name="delay">Delay in milliseconds between each character. Default is 200ms.</param>
    [Pure]
    public static async Task TypeWrite(string output, int delay = 200) {
        await TypeWrite(output, Colors.Default, delay);
    }

    /// <summary>
    /// Types out the string <paramref name="output"/> character by character with a delay of <paramref name="delay"/> milliseconds between each character.
    /// </summary>
    /// <param name="output"></param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <param name="delay">Delay in milliseconds between each character. Default is 200ms.</param>
    [Pure]
    public static async Task TypeWrite(string output, ConsoleColor color, int delay = 200) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = color;
            var length = output.Length - 1;
            for (int i = 0; i < length; i++) {
                ogConsole.Write(output[i]);
                await Task.Delay(delay);
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
    /// <param name="delay">Delay in milliseconds between each character. Default is 200ms.</param>
    [Pure]
    public static async Task TypeWriteLine(string output, int delay = 200) {
        await TypeWrite(output, delay);
        ogConsole.WriteLine();
    }

    /// <summary>
    /// Types out the string <paramref name="output"/> character by character with a delay of <paramref name="delay"/> milliseconds between each character and adds a new line at the end.
    /// </summary>
    /// <param name="output"></param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <param name="delay">Delay in milliseconds between each character. Default is 200ms.</param>
    [Pure]
    public static async Task TypeWriteLine(string output, ConsoleColor color, int delay = 200) {
        await TypeWrite(output, color, delay);
        ogConsole.WriteLine();
    }
}