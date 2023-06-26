using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="type"></param>
    /// <param name="outputColor"></param>
    /// <param name="inputColor"></param>
    /// <returns>Converted input</returns>
    /// <remarks>
    /// For complex types request a string and validate/convert yourself
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static T? ReadLine<T>(string message, Type type, ConsoleColor outputColor, ConsoleColor inputColor) {
        var input = ReadLine(message, outputColor, inputColor);
        return (T?)Convert.ChangeType(input, type);
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="type"></param>
    /// <param name="inputColor"></param>
    /// <returns>Converted input</returns>
    /// <remarks>
    /// For complex types request a string and validate/convert yourself
    /// </remarks>
    [Pure]
    public static T? ReadLine<T>(string message, Type type, ConsoleColor inputColor) {
        return ReadLine<T?>(message, type, Colors.Default, inputColor);
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="type"></param>
    /// <returns>Converted input</returns>
    /// <remarks>
    /// For complex types request a string and validate/convert yourself
    /// </remarks>
    [Pure]
    public static T? ReadLine<T>(string message, Type type) {
        return ReadLine<T?>(message, type, Colors.Default, Colors.Input);
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T">Any common type</typeparam>
    /// <param name="message">Request title</param>
    /// <param name="outputColor"></param>
    /// <param name="inputColor"></param>
    /// <returns>Converted input</returns>
    /// <remarks>
    /// For complex types request a string and validate/convert yourself
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable add the output type or use string overload instead", Url = "http://help/unreferencedcode")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static T? ReadLine<T>(string message, ConsoleColor outputColor, ConsoleColor inputColor) {
        var input = ReadLine(message, outputColor, inputColor);

        ArgumentNullException.ThrowIfNull(input);

        // Convert input to desired type
        var converter = TypeDescriptor.GetConverter(typeof(T));
        return (T?)converter.ConvertFromString(input!);
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T">Any common type</typeparam>
    /// <param name="message">Request title</param>
    /// <param name="inputColor"></param>
    /// <returns>Converted input</returns>
    /// <remarks>
    /// For complex types request a string and validate/convert yourself
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable add the output type or use string overload instead", Url = "http://help/unreferencedcode")]
    [Pure]
    public static T? ReadLine<T>(string message, ConsoleColor inputColor) {
        return ReadLine<T?>(message, Colors.Default, inputColor);
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T">Any common type</typeparam>
    /// <param name="message">Request title</param>
    /// <returns>Converted input</returns>
    /// <remarks>
    /// For complex types request a string and validate/convert yourself
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable add the output type or use string overload instead", Url = "http://help/unreferencedcode")]
    [Pure]
    public static T? ReadLine<T>(string message) {
        return ReadLine<T?>(message, Colors.Default, Colors.Input);
    }

    /// <summary>
    /// Used to request user input
    /// </summary>
    /// <param name="message">Request title</param>
    /// <param name="outputColor"></param>
    /// <param name="inputColor"></param>
    /// <returns>Trimmed string, or empty if the input was empty</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static string ReadLine(string message, ConsoleColor outputColor, ConsoleColor inputColor) {
        Write(message, outputColor);
        ogConsole.ForegroundColor = inputColor;
        var input = ogConsole.ReadLine();
        ogConsole.ResetColor();

        return string.IsNullOrWhiteSpace(input) ? string.Empty : input.Trim();
    }

    /// <summary>
    /// Used to request user input
    /// </summary>
    /// <param name="message">Request title</param>
    /// <param name="inputColor"></param>
    /// <returns>Trimmed string</returns>
    [Pure]
    public static string ReadLine(string message, ConsoleColor inputColor) {
        return ReadLine(message, Colors.Default, inputColor);
    }

    /// <summary>
    /// Used to request user input
    /// </summary>
    /// <param name="message">Request title</param>
    /// <returns>Trimmed string</returns>
    public static string ReadLine(string message) {
        return ReadLine(message, Colors.Default, Colors.Input);
    }
}