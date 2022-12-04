using System;

namespace PrettyConsole.Models;

/// <summary>
/// Used as input parameter in an overload of the ProgressBar method.
/// </summary>
public readonly ref struct ProgressBarDisplay {
    /// <summary>
    /// Header text -> this is where you could write an updatable status message.
    /// </summary>
    public readonly string Header { get; init; }

    /// <summary>
    /// The current progress value.
    /// </summary>
    /// <remarks>Recommended to round up to 2 digits</remarks>
    public required readonly double Percentage { get; init; }

    /// <summary>
    /// The color of the header, progress bar bounds and percentage
    /// </summary>
    public readonly ConsoleColor Foreground { get; init; }

    /// <summary>
    /// The color of the filled progress bar
    /// </summary>
    public readonly ConsoleColor Progress { get; init; }
}
