using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

using PrettyConsole.Models;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Write any object to the console in the default color and ends line
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <remarks>
    /// <para>
    /// To write without ending line, use <ogConsole>Write</ogConsole> with the same parameters
    /// </para>
    /// <para>
    /// This internally uses Write(string) to avoid boxing
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable use regular string overload", Url = "http://help/unreferencedcode")]
    [Pure]
    public static void WriteLine<T>(T item) {
        ArgumentNullException.ThrowIfNull(item);
        WriteLine(item.ToString(), Colors.Default);
    }

    /// <summary>
    /// Write a read-only span of characters to the console in the default color as set by <see cref="Colors.Default"/>, followed by the current line terminator.
    /// </summary>
    /// <param name="buffer">The read-only span of characters to write to the console.</param>
    /// <remarks>
    /// To stay on the same line, use <see cref="Write(ReadOnlySpan{char})"/> with the same parameters.
    /// </remarks>
    [Pure]
    public static void WriteLine(ReadOnlySpan<char> buffer) {
        WriteLine(buffer, Colors.Default);
    }

    /// <summary>
    /// Write a read-only span of characters to the console in the specified color, followed by the current line terminator.
    /// </summary>
    /// <param name="buffer">The read-only span of characters to write to the console.</param>
    /// <param name="color">The color in which the output will be displayed.</param>
    /// <remarks>
    /// To stay on the same line, use <see cref="Write(ReadOnlySpan{char}, ConsoleColor)"/> with the same parameters.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static void WriteLine(ReadOnlySpan<char> buffer, ConsoleColor color) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = color;
            ogConsole.Out.WriteLine(buffer);
        } finally {
            ogConsole.ResetColor();
        }
    }

    /// <summary>
    /// Write a string to the console in the default color
    /// </summary>
    /// <param name="output">content</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [Pure]
    public static void WriteLine(string? output) {
        WriteLine(output.AsSpan(), Colors.Default);
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
    public static void WriteLine(string? output, ConsoleColor color) {
        WriteLine(output.AsSpan(), color);
    }

    /// <summary>
    /// Write any object to the console in <paramref name="color"/> and ends line
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// <para>
    /// To write without ending line, use <ogConsole>Write</ogConsole> with the same parameters
    /// </para>
    /// <para>
    /// This internally uses Write(string) to avoid boxing
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable use regular string overload", Url = "http://help/unreferencedcode")]
    [Pure]
    public static void WriteLine<T>(T item, ConsoleColor color) {
        ArgumentNullException.ThrowIfNull(item);
        WriteLine(item.ToString(), color);
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console and ends line
    /// </summary>
    /// <remarks>
    /// To write without ending line, use <ogConsole>Write</ogConsole> with the same parameters
    /// </remarks>
    [Obsolete("Use method with TextRenderingScheme parameter")]
    [Pure]
    public static void WriteLine(params (object item, ConsoleColor color)[] elements) {
        Write(elements);
        NewLine();
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console and ends line
    /// </summary>
    /// <remarks>
    /// To write without ending line, use <ogConsole>Write</ogConsole> with the same parameters
    /// </remarks>
    [Pure]
    public static void WriteLine(params (string item, ConsoleColor color)[] elements) {
        Write(new TextRenderingScheme(elements));
        NewLine();
    }

    /// <summary>
    /// Write a string to the console in the default color and ends line
    /// </summary>
    /// <param name="scheme">The text rendering scheme to use for writing to the console</param>
    /// <remarks>
    /// <para>
    /// To write without ending line, use <ogConsole>Write</ogConsole> with the same parameters
    /// </para>
    /// </remarks>
    [Pure]
    public static void WriteLine(TextRenderingScheme scheme) {
        Write(scheme);
        NewLine();
    }

    /// <summary>
    /// Write a string to the error console in the default color and ends line
    /// </summary>
    /// <param name="output">content</param>
    /// <remarks>
    /// <para>
    /// To write without ending line, use <ogConsole>WriteError</ogConsole> with the same parameters
    /// </para>
    /// <para>
    /// This internally uses Write(string) to avoid boxing
    /// </para>
    /// </remarks>
    [Pure]
    public static void WriteLineError(string? output) {
        WriteError(output, Colors.Default);
        ogConsole.Error.WriteLine();
    }

    /// <summary>
    /// Write a string to the error console in the Colors.Error and ends line
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <remarks>
    /// <para>
    /// To write without ending line, use <ogConsole>WriteError</ogConsole> with the same parameters
    /// </para>
    /// <para>
    /// This internally uses Write(string) to avoid boxing
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable use regular string overload", Url = "http://help/unreferencedcode")]
    [Pure]
    public static void WriteLineError<T>(T item) {
        ArgumentNullException.ThrowIfNull(item);
        WriteError(item.ToString(), Colors.Error);
        ogConsole.Error.WriteLine();
    }

    /// <summary>
    /// Write any object to the error console in <paramref name="color"/> and ends line
    /// </summary>
    /// <param name="output">content</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// <para>
    /// To write without ending line, use <ogConsole>WriteError</ogConsole> with the same parameters
    /// </para>
    /// <para>
    /// This internally uses Write(string) to avoid boxing
    /// </para>
    /// </remarks>
    [Pure]
    public static void WriteLineError(string? output, ConsoleColor color) {
        WriteError(output, color);
        ogConsole.Error.WriteLine();
    }

    /// <summary>
    /// Write any object to the error console in <paramref name="color"/> and ends line
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// <para>
    /// To write without ending line, use <ogConsole>WriteError</ogConsole> with the same parameters
    /// </para>
    /// <para>
    /// This internally uses Write(string) to avoid boxing
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable use regular string overload", Url = "http://help/unreferencedcode")]
    [Pure]
    public static void WriteLineError<T>(T item, ConsoleColor color) {
        ArgumentNullException.ThrowIfNull(item);
        WriteError(item.ToString(), color);
        ogConsole.Error.WriteLine();
    }
}