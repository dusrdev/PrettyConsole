using System;
namespace PrettyConsole;

/// <summary>
/// This is the model that provides the default color that are used in Console
/// </summary>
public class Colors {
    /// <summary>
    /// The primary color
    /// </summary>
    public ConsoleColor Primary { get; set; } = ConsoleColor.White;
    /// <summary>
    /// The default color
    /// </summary>
    public ConsoleColor Default { get; set; } = ConsoleColor.Gray;
    /// <summary>
    /// The color of the text that is entered by the user when using inputs
    /// </summary>
    public ConsoleColor Input { get; set; } = ConsoleColor.Gray;
    /// <summary>
    /// The color that indicates success
    /// </summary>
    public ConsoleColor Success { get; set; } = ConsoleColor.Green;
    /// <summary>
    /// The color that indicates error
    /// </summary>
    public ConsoleColor Error { get; set; } = ConsoleColor.Red;
    /// <summary>
    /// The color of highlights
    /// </summary>
    public ConsoleColor Highlight { get; set; } = ConsoleColor.Blue;
}
