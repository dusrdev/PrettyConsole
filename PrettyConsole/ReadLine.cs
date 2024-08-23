using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    public static bool TryReadLine<T>(ColoredOutput message, out T? result) where T : IParsable<T> {
        Write(message);
        var input = ogConsole.ReadLine();
        return T.TryParse(input, null, out result);
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    public static bool TryReadLine<T>(ColoredOutput message, ConsoleColor inputColor, out T? result)
        where T : IParsable<T>
        => TryReadLine(message, inputColor, UnknownColor, out result);

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    public static bool TryReadLine<T>(ColoredOutput message, ConsoleColor inputColor, ConsoleColor inputBackgroundColor,
        out T? result) where T : IParsable<T> {
        Write(message);
        ogConsole.ForegroundColor = inputColor;
        ogConsole.BackgroundColor = inputBackgroundColor;
        var input = ogConsole.ReadLine();
        var res = T.TryParse(input, null, out result);
        ResetColors();
        return res;
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    public static bool TryReadLine<TEnum>(ColoredOutput message, bool ignoreCase, out TEnum result) where TEnum : struct, Enum {
        Write(message);
        var input = ogConsole.ReadLine();
        return Enum.TryParse(input, ignoreCase, out result);
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    public static bool TryReadLine<TEnum>(ColoredOutput message, ConsoleColor inputColor, bool ignoreCase, out TEnum result)
        where TEnum : struct, Enum
        => TryReadLine(message, inputColor, UnknownColor, ignoreCase, out result);

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    public static bool TryReadLine<TEnum>(ColoredOutput message, ConsoleColor inputColor, ConsoleColor inputBackgroundColor,
        bool ignoreCase, out TEnum result) where TEnum : struct, Enum {
        Write(message);
        ogConsole.ForegroundColor = inputColor;
        ogConsole.BackgroundColor = inputBackgroundColor;
        var input = ogConsole.ReadLine();
        var res = Enum.TryParse(input, ignoreCase, out result);
        ResetColors();
        return res;
    }

    /// <summary>
    /// Used to request user input without any prepended message
    /// </summary>
    /// <remarks>
    /// You can use <see cref="Write(ColoredOutput)"/> or it's overloads in conjunction with this to create more complex input requests.
    /// </remarks>
    public static string? ReadLine() => ogConsole.ReadLine();

    /// <summary>
    /// Used to request user input without any prepended message
    /// </summary>
    /// <remarks>
    /// You can use <see cref="Write(ColoredOutput)"/> or it's overloads in conjunction with this to create more complex input requests.
    /// </remarks>
    public static string? ReadLine(ConsoleColor inputColor) => ReadLine(inputColor, UnknownColor);

    /// <summary>
    /// Used to request user input without any prepended message
    /// </summary>
    /// <remarks>
    /// You can use <see cref="Write(ColoredOutput)"/> or it's overloads in conjunction with this to create more complex input requests.
    /// </remarks>
    public static string? ReadLine(ConsoleColor inputColor, ConsoleColor inputBackgroundColor) {
        ogConsole.ForegroundColor = inputColor;
        ogConsole.BackgroundColor = inputBackgroundColor;
        var res = ogConsole.ReadLine();
        ResetColors();
        return res;
    }

    /// <summary>
    /// Used to request user input
    /// </summary>
    public static string? ReadLine(ColoredOutput message) {
        Write(message);
        return ogConsole.ReadLine();
    }

    /// <summary>
    /// Used to request user input
    /// </summary>
    public static string? ReadLine(ColoredOutput message, ConsoleColor inputColor)
        => ReadLine(message, inputColor, UnknownColor);

    /// <summary>
    /// Used to request user input
    /// </summary>
    public static string? ReadLine(ColoredOutput message, ConsoleColor inputColor, ConsoleColor inputBackgroundColor) {
        Write(message);
        ogConsole.ForegroundColor = inputColor;
        ogConsole.BackgroundColor = inputBackgroundColor;
        var res = ogConsole.ReadLine();
        ResetColors();
        return res;
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    public static T? ReadLine<T>(ColoredOutput message) where T : IParsable<T> {
        _ = TryReadLine(message, out T? result);
        return result;
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    public static T? ReadLine<T>(ColoredOutput message, ConsoleColor inputColor) where T : IParsable<T> {
        _ = TryReadLine(message, inputColor, out T? result);
        return result;
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    public static T? ReadLine<T>(ColoredOutput message, ConsoleColor inputColor, ConsoleColor inputBackgroundColor)
        where T : IParsable<T> {
        _ = TryReadLine(message, inputColor, inputBackgroundColor, out T? result);
        return result;
    }
}