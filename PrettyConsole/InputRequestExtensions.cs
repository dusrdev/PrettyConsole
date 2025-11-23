namespace PrettyConsole;

/// <summary>
/// Provides methods extending <see cref="Console"/> with input request extensions.
/// </summary>
public static class InputRequestExtensions {
    private static Func<ConsoleKeyInfo> s_readKey = Console.ReadKey;

    /// <summary>
    /// Allows tests to override how Console.ReadKey is performed.
    /// </summary>
    /// <param name="readKey">Delegate that returns a <see cref="ConsoleKeyInfo"/>.</param>
    internal static void ConfigureReadKey(Func<ConsoleKeyInfo>? readKey) {
        s_readKey = readKey ?? Console.ReadKey;
    }

    /// <summary>
    /// Used to get user confirmation with the default values ["y", "yes"]
    /// </summary>
    public static ReadOnlySpan<string> DefaultConfirmValues => new[] { "y", "yes" };

    extension(Console) {
        /// <summary>
        /// Used to wait for user input
        /// </summary>
        /// <param name="handler">Interpolated string handler that streams the content.</param>
        public static void RequestAnyInput([InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) {
            handler.ResetColors();
            _ = s_readKey();
        }

        /// <summary>
        /// Used to get user confirmation with the default values ["y", "yes"] or just pressing enter
        /// </summary>
        /// <param name="handler">Interpolated string handler that streams the content.</param>
        /// <remarks>
        /// It does not display a question mark or any other prompt, only the message
        /// </remarks>
        public static bool Confirm([InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) => Confirm(DefaultConfirmValues, true, handler);

        /// <summary>
        /// Used to get user confirmation
        /// </summary>
        /// <param name="trueValues">a collection of values that indicate positive confirmation</param>
        /// <param name="emptyIsTrue">if simply pressing enter is considered positive or not</param>
        /// <param name="handler">Interpolated string handler that streams the content.</param>
        /// <remarks>
        /// It does not display a question mark or any other prompt, only the message
        /// </remarks>
        public static bool Confirm(ReadOnlySpan<string> trueValues, bool emptyIsTrue = true, [InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) {
            handler.ResetColors();
            var input = PrettyConsoleExtensions.In.ReadLine();
            if (input is null or { Length: 0 }) {
                return emptyIsTrue;
            }

            foreach (var value in trueValues) {
                if (input.Equals(value, StringComparison.OrdinalIgnoreCase)) {
                    return true;
                }
            }

            return false;
        }
    }
}