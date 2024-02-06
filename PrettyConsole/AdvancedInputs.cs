using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Used to wait for user input
    /// </summary>
    public static void RequestAnyInput(string message = "Press any key to continue...") => RequestAnyInput(new ColoredOutput(message));

    /// <summary>
    /// Used to wait for user input
    /// </summary>
    public static void RequestAnyInput(ColoredOutput output) {
        Write(output);
        _ = ogConsole.ReadKey();
    }

    private static ReadOnlySpan<string> DefaultConfirmValues => new[] { "y", "yes" };

    /// <summary>
    /// Used to get user confirmation with the default values ["y", "yes"] or just pressing enter
    /// </summary>
    /// <remarks>
    /// It does not display a question mark or any other prompt, only the message
    /// </remarks>
    public static bool Confirm(ColoredOutput message) => Confirm(message, DefaultConfirmValues);

    /// <summary>
    /// Used to get user confirmation
    /// </summary>
    /// <param name="message"></param>
    /// <param name="trueValues">a collection of values that indicate positive confirmation</param>
    /// <param name="emptyIsTrue">if simply pressing enter is considered positive or not</param>
    /// <remarks>
    /// It does not display a question mark or any other prompt, only the message
    /// </remarks>
    public static bool Confirm(ColoredOutput message, ReadOnlySpan<string> trueValues, bool emptyIsTrue = true) {
        try {
            Write(message);
            var input = ogConsole.ReadLine().AsSpan();
            if (input.Length is 0) {
                return emptyIsTrue;
            }
            foreach (var value in trueValues) {
                if (input.Equals(value, StringComparison.InvariantCultureIgnoreCase)) {
                    return true;
                }
            }
            return false;
        } finally {
            ResetColors();
        }
    }
}