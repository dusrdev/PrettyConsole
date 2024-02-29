using ogConsole = System.Console;

namespace PrettyConsole;

/// <summary>
/// The static class the provides the abstraction over System.Console
/// </summary>
public static partial class Console {
    // Gets an entire buffer length string full with white-spaces, used to override lines when using the progress-bar
    private static readonly string EmptyLine = new string(' ', ogConsole.BufferWidth);

    // Constant pattern containing the characters needed for the indeterminate progress bar
    private const string Twirl = "-\\|/";

    // A whitespace the length of 10 spaces
    private const string ExtraBuffer = "          ";

    /// <summary>
    /// Resets the console colors to the defaults (foreground:gray, background:black)
    /// </summary>
    public static void ResetColors() {
        ogConsole.ForegroundColor = Color.Gray;
        ogConsole.BackgroundColor = Color.Black;
    }
}
