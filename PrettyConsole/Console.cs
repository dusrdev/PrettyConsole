using ogConsole = System.Console;

namespace PrettyConsole;

/// <summary>
/// The static class the provides the abstraction over System.Console
/// </summary>
public static partial class Console {
    /// <summary>
    /// Resets the console colors to the defaults (foreground:gray, background:black)
    /// </summary>
    public static void ResetColors() {
        ogConsole.ForegroundColor = Color.Gray;
        ogConsole.BackgroundColor = Color.Black;
    }
}
