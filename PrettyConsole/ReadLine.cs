namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="result">The result of the parsing</param>
    /// <returns>True if the parsing was successful, false otherwise</returns>
    public static bool TryReadLine<T>(ReadOnlySpan<ColoredOutput> message, out T? result) where T : IParsable<T> {
        Write(message);
        var input = baseConsole.ReadLine();
        return T.TryParse(input, null, out result);
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="default">The default value to return if parsing fails</param>
    /// <param name="result">The result of the parsing</param>
    /// <returns>True if the parsing was successful, false otherwise</returns>
    public static bool TryReadLine<T>(ReadOnlySpan<ColoredOutput> message, T @default, out T result) where T : IParsable<T> {
        Write(message);
        var input = baseConsole.ReadLine();
        var couldParse = T.TryParse(input, null, out T? innerResult);
        result = innerResult ?? @default;
        return couldParse;
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="inputColor">The color of the input</param>
    /// <param name="result">The result of the parsing</param>
    /// <returns>True if the parsing was successful, false otherwise</returns>
    public static bool TryReadLine<T>(ReadOnlySpan<ColoredOutput> message, ConsoleColor inputColor, out T? result)
        where T : IParsable<T>
        => TryReadLine(message, inputColor, Color.DefaultBackgroundColor, out result);

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="inputColor">The color of the input</param>
    /// <param name="default">The default value to return if parsing fails</param>
    /// <param name="result">The result of the parsing</param>
    /// <returns>Whether the parsing was successful</returns>
    public static bool TryReadLine<T>(ReadOnlySpan<ColoredOutput> message, ConsoleColor inputColor, T @default, out T result)
        where T : IParsable<T>
        => TryReadLine(message, inputColor, Color.DefaultBackgroundColor, @default, out result);

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="inputColor">The color of the input</param>
    /// <param name="inputBackgroundColor">The background color of the input</param>
    /// <param name="result">The result of the parsing</param>
    /// <returns>Whether the parsing was successful</returns>
    public static bool TryReadLine<T>(ReadOnlySpan<ColoredOutput> message, ConsoleColor inputColor, ConsoleColor inputBackgroundColor, out T? result) where T : IParsable<T> {
        Write(message);
        baseConsole.ForegroundColor = inputColor;
        baseConsole.BackgroundColor = inputBackgroundColor;
        var input = baseConsole.ReadLine();
        var res = T.TryParse(input, null, out result);
        ResetColors();
        return res;
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="inputColor">The color of the input</param>
    /// <param name="inputBackgroundColor">The background color of the input</param>
    /// <param name="default">The default value to return if parsing fails</param>
    /// <param name="result">The result of the parsing</param>
    /// <returns>Whether the parsing was successful</returns>
    public static bool TryReadLine<T>(ReadOnlySpan<ColoredOutput> message, ConsoleColor inputColor, ConsoleColor inputBackgroundColor, T @default, out T result) where T : IParsable<T> {
        Write(message);
        baseConsole.ForegroundColor = inputColor;
        baseConsole.BackgroundColor = inputBackgroundColor;
        var input = baseConsole.ReadLine();
        var res = T.TryParse(input, null, out T? innerResult);
        ResetColors();
        result = innerResult ?? @default;
        return res;
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="ignoreCase">Whether to ignore case when parsing</param>
    /// <param name="result">The result of the parsing</param>
    /// <returns>Whether the parsing was successful</returns>
    public static bool TryReadLine<TEnum>(ReadOnlySpan<ColoredOutput> message, bool ignoreCase, out TEnum result) where TEnum : struct, Enum
    => TryReadLine(message, ignoreCase, default, out result);

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="ignoreCase">Whether to ignore case when parsing</param>
    /// <param name="default">The default value to return if parsing fails</param>
    /// <param name="result">The result of the parsing</param>
    /// <returns>Whether the parsing was successful</returns>
    public static bool TryReadLine<TEnum>(ReadOnlySpan<ColoredOutput> message, bool ignoreCase, TEnum @default, out TEnum result) where TEnum : struct, Enum {
        Write(message);
        var input = baseConsole.ReadLine();
        var res = Enum.TryParse(input, ignoreCase, out result);
        if (!res) {
            result = @default;
        }
        return res;
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="inputColor">The color of the input</param>
    /// <param name="ignoreCase">Whether to ignore case when parsing</param>
    /// <param name="result">The result of the parsing</param>
    /// <returns>Whether the parsing was successful</returns>
    public static bool TryReadLine<TEnum>(ReadOnlySpan<ColoredOutput> message, ConsoleColor inputColor, bool ignoreCase, out TEnum result)
        where TEnum : struct, Enum
        => TryReadLine(message, inputColor, Color.DefaultBackgroundColor, ignoreCase, out result);

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="inputColor">The color of the input</param>
    /// <param name="ignoreCase">Whether to ignore case when parsing</param>
    /// <param name="default">The default value to return if parsing fails</param>
    /// <param name="result">The result of the parsing</param>
    /// <returns>Whether the parsing was successful</returns>
    public static bool TryReadLine<TEnum>(ReadOnlySpan<ColoredOutput> message, ConsoleColor inputColor, bool ignoreCase, TEnum @default, out TEnum result)
        where TEnum : struct, Enum
        => TryReadLine(message, inputColor, Color.DefaultBackgroundColor, ignoreCase, @default, out result);

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="inputColor">The color of the input</param>
    /// <param name="inputBackgroundColor">The background color of the input</param>
    /// <param name="ignoreCase">Whether to ignore case when parsing</param>
    /// <param name="result">The result of the parsing</param>
    /// <returns>Whether the parsing was successful</returns>
    public static bool TryReadLine<TEnum>(ReadOnlySpan<ColoredOutput> message, ConsoleColor inputColor, ConsoleColor inputBackgroundColor,
        bool ignoreCase, out TEnum result) where TEnum : struct, Enum
        => TryReadLine(message, inputColor, inputBackgroundColor, ignoreCase, default, out result);

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="inputColor">The color of the input</param>
    /// <param name="inputBackgroundColor">The background color of the input</param>
    /// <param name="ignoreCase">Whether to ignore case when parsing</param>
    /// <param name="default">The default value to return if parsing fails</param>
    /// <param name="result">The result of the parsing</param>
    /// <returns>Whether the parsing was successful</returns>
    public static bool TryReadLine<TEnum>(ReadOnlySpan<ColoredOutput> message, ConsoleColor inputColor, ConsoleColor inputBackgroundColor, bool ignoreCase, TEnum @default, out TEnum result) where TEnum : struct, Enum {
        Write(message);
        baseConsole.ForegroundColor = inputColor;
        baseConsole.BackgroundColor = inputBackgroundColor;
        var input = baseConsole.ReadLine();
        var res = Enum.TryParse(input, ignoreCase, out result);
        if (!res) {
            result = @default;
        }
        ResetColors();
        return res;
    }

    /// <summary>
    /// Used to request user input without any prepended message
    /// </summary>
    /// <remarks>
    /// You can use <see cref="Write(ColoredOutput)"/> or it's overloads in conjunction with this to create more complex input requests.
    /// </remarks>
    public static string? ReadLine() => baseConsole.ReadLine();

    /// <summary>
    /// Used to request user input without any prepended message
    /// </summary>
    /// <param name="inputColor">The color of the input</param>
    /// <returns>The result of the parsing</returns>
    /// <remarks>
    /// You can use <see cref="Write(ColoredOutput)"/> or it's overloads in conjunction with this to create more complex input requests.
    /// </remarks>
    public static string? ReadLine(ConsoleColor inputColor) => ReadLine(inputColor, Color.DefaultBackgroundColor);

    /// <summary>
    /// Used to request user input without any prepended message
    /// </summary>
    /// <param name="inputColor">The color of the input</param>
    /// <param name="inputBackgroundColor">The background color of the input</param>
    /// <remarks>
    /// You can use <see cref="Write(ColoredOutput)"/> or it's overloads in conjunction with this to create more complex input requests.
    /// </remarks>
    public static string? ReadLine(ConsoleColor inputColor, ConsoleColor inputBackgroundColor) {
        baseConsole.ForegroundColor = inputColor;
        baseConsole.BackgroundColor = inputBackgroundColor;
        var res = baseConsole.ReadLine();
        ResetColors();
        return res;
    }

    /// <summary>
    /// Used to request user input
    /// </summary>
    /// <param name="message">The message to display to the user</param>
    public static string? ReadLine(ReadOnlySpan<ColoredOutput> message) {
        Write(message);
        return baseConsole.ReadLine();
    }

    /// <summary>
    /// Used to request user input
    /// </summary>
    /// <param name="message">The message to display to the user</param>
    /// <param name="inputColor">The color of the input</param>
    /// <returns>The result of the parsing</returns>
    public static string? ReadLine(ReadOnlySpan<ColoredOutput> message, ConsoleColor inputColor)
        => ReadLine(message, inputColor, Color.DefaultBackgroundColor);

    /// <summary>
    /// Used to request user input
    /// </summary>
    /// <param name="message">The message to display to the user</param>
    /// <param name="inputColor">The color of the input</param>
    /// <param name="inputBackgroundColor">The background color of the input</param>
    /// <returns>The result of the parsing</returns>
    public static string? ReadLine(ReadOnlySpan<ColoredOutput> message, ConsoleColor inputColor, ConsoleColor inputBackgroundColor) {
        Write(message);
        baseConsole.ForegroundColor = inputColor;
        baseConsole.BackgroundColor = inputBackgroundColor;
        var res = baseConsole.ReadLine();
        ResetColors();
        return res;
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <returns>The result of the parsing</returns>
    public static T? ReadLine<T>(ReadOnlySpan<ColoredOutput> message) where T : IParsable<T> {
        _ = TryReadLine(message, out T? result);
        return result;
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="default">The default value to return if parsing fails</param>
    /// <returns>The result of the parsing</returns>
    public static T ReadLine<T>(ReadOnlySpan<ColoredOutput> message, T @default) where T : IParsable<T> {
        _ = TryReadLine(message, out T? result);
        return result ?? @default;
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="inputColor">The color of the input</param>
    /// <returns>The result of the parsing</returns>
    public static T? ReadLine<T>(ReadOnlySpan<ColoredOutput> message, ConsoleColor inputColor) where T : IParsable<T> {
        _ = TryReadLine(message, inputColor, out T? result);
        return result;
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="default">The default value to return if parsing fails</param>
    /// <param name="inputColor">The color of the input</param>
    /// <returns>The result of the parsing</returns>
    public static T ReadLine<T>(ReadOnlySpan<ColoredOutput> message, T @default, ConsoleColor inputColor) where T : IParsable<T> {
        _ = TryReadLine(message, inputColor, out T? result);
        return result ?? @default;
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="inputColor">The color of the input</param>
    /// <param name="inputBackgroundColor">The background color of the input</param>
    /// <returns>The result of the parsing</returns>
    public static T? ReadLine<T>(ReadOnlySpan<ColoredOutput> message, ConsoleColor inputColor, ConsoleColor inputBackgroundColor)
        where T : IParsable<T> {
        _ = TryReadLine(message, inputColor, inputBackgroundColor, out T? result);
        return result;
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">The message to display to the user</param>
    /// <param name="default">The default value to return if parsing fails</param>
    /// <param name="inputColor">The color of the input</param>
    /// <param name="inputBackgroundColor">The background color of the input</param>
    /// <returns>The result of the parsing</returns>
    public static T ReadLine<T>(ReadOnlySpan<ColoredOutput> message, T @default, ConsoleColor inputColor, ConsoleColor inputBackgroundColor)
        where T : IParsable<T> {
        _ = TryReadLine(message, inputColor, inputBackgroundColor, out T? result);
        return result ?? @default;
    }
}