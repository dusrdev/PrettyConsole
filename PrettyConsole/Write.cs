using System;
using System.Diagnostics.CodeAnalysis;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Write any object to the console in the default color
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable use regular string overload", Url = "http://help/unreferencedcode")]
    public static void Write<T>(T item) {
        ArgumentNullException.ThrowIfNull(item);
        Write(item.ToString(), Colors.Default);
    }

    /// <summary>
    /// Write a string to the console in the default color
    /// </summary>
    /// <param name="output">Output</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    public static void Write(string? output) {
        Write(output, Colors.Default);
    }

    /// <summary>
    /// Write a string to the console in <paramref name="color"/>
    /// </summary>
    /// <param name="output">content</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    public static void Write(string? output, ConsoleColor color) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = color;
            ogConsole.Write(output);
        } finally {
            ogConsole.ResetColor();
        }
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console
    /// </summary>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [Obsolete("Use method with (string,color) tuples!")]
    public static void Write(params (object item, ConsoleColor color)[] elements) {
        var newElements = new (string, ConsoleColor)[elements.Length];
        for (int i = 0; i < elements.Length; i++) {
            newElements[i] = (elements[i].item.ToString() ?? string.Empty, elements[i].color);
        }
        Write(newElements);
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console
    /// </summary>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    public static void Write(params (string item, ConsoleColor color)[] elements) {
        if (elements is null || elements.Length is 0) {
            throw new ArgumentException("Invalid parameters");
        }
        ogConsole.ResetColor();
        try {
            foreach (var (o, c) in elements) {
                ogConsole.ForegroundColor = c;
                ogConsole.Write(o);
            }
        } finally {
            ogConsole.ResetColor();
        }
    }

    /// <summary>
    /// Write a string to the error console in Colors.Error
    /// </summary>
    /// <param name="output">content</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLineError</ogConsole> with the same parameters
    /// </remarks>
    public static void WriteError(string? output) {
        WriteError(output, Colors.Error);
    }

    /// <summary>
    /// Write a string to the error console in <paramref name="color"/>
    /// </summary>
    /// <param name="output">content</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLineError</ogConsole> with the same parameters
    /// </remarks>
    public static void WriteError(string? output, ConsoleColor color) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = color;
            ogConsole.Error.Write(output);
        } finally {
            ogConsole.ResetColor();
        }
    }
}