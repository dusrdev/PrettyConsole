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
        var input = In.ReadLine();
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
        var couldParse = TryReadLine(message, out T? innerResult);
        if (couldParse) {
            result = innerResult!;
            return true;
        }
        result = @default;
        return false;
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
        var input = In.ReadLine();
        var res = Enum.TryParse(input, ignoreCase, out result);
        if (!res) {
            result = @default;
        }
        return res;
    }

    /// <summary>
    /// Used to request user input without any prepended message
    /// </summary>
    /// <remarks>
    /// You can use <see cref="Write(ColoredOutput)"/> or it's overloads in conjunction with this to create more complex input requests.
    /// </remarks>
    public static string? ReadLine() => In.ReadLine();

    /// <summary>
    /// Used to request user input
    /// </summary>
    /// <param name="message">The message to display to the user</param>
    public static string? ReadLine(ReadOnlySpan<ColoredOutput> message) {
        Write(message);
        return ReadLine();
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
        _ = TryReadLine(message, @default, out T result);
        return result;
    }
}