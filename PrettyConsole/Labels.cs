using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

using PrettyConsole.Models;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Write any object to the console as a label, Colors.Default background - black text
    /// </summary>
    /// <param name="buffer">content</param>
    /// <remarks>
    /// To end line, call <ogConsole>NewLine()</ogConsole> after.
    /// </remarks>
    [Pure]
    public static void Label(ReadOnlySpan<char> buffer) {
        Label(buffer, ConsoleColor.Black, Colors.Default);
    }

    /// <summary>
    /// Write any object to the console as a label, modified foreground and background
    /// </summary>
    /// <param name="buffer">content</param>
    /// <param name="foreground">foreground color - i.e: color of the string representation</param>
    /// <param name="background">background color</param>
    /// <remarks>
    /// To end line, call <ogConsole>NewLine()</ogConsole> after.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static void Label(ReadOnlySpan<char> buffer, ConsoleColor foreground, ConsoleColor background) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = foreground;
            ogConsole.BackgroundColor = background;
            ogConsole.Out.Write(buffer);
        } finally {
            ogConsole.ResetColor();
        }
    }
    /// <summary>
    /// Write any object to the console as a label, Colors.Default background - black text
    /// </summary>
    /// <param name="output">content</param>
    /// <remarks>
    /// To end line, call <ogConsole>NewLine()</ogConsole> after.
    /// </remarks>
    [Pure]
    public static void Label(string? output) {
        Label(output.AsSpan(), ConsoleColor.Black, Colors.Default);
    }

    /// <summary>
    /// Write any object to the console as a label, modified foreground and background
    /// </summary>
    /// <param name="output">content</param>
    /// <param name="foreground">foreground color - i.e: color of the string representation</param>
    /// <param name="background">background color</param>
    /// <remarks>
    /// To end line, call <ogConsole>NewLine()</ogConsole> after.
    /// </remarks>
    [Pure]
    public static void Label(string? output, ConsoleColor foreground, ConsoleColor background) {
        Label(output.AsSpan(), foreground, background);
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console
    /// </summary>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [Obsolete("Use method with (string,foreground,background) tuples!")]
    [Pure]
    public static void Label(params (object item, ConsoleColor foreground, ConsoleColor background)[] elements) {
        var newElements = new (string, ConsoleColor, ConsoleColor)[elements.Length];
        for (int i = 0; i < elements.Length; i++) {
            newElements[i] = (elements[i].item?.ToString()
                            ?? string.Empty,
                            elements[i].foreground,
                            elements[i].background);
        }
        Label(newElements);
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console
    /// </summary>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [Pure]
    public static void Label(params (string item, ConsoleColor foreground, ConsoleColor background)[] elements) {
        if (elements is null || elements.Length is 0) {
            throw new ArgumentException("Invalid parameters");
        }
        try {
            ogConsole.ResetColor();
            foreach (var (o, foreground, background) in elements) {
                ogConsole.ForegroundColor = foreground;
                ogConsole.BackgroundColor = background;
                ogConsole.Write(o);
            }
        } finally {
            ogConsole.ResetColor();
        }
    }

    /// <summary>
    /// Write any object to the error console as a label, modified foreground and background
    /// </summary>
    /// <param name="output">content</param>
    /// <param name="foreground">foreground color - i.e: color of the string representation</param>
    /// <param name="background">background color</param>
    /// <remarks>
    /// To end line, call <ogConsole>NewLine()</ogConsole> after.
    /// </remarks>
    [Pure]
    public static void ErrorLabel(string? output, ConsoleColor foreground, ConsoleColor background) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = foreground;
            ogConsole.BackgroundColor = background;
            ogConsole.Error.Write(output);
        } finally {
            ogConsole.ResetColor();
        }
    }
}