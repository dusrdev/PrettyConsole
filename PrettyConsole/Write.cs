using System.Runtime.CompilerServices;
using System;
using System.Diagnostics.CodeAnalysis;

using PrettyConsole.Models;

using ogConsole = System.Console;
using System.Diagnostics.Contracts;

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
    [Pure]
    public static void Write<T>(T item) {
        ArgumentNullException.ThrowIfNull(item);
        Write(item.ToString(), Colors.Default);
    }

    /// <summary>
    /// Write a read-only span of characters to the console in the default color as set by <see cref="Colors.Default"/>.
    /// </summary>
    /// <param name="buffer">The read-only span of characters to write to the console.</param>
    /// <remarks>
    /// To end line, use <see cref="WriteLine(ReadOnlySpan{char})"/> with the same parameters.
    /// </remarks>
    [Pure]
    public static void Write(ReadOnlySpan<char> buffer) {
        Write(buffer, Colors.Default);
    }

    /// <summary>
    /// Write a read-only span of characters to the console in the specified color.
    /// </summary>
    /// <param name="buffer">The read-only span of characters to write to the console.</param>
    /// <param name="color">The color in which the output will be displayed.</param>
    /// <remarks>
    /// To end line, use <see cref="WriteLine(ReadOnlySpan{char}, ConsoleColor)"/> with the same parameters.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static void Write(ReadOnlySpan<char> buffer, ConsoleColor color) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = color;
            ogConsole.Out.Write(buffer);
        } finally {
            ogConsole.ResetColor();
        }
    }

    /// <summary>
    /// Write a string to the console in the default color
    /// </summary>
    /// <param name="output">Output</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [Pure]
    public static void Write(string? output) {
        Write(output.AsSpan(), Colors.Default);
    }

    /// <summary>
    /// Write a string to the console in <paramref name="color"/>
    /// </summary>
    /// <param name="output">content</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [Pure]
    public static void Write(string? output, ConsoleColor color) {
        Write(output.AsSpan(), color);
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console
    /// </summary>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [Obsolete("Use method with TextRenderingScheme parameter")]
    [Pure]
    public static void Write(params (object item, ConsoleColor color)[] elements) {
        var newElements = new (string, ConsoleColor)[elements.Length];
        for (int i = 0; i < elements.Length; i++) {
            newElements[i] = (elements[i].item.ToString() ?? string.Empty, elements[i].color);
        }
        Write(new TextRenderingScheme(newElements));
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console
    /// </summary>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [Pure]
    public static void Write(params (string item, ConsoleColor color)[] elements) {
        Write(new TextRenderingScheme(elements));
    }

    /// <summary>
    /// Write the specified <paramref name="scheme"/> to the console, rendering each element with its corresponding color.
    /// </summary>
    /// <param name="scheme">The <see cref="TextRenderingScheme"/> to write to the console.</param>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="scheme"/> contains no elements.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static void Write(TextRenderingScheme scheme) {
        if (scheme.Segments is { Length: 0 }) {
            throw new ArgumentException("Invalid parameters");
        }
        ogConsole.ResetColor();
        try {
            foreach (var (t, c) in scheme.Segments) {
                ogConsole.ForegroundColor = c;
                ogConsole.Write(t);
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
    [Pure]
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
    [Pure]
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