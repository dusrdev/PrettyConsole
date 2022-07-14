using System;

namespace PrettyConsole;

/// <summary>
/// This is the interface that provides the default color that are used in Console
/// </summary>
public interface IColors {
    /// <summary>
    /// The default color
    /// </summary>
    public ConsoleColor Default { get; }
    /// <summary>
    /// The color of the text that is entered by the user when using inputs
    /// </summary>
    public ConsoleColor Input { get; }
    /// <summary>
    /// The color that indicates success
    /// </summary>
    public ConsoleColor Success { get; }
    /// <summary>
    /// The color that indicates error
    /// </summary>
    public ConsoleColor Error { get; }
    /// <summary>
    /// The color of highlights
    /// </summary>
    public ConsoleColor Highlight { get; }
}
