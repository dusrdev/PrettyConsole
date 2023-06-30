using System;

namespace PrettyConsole.Models;

/// <summary>
/// Represents a scheme for rendering text with different colors in a console application.
/// </summary>
/// <remarks>
/// A <see cref="TextRenderingScheme"/> is defined by a collection of <see cref="Segments"/>,
/// where each segment is a tuple of a string and a <see cref="ConsoleColor"/> value.
/// </remarks>
public record TextRenderingScheme(params (string Text, ConsoleColor Color)[] Segments);