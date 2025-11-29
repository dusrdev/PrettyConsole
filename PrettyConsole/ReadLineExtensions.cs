using System.Globalization;

namespace PrettyConsole;

/// <summary>
/// Provides methods extending the overloads of <see cref="Console.ReadLine()"/>.
/// </summary>
public static class ReadLineExtensions {
    extension(Console) {
        /// <summary>
        /// Used to request user input, validates and converts common types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result of the parsing</param>
        /// <param name="handler">Interpolated string handler that streams the content.</param>
        /// <returns>True if the parsing was successful, false otherwise</returns>
        public static bool TryReadLine<T>(out T? result, [InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) where T : IParsable<T> {
            handler.Flush();
            var input = ConsoleContext.In.ReadLine();
            return T.TryParse(input, CultureInfo.CurrentCulture, out result);
        }

        /// <summary>
        /// Used to request user input, validates and converts common types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result of the parsing</param>
        /// <param name="default">The default value to return if parsing fails</param>
        /// <param name="handler">Interpolated string handler that streams the content.</param>
        /// <returns>True if the parsing was successful, false otherwise</returns>
        public static bool TryReadLine<T>(out T result, T @default, [InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) where T : IParsable<T> {
            var couldParse = TryReadLine(out T? innerResult, handler);
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
        /// <param name="result">The result of the parsing</param>
        /// <param name="ignoreCase">Whether to ignore case when parsing</param>
        /// <param name="handler">Interpolated string handler that streams the content.</param>
        /// <returns>True if the parsing was successful, false otherwise</returns>
        public static bool TryReadLine<TEnum>(out TEnum result, bool ignoreCase, [InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) where TEnum : struct, Enum {
            return TryReadLine(out result, ignoreCase, default, handler);
        }

        /// <summary>
        /// Used to request user input, validates and converts common types.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="result">The result of the parsing</param>
        /// <param name="ignoreCase">Whether to ignore case when parsing</param>
        /// <param name="default">The default value to return if parsing fails</param>
        /// <param name="handler">Interpolated string handler that streams the content.</param>
        /// <returns>True if the parsing was successful, false otherwise</returns>
        public static bool TryReadLine<TEnum>(out TEnum result, bool ignoreCase, TEnum @default, [InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) where TEnum : struct, Enum {
            handler.Flush();
            var input = ConsoleContext.In.ReadLine();
            var res = Enum.TryParse(input, ignoreCase, out result);
            if (!res) {
                result = @default;
            }
            return res;
        }

        /// <summary>
        /// Used to request user input.
        /// </summary>
        /// <param name="handler">Interpolated string handler that streams the content.</param>
        /// <returns>A string if the user entered any, empty string otherwise - never null.</returns>
        [OverloadResolutionPriority(3)]
        public static string ReadLine([InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) {
            _ = TryReadLine(out string result, string.Empty, handler);
            return result;
        }

        /// <summary>
        /// Used to request user input, validates and converts common types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler">Interpolated string handler that streams the content.</param>
        /// <returns>The result of the parsing</returns>
        [OverloadResolutionPriority(2)]
        public static T? ReadLine<T>([InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) where T : IParsable<T> {
            _ = TryReadLine(out T? result, handler);
            return result;
        }

        /// <summary>
        /// Used to request user input, validates and converts common types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="default">The default value to return if parsing fails</param>
        /// <param name="handler">Interpolated string handler that streams the content.</param>
        /// <returns>The result of the parsing</returns>
        [OverloadResolutionPriority(1)]
        public static T ReadLine<T>(T @default, [InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) where T : IParsable<T> {
            _ = TryReadLine(out T result, @default, handler);
            return result;
        }
    }
}