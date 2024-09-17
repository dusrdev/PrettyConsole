using System.Runtime.CompilerServices;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Clears the current line and overrides it with <paramref name="output"/>
    /// </summary>
    /// <param name="output"></param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void OverrideCurrentLine(ReadOnlySpan<ColoredOutput> output) {
        using var memoryOwner = Helper.ObtainMemory(ogConsole.BufferWidth);
        Span<char> emptyLine = memoryOwner.Memory.Span.Slice(0, ogConsole.BufferWidth);
        emptyLine.Fill(' ');
        var currentLine = ogConsole.CursorTop;
        ogConsole.SetCursorPosition(0, currentLine);
        ogConsole.Error.Write(emptyLine);
        ogConsole.SetCursorPosition(0, currentLine);
        WriteError(output);
    }

    // /// <summary>
    // /// Clears the current line and overrides it with <paramref name="outputs"/>
    // /// </summary>
    // /// <param name="outputs"></param>
    // [MethodImpl(MethodImplOptions.Synchronized)]
    // public static void OverrideLines(ReadOnlySpan<ReadOnlySpan<ColoredOutput>> outputs) {
    //     using var memoryOwner = Helper.ObtainMemory(ogConsole.BufferWidth);
    //     Span<char> emptyLine = memoryOwner.Memory.Span.Slice(0, ogConsole.BufferWidth);
    //     emptyLine.Fill(' ');
    //     var currentLine = ogConsole.CursorTop;
    //     ogConsole.SetCursorPosition(0, currentLine);
    //     ogConsole.Error.Write(emptyLine);
    //     ogConsole.SetCursorPosition(0, currentLine);
    //     WriteError(outputs);
    // }

    private const int TypeWriteDefaultDelay = 200;

    /// <summary>
    /// Types out the <see cref="ColoredOutput"/> character by character with a delay of <paramref name="delay"/> milliseconds between each character.
    /// </summary>
    /// <param name="output"></param>
    /// <param name="delay">Delay in milliseconds between each character.</param>
    public static async Task TypeWrite(ColoredOutput output, int delay = TypeWriteDefaultDelay) {
        ResetColors();
        ogConsole.ForegroundColor = output.ForegroundColor;
        ogConsole.BackgroundColor = output.BackgroundColor;
        for (int i = 0; i < output.Value.Length - 1; i++) {
            ogConsole.Write(output.Value[i]);
            await Task.Delay(delay);
        }

        ogConsole.Write(output.Value[output.Value.Length - 1]);
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