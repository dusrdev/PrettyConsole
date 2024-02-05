using System;

using ogConsole = System.Console;

namespace PrettyConsole;

/// <summary>
/// The static class the provides the abstraction over System.Console
/// </summary>
public static partial class Console {
    // Used to have the progress bar change size dynamically to the buffer size
    private static readonly int ProgressBarSize = ogConsole.BufferWidth - 10;

    // Gets an entire buffer length string full with white-spaces, used to override lines when using the progress-bar
    private static readonly string EmptyLine = Extensions.GetWhiteSpaces(ogConsole.BufferWidth);

    // Constant pattern containing the characters needed for the indeterminate progress bar
    private const string Twirl = "-\\|/";

    // A whitespace the length of 10 spaces
    private const string ExtraBuffer = "          ";
}
