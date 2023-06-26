using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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
    public static void WriteLine<T>(T item) {
        ArgumentNullException.ThrowIfNull(item);
        WriteLine(item.ToString(), Colors.Default);
    }

    /// <summary>
    /// Write a string to the console in the default color
    /// </summary>
    /// <param name="output">content</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string? output) {
        WriteLine(output, Colors.Default);
    }

    /// <summary>
    /// Write a string to the console in <paramref name="color"/>
    /// </summary>
    /// <param name="output">content</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string? output, ConsoleColor color) {
        Write(output, color);
        NewLine();
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
    [Obsolete("Use method with (string,color) tuples!")]
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
    public static void WriteLine(params (string item, ConsoleColor color)[] elements) {
        Write(elements);
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
    public static void WriteLineError<T>(T item, ConsoleColor color) {
        ArgumentNullException.ThrowIfNull(item);
        WriteError(item.ToString(), color);
        ogConsole.Error.WriteLine();
    }
}