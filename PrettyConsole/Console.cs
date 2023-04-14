using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using PrettyConsole.Models;

using ogConsole = System.Console;

namespace PrettyConsole;

#nullable enable

/// <summary>
/// The static class the provides the abstraction over System.Console
/// </summary>
public static class Console {
    /// <summary>
    /// The default color scheme
    /// </summary>
    /// <remarks>
    /// Modify using ConfigureColors
    /// </remarks>
    public static readonly IColors Colors = new Colors();

    /// <summary>
    /// Used to set the colors which are used by default in most functions of the class
    /// </summary>
    public static void ConfigureColors(Action<Colors> modification) {
        modification.Invoke((Colors)Colors);
    }

    // Used to have the progress bar change size dynamically to the buffer size
    private static readonly int ProgressBarSize = ogConsole.BufferWidth - 10;

    // Gets an entire buffer length string full with whitespaces, used to override lines when using the progressbar
    private static readonly string EmptyLine = Extensions.GetWhiteSpaces(ogConsole.BufferWidth);

    // Constant pattern containing the characters needed for the indeterminate progress bar
    private const string Twirl = "-\\|/";

    // A whitespace the length of 10 spaces
    private const string ExtraBuffer = "          ";

    /// <summary>
    /// Write any object to the console in the default color
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable use regular string overload", Url = "http://help/unreferencedcode")]
    public static void Write<T>(T item) {
        ArgumentNullException.ThrowIfNull(item);
        Write(item.ToString(), Colors.Default);
    }

    /// <summary>
    /// Write a string to the console in the default color
    /// </summary>
    /// <param name="output">Output</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    public static void Write(string? output) {
        Write(output, Colors.Default);
    }

    /// <summary>
    /// Write a string to the console in <paramref name="color"/>
    /// </summary>
    /// <param name="output">content</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void Write(string? output, ConsoleColor color) {
        ogConsole.ResetColor();
        ogConsole.ForegroundColor = color;
        ogConsole.Write(output);
        ogConsole.ResetColor();
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console
    /// </summary>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [Obsolete("Use method with (string,color) tuples!")]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void Write(params (object item, ConsoleColor color)[] elements) {
        if (elements is null || elements.Length is 0) {
            throw new ArgumentException("Invalid parameters");
        }
        ogConsole.ResetColor();
        foreach (var (o, c) in elements) {
            ogConsole.ForegroundColor = c;
            ogConsole.Write(o);
        }
        ogConsole.ResetColor();
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console
    /// </summary>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void Write(params (string item, ConsoleColor color)[] elements) {
        if (elements is null || elements.Length is 0) {
            throw new ArgumentException("Invalid parameters");
        }
        ogConsole.ResetColor();
        foreach (var (o, c) in elements) {
            ogConsole.ForegroundColor = c;
            ogConsole.Write(o);
        }
        ogConsole.ResetColor();
    }

    /// <summary>
    /// Write a string to the error console in Colors.Error
    /// </summary>
    /// <param name="output">content</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLineError</ogConsole> with the same parameters
    /// </remarks>
    public static void WriteError(string? output) {
        WriteError(output, Colors.Error);
    }

    /// <summary>
    /// Write a string to the error console in <paramref name="color"/>
    /// </summary>
    /// <param name="output">content</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLineError</ogConsole> with the same parameters
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void WriteError(string? output, ConsoleColor color) {
        ogConsole.ResetColor();
        ogConsole.ForegroundColor = color;
        ogConsole.Error.Write(output);
        ogConsole.ResetColor();
    }

    /// <summary>
    /// Write any object to the console in the default color and ends line
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <remarks>
    /// <para>
    /// To write without ending line, use <ogConsole>Write</ogConsole> with the same parameters
    /// </para>
    /// <para>
    /// This internally uses Write(string) to avoid boxing
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable use regular string overload", Url = "http://help/unreferencedcode")]
    public static void WriteLine<T>(T item) {
        ArgumentNullException.ThrowIfNull(item);
        WriteLine(item.ToString(), Colors.Default);
    }

    /// <summary>
    /// Write a string to the console in the default color
    /// </summary>
    /// <param name="output">content</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void WriteLine(string? output) {
        WriteLine(output, Colors.Default);
    }

    /// <summary>
    /// Write a string to the console in <paramref name="color"/>
    /// </summary>
    /// <param name="output">content</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void WriteLine(string? output, ConsoleColor color) {
        Write(output, color);
        NewLine();
    }

    /// <summary>
    /// Write any object to the console in <paramref name="color"/> and ends line
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// <para>
    /// To write without ending line, use <ogConsole>Write</ogConsole> with the same parameters
    /// </para>
    /// <para>
    /// This internally uses Write(string) to avoid boxing
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable use regular string overload", Url = "http://help/unreferencedcode")]
    public static void WriteLine<T>(T item, ConsoleColor color) {
        ArgumentNullException.ThrowIfNull(item);
        WriteLine(item.ToString(), color);
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console and ends line
    /// </summary>
    /// <remarks>
    /// To write without ending line, use <ogConsole>Write</ogConsole> with the same parameters
    /// </remarks>
    [Obsolete("Use method with (string,color) tuples!")]
    public static void WriteLine(params (object item, ConsoleColor color)[] elements) {
        Write(elements);
        NewLine();
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console and ends line
    /// </summary>
    /// <remarks>
    /// To write without ending line, use <ogConsole>Write</ogConsole> with the same parameters
    /// </remarks>
    public static void WriteLine(params (string item, ConsoleColor color)[] elements) {
        Write(elements);
        NewLine();
    }

    /// <summary>
    /// Write a string to the error console in the default color and ends line
    /// </summary>
    /// <param name="output">content</param>
    /// <remarks>
    /// <para>
    /// To write without ending line, use <ogConsole>WriteError</ogConsole> with the same parameters
    /// </para>
    /// <para>
    /// This internally uses Write(string) to avoid boxing
    /// </para>
    /// </remarks>
    public static void WriteLineError(string? output) {
        WriteError(output, Colors.Default);
        ogConsole.Error.WriteLine();
    }

    /// <summary>
    /// Write a string to the error console in the Colors.Error and ends line
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <remarks>
    /// <para>
    /// To write without ending line, use <ogConsole>WriteError</ogConsole> with the same parameters
    /// </para>
    /// <para>
    /// This internally uses Write(string) to avoid boxing
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable use regular string overload", Url = "http://help/unreferencedcode")]
    public static void WriteLineError<T>(T item) {
        ArgumentNullException.ThrowIfNull(item);
        WriteError(item.ToString(), Colors.Error);
        ogConsole.Error.WriteLine();
    }

    /// <summary>
    /// Write any object to the error console in <paramref name="color"/> and ends line
    /// </summary>
    /// <param name="output">content</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// <para>
    /// To write without ending line, use <ogConsole>WriteError</ogConsole> with the same parameters
    /// </para>
    /// <para>
    /// This internally uses Write(string) to avoid boxing
    /// </para>
    /// </remarks>
    public static void WriteLineError(string? output, ConsoleColor color) {
        WriteError(output, color);
        ogConsole.Error.WriteLine();
    }

    /// <summary>
    /// Write any object to the error console in <paramref name="color"/> and ends line
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// <para>
    /// To write without ending line, use <ogConsole>WriteError</ogConsole> with the same parameters
    /// </para>
    /// <para>
    /// This internally uses Write(string) to avoid boxing
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable use regular string overload", Url = "http://help/unreferencedcode")]
    public static void WriteLineError<T>(T item, ConsoleColor color) {
        ArgumentNullException.ThrowIfNull(item);
        WriteError(item.ToString(), color);
        ogConsole.Error.WriteLine();
    }

    /// <summary>
    /// Write any object to the console as a label, Colors.Default background - black text
    /// </summary>
    /// <param name="output">content</param>
    /// <remarks>
    /// To end line, call <ogConsole>NewLine()</ogConsole> after.
    /// </remarks>
    public static void Label(string? output) {
        Label(output, ConsoleColor.Black, Colors.Default);
    }

    /// <summary>
    /// Write any object to the console as a label, modified foreground and background
    /// </summary>
    /// <param name="output">content</param>
    /// <param name="foreground">foreground color - i.e: color of the string representation</param>
    /// <param name="background">background color</param>
    /// <remarks>
    /// To end line, call <ogConsole>NewLine()</ogConsole> after.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void Label(string? output, ConsoleColor foreground, ConsoleColor background) {
        ogConsole.ResetColor();
        ogConsole.ForegroundColor = foreground;
        ogConsole.BackgroundColor = background;
        ogConsole.Write(output);
        ogConsole.ResetColor();
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console
    /// </summary>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [Obsolete("Use method with (string,foreground,background) tuples!")]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void Label(params (object item, ConsoleColor foreground, ConsoleColor background)[] elements) {
        if (elements is null || elements.Length is 0) {
            throw new ArgumentException("Invalid parameters");
        }
        ogConsole.ResetColor();
        foreach (var (o, foreground, background) in elements) {
            ogConsole.ForegroundColor = foreground;
            ogConsole.BackgroundColor = background;
            ogConsole.Write(o);
        }
        ogConsole.ResetColor();
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console
    /// </summary>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void Label(params (string item, ConsoleColor foreground, ConsoleColor background)[] elements) {
        if (elements is null || elements.Length is 0) {
            throw new ArgumentException("Invalid parameters");
        }
        ogConsole.ResetColor();
        foreach (var (o, foreground, background) in elements) {
            ogConsole.ForegroundColor = foreground;
            ogConsole.BackgroundColor = background;
            ogConsole.Write(o);
        }
        ogConsole.ResetColor();
    }

    /// <summary>
    /// Write any object to the error console as a label, modified foreground and background
    /// </summary>
    /// <param name="output">content</param>
    /// <param name="foreground">foreground color - i.e: color of the string representation</param>
    /// <param name="background">background color</param>
    /// <remarks>
    /// To end line, call <ogConsole>NewLine()</ogConsole> after.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void ErrorLabel(string? output, ConsoleColor foreground, ConsoleColor background) {
        ogConsole.ResetColor();
        ogConsole.ForegroundColor = foreground;
        ogConsole.BackgroundColor = background;
        ogConsole.Error.Write(output);
        ogConsole.ResetColor();
    }

    /// <summary>
    /// Used to wait for user input, you can customize <paramref name="message"/> or leave as default
    /// </summary>
    /// <param name="message"><ogConsole>Default value:</ogConsole> "Press any key to continue"</param>
    public static void RequestAnyInput(string message = "Press any key to continue") {
        Write((message, Colors.Default), ("... ", Colors.Highlight));
        ogConsole.ForegroundColor = Colors.Input;
        _ = ogConsole.In.Read();
        ogConsole.ForegroundColor = Colors.Default;
        ogConsole.ResetColor();
    }

    /// <summary>
    /// Used to get user confirmation
    /// </summary>
    /// <param name="message"></param>
    /// <remarks>
    /// The user can confirm by entering <ogConsole>"y"</ogConsole>/<ogConsole>"yes"</ogConsole> or just pressing <ogConsole>enter</ogConsole>, anything else is regarded as <c>false</c>.
    /// </remarks>
    [Pure]
    public static bool Confirm(string message) {
        Write((message, Colors.Default), ("? ", Colors.Highlight), ("[", Colors.Default), ("y", Colors.Success),
            ("/", Colors.Default), ("n", Colors.Error), ("]: ", Colors.Default));
        ogConsole.ForegroundColor = Colors.Input;
        var input = ogConsole.ReadLine();
        ogConsole.ResetColor();
        return string.IsNullOrEmpty(input) || input.ToLower() is "y" or "yes";
    }

    /// <summary>
    /// Enumerates a list of strings and allows the user to select one by number
    /// </summary>
    /// <param name="title"><ogConsole>Optional</ogConsole>, null or whitespace will not be displayed</param>
    /// <param name="choices">Any collection of strings</param>
    /// <returns>The selected string, or null if the choice was invalid.</returns>
    /// <remarks>
    /// This validates the input for you.
    /// </remarks>
    [Pure]
    public static string? Selection(string title, IEnumerable<string> choices) {
        if (!Extensions.IsEmptyOrWhiteSpace(title)) {
            WriteLine(title, Colors.Highlight);
        }
        Dictionary<int, string> dict = new();
        var i = 1;
        // Enumerate list with numbers to allow selection by index
        foreach (var choice in choices) {
            WriteLine((string.Concat("\t", i.ToString()), Colors.Highlight), (string.Concat(". ", choice), Colors.Default));
            dict.Add(i, choice);
            i++;
        }
        NewLine();

        var selected = ReadLine<int>("Enter your choice: ", typeof(int));

        return !dict.TryGetValue(selected, out var value) ? null : value;
    }

    /// <summary>
    /// Enumerates a list of strings and allows the user to select multiple strings by any order
    /// </summary>
    /// <param name="title"><ogConsole>Optional</ogConsole>, null or whitespace will not be displayed</param>
    /// <param name="choices">Any collection of strings</param>
    /// <returns>A list containing any selected choices by order of selection, or default if choice is invalid</returns>
    /// <remarks>
    /// This validates the input for you.
    /// </remarks>
    [Pure]
    public static List<string>? MultiSelection(string title, IEnumerable<string> choices) {
        if (!Extensions.IsEmptyOrWhiteSpace(title)) {
            WriteLine(title, Colors.Highlight);
        }
        Dictionary<int, string> dict = new();
        var i = 1;
        // Enumerate list of choices
        foreach (var choice in choices) {
            WriteLine((string.Concat("\t", i.ToString()), Colors.Highlight), (string.Concat(". ", choice), Colors.Default));
            dict.Add(i, choice);
            i++;
        }
        List<string> results = new();

        NewLine();
        var input = ReadLine("Enter your choices separated with spaces: ");

        // evaluate and add selections to results
        foreach (var choice in Extensions.Split(input, ' ')) {
            if (!int.TryParse(choice, out var num)) {
                throw new ArgumentException(nameof(choice));
            }

            if (!dict.TryGetValue(num, out var c)) {
                return default;
            }
            results.Add(c);
        }

        ogConsole.ResetColor();
        return results;
    }

    /// <summary>
    /// Enumerates a menu containing main option as well as sub options and allows the user to select both.
    /// <para>
    /// * This function is great where more options or categories are required than <ogConsole>Selection</ogConsole> can provide.
    /// </para>
    /// </summary>
    /// <param name="title"><ogConsole>Optional</ogConsole>, null or whitespace will not be displayed</param>
    /// <param name="menu">A nested dictionary containing menu titles</param>
    /// <returns>The selected main option and selected sub option</returns>
    /// <remarks>
    /// This validates the input for you.
    /// </remarks>
    [Pure]
    public static (string option, string subOption) TreeMenu(string title, Dictionary<string, List<string>> menu) {
        if (!Extensions.IsEmptyOrWhiteSpace(title)) {
            WriteLine(title, Colors.Highlight);
            NewLine();
        }
        var menuKeys = menu.Keys.ToList();
        var maxMainOption =
            menuKeys
            .Max(static x => x.Length); // Used to make sub-tree prefix spaces uniform
        var dict = new Dictionary<int, List<int>>();
        maxMainOption += 10;
        int i = 1, j = 1;

        //Enumerate options and sub-options
        foreach (var (mainChoice, subChoices) in menu) {
            var lst = new List<int>();
            var prefixLength = i.ToString().Length + 2;
            Write((i.ToString(), Colors.Highlight), (string.Concat(". ", Extensions.SuffixWithSpaces(mainChoice, maxMainOption - prefixLength)),
                Colors.Default));
            foreach (var subChoice in subChoices) {
                lst.Add(j);
                if (j is 1) {
                    WriteLine((j.ToString(), Colors.Highlight), (string.Concat(". ", subChoice), Colors.Default));
                } else {
                    WriteLine((string.Concat(Extensions.SuffixWithSpaces(null, maxMainOption), j), Colors.Highlight), (string.Concat(". ", subChoice),
                        Colors.Default));
                }
                j++;
            }
            dict.Add(i, lst);
            j = 1;
            i++;
            NewLine();
        }

        var input = ReadLine("Enter your main choice and sub choice separated with space: ");

        var selected = Extensions.Split(input, ' ').Take(2).ToArray();

        var (sub, main) = (selected[0], selected[1]);

        // Validate
        if (!int.TryParse(main, out var mainNum) || !dict.ContainsKey(mainNum)) {
            throw new ArgumentException(nameof(mainNum));
        }

        if (!int.TryParse(sub, out var subNum) || !dict[mainNum].Contains(subNum)) {
            throw new ArgumentException(nameof(subNum));
        }

        ogConsole.ResetColor();

        var selectedMainOption = menuKeys[mainNum - 1];
        var selectedSubOption = menu[selectedMainOption][subNum - 1];
        return (selectedMainOption, selectedSubOption);
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="type"></param>
    /// <param name="outputColor"></param>
    /// <param name="inputColor"></param>
    /// <returns>Converted input</returns>
    /// <remarks>
    /// For complex types request a string and validate/convert yourself
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static T? ReadLine<T>(string message, Type type, ConsoleColor outputColor, ConsoleColor inputColor) {
        var input = ReadLine(message, outputColor, inputColor);
        return (T?)Convert.ChangeType(input, type);
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="type"></param>
    /// <param name="inputColor"></param>
    /// <returns>Converted input</returns>
    /// <remarks>
    /// For complex types request a string and validate/convert yourself
    /// </remarks>
    [Pure]
    public static T? ReadLine<T>(string message, Type type, ConsoleColor inputColor) {
        return ReadLine<T?>(message, type, Colors.Default, inputColor);
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="type"></param>
    /// <returns>Converted input</returns>
    /// <remarks>
    /// For complex types request a string and validate/convert yourself
    /// </remarks>
    [Pure]
    public static T? ReadLine<T>(string message, Type type) {
        return ReadLine<T?>(message, type, Colors.Default, Colors.Input);
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T">Any common type</typeparam>
    /// <param name="message">Request title</param>
    /// <param name="outputColor"></param>
    /// <param name="inputColor"></param>
    /// <returns>Converted input</returns>
    /// <remarks>
    /// For complex types request a string and validate/convert yourself
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable add the output type or use string overload instead", Url = "http://help/unreferencedcode")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static T? ReadLine<T>(string message, ConsoleColor outputColor, ConsoleColor inputColor) {
        var input = ReadLine(message, outputColor, inputColor);

        ArgumentNullException.ThrowIfNull(input);

        // Convert input to desired type
        var converter = TypeDescriptor.GetConverter(typeof(T));
        return (T?)converter.ConvertFromString(input!);
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T">Any common type</typeparam>
    /// <param name="message">Request title</param>
    /// <param name="inputColor"></param>
    /// <returns>Converted input</returns>
    /// <remarks>
    /// For complex types request a string and validate/convert yourself
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable add the output type or use string overload instead", Url = "http://help/unreferencedcode")]
    [Pure]
    public static T? ReadLine<T>(string message, ConsoleColor inputColor) {
        return ReadLine<T?>(message, Colors.Default, inputColor);
    }

    /// <summary>
    /// Used to request user input, validates and converts common types.
    /// </summary>
    /// <typeparam name="T">Any common type</typeparam>
    /// <param name="message">Request title</param>
    /// <returns>Converted input</returns>
    /// <remarks>
    /// For complex types request a string and validate/convert yourself
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable add the output type or use string overload instead", Url = "http://help/unreferencedcode")]
    [Pure]
    public static T? ReadLine<T>(string message) {
        return ReadLine<T?>(message, Colors.Default, Colors.Input);
    }

    /// <summary>
    /// Used to request user input
    /// </summary>
    /// <param name="message">Request title</param>
    /// <param name="outputColor"></param>
    /// <param name="inputColor"></param>
    /// <returns>Trimmed string, or empty if the input was empty</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static string ReadLine(string message, ConsoleColor outputColor, ConsoleColor inputColor) {
        Write(message, outputColor);
        ogConsole.ForegroundColor = inputColor;
        var input = ogConsole.ReadLine();
        ogConsole.ResetColor();

        if (string.IsNullOrWhiteSpace(input)) {
            return string.Empty;
        }

        return input.Trim();
    }

    /// <summary>
    /// Used to request user input
    /// </summary>
    /// <param name="message">Request title</param>
    /// <param name="inputColor"></param>
    /// <returns>Trimmed string</returns>
    [Pure]
    public static string ReadLine(string message, ConsoleColor inputColor) {
        return ReadLine(message, Colors.Default, inputColor);
    }

    /// <summary>
    /// Used to request user input
    /// </summary>
    /// <param name="message">Request title</param>
    /// <returns>Trimmed string</returns>
    public static string ReadLine(string message) {
        return ReadLine(message, Colors.Default, Colors.Input);
    }

    /// <summary>
    /// A simple twirl style indeterminate progress bar to signal the user that the app is not stuck but rather is performing a time consuming task.
    /// <para>
    /// The output is cleared so the next line will be written on the same line as the progress bar was.
    /// </para>
    /// </summary>
    /// <param name="task">The task you want to await on, it will not be modified, only the state is observed</param>
    /// <param name="color">The color in which to display the progress bar</param>
    /// <param name="title">Message to display alongside the progress bar</param>
    /// <param name="displayElapsedTime">Display elapsed time</param>
    /// <param name="updateRate">Rate at which the progress bar refreshes in milliseconds</param>
    /// <param name="token">So you can cancel the progress bar and end it any time</param>
    /// <remarks>
    /// <para>The cancellation token parameter is to be used if you want to cancel the progress bar and end it any time.</para>
    /// <para>It can also be used when you to display it while non-task actions are running, simply set the task to Task.Delay(-1) and cancel with the token when you want to</para>
    /// </remarks>
    [RequiresUnreferencedCode("If trimming is unavoidable try the regular task overload", Url = "http://help/unreferencedcode")]
    [Pure]
    public static async Task<T> IndeterminateProgressBar<T>(Task<T> task, ConsoleColor color, string title, bool displayElapsedTime, int updateRate = 50, CancellationToken token = default) {
        try {
            if (task.Status is not TaskStatus.Running) {
                task.Start();
            }
        } catch {
            //ignore
        }

        ogConsole.ResetColor();
        ogConsole.ForegroundColor = color;
        Stopwatch? stopwatch = null;
        if (displayElapsedTime) {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }
        var lineNum = ogConsole.CursorTop;

        title = string.IsNullOrWhiteSpace(title) ? string.Empty : string.Concat(title, " ");

        while (!task.IsCompleted) { // Await until the TaskAwaiter informs of completion
            foreach (char c in Twirl) { // Cycle through the characters of twirl
                if (displayElapsedTime) {
                    ogConsole.Write($"{title}{c} [Elapsed: {stopwatch!.Elapsed.ToFriendlyString()}]{ExtraBuffer}"); // Remove last character and re-write
                } else {
                    ogConsole.Write($"{title}{c}{ExtraBuffer}"); // Remove last character and re-write
                }
                ogConsole.SetCursorPosition(0, lineNum);
                await Task.Delay(updateRate, token); // The update rate
                ogConsole.Write(EmptyLine);
                ogConsole.SetCursorPosition(0, lineNum);
                if (token.IsCancellationRequested) {
                    ogConsole.Write(EmptyLine);
                    ogConsole.SetCursorPosition(0, lineNum);
                    return await task;
                }
            }
        }

        stopwatch?.Stop();

        NewLine(); // Break line after completion

        ogConsole.ResetColor();

        return await task;
    }

    /// <summary>
    /// A simple twirl style indeterminate progress bar to signal the user that the app is not stuck but rather is performing a time consuming task.
    /// <para>
    /// The output is cleared so the next line will be written on the same line as the progress bar was.
    /// </para>
    /// </summary>
    /// <param name="task">The task you want to await on, it will not be modified, only the state is observed</param>
    /// <param name="color">The color in which to display the progress bar</param>
    /// <param name="title">Message to display alongside the progress bar</param>
    /// <param name="displayElapsedTime">Display elapsed time</param>
    /// <param name="updateRate">Rate at which the progress bar refreshes in milliseconds</param>
    /// <param name="token">So you can cancel the progress bar and end it any time</param>
    /// <remarks>
    /// <para>The cancellation token parameter is to be used if you want to cancel the progress bar and end it any time.</para>
    /// <para>It can also be used when you to display it while non-task actions are running, simply set the task to Task.Delay(-1) and cancel with the token when you want to</para>
    /// </remarks>
    public static async Task IndeterminateProgressBar(Task task, ConsoleColor color, string title, bool displayElapsedTime, int updateRate = 50, CancellationToken token = default) {
        try {
            if (task.Status is not TaskStatus.Running) {
                task.Start();
            }
        } catch {
            //ignore
        }

        ogConsole.ResetColor();
        ogConsole.ForegroundColor = color;
        Stopwatch? stopwatch = null;
        if (displayElapsedTime) {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }
        var lineNum = ogConsole.CursorTop;

        title = string.IsNullOrWhiteSpace(title) ? string.Empty : string.Concat(title, " ");

        while (!task.IsCompleted) { // Await until the TaskAwaiter informs of completion
            foreach (char c in Twirl) { // Cycle through the characters of twirl
                if (displayElapsedTime) {
                    ogConsole.Write($"{title}{c} [Elapsed: {stopwatch!.Elapsed.ToFriendlyString()}]{ExtraBuffer}"); // Remove last character and re-write
                } else {
                    ogConsole.Write(string.Concat(title, c.ToString(), ExtraBuffer)); // Remove last character and re-write
                }
                ogConsole.SetCursorPosition(0, lineNum);
                await Task.Delay(updateRate, token); // The update rate
                ogConsole.Write(EmptyLine);
                ogConsole.SetCursorPosition(0, lineNum);
                if (token.IsCancellationRequested) {
                    ogConsole.Write(EmptyLine);
                    ogConsole.SetCursorPosition(0, lineNum);
                    return;
                }
            }
        }

        stopwatch?.Stop();

        NewLine(); // Break line after completion

        ogConsole.ResetColor();
    }

    /// <summary>
    /// Outputs progress bar filled according to <paramref name="percent"/>
    /// <para>
    /// When called consecutively, it overrides the previous
    /// </para>
    /// </summary>
    /// <param name="percent"></param>
    public static void UpdateProgressBar(int percent) {
        UpdateProgressBar(percent, Colors.Default);
    }

    /// <summary>
    /// Outputs progress bar filled according to <paramref name="percent"/>
    /// <para>
    /// When called consecutively, it overrides the previous
    /// </para>
    /// </summary>
    /// <param name="percent"></param>
    /// <param name="color">The color you want the progress bar to be</param>
    public static void UpdateProgressBar(int percent, ConsoleColor color) {
        UpdateProgressBar(percent, color, color);
    }

    /// <summary>
    /// Outputs progress bar filled according to <paramref name="percent"/>
    /// <para>
    /// When called consecutively, it overrides the previous
    /// </para>
    /// </summary>
    /// <param name="percent"></param>
    /// <param name="foreground">color of the bounds and percentage</param>
    /// <param name="progress">color of the progress bar fill</param>
    public static void UpdateProgressBar(int percent, ConsoleColor foreground, ConsoleColor progress) {
        ogConsole.ResetColor();
        ogConsole.ForegroundColor = foreground;
        var currentLine = ogConsole.CursorTop;
        ogConsole.SetCursorPosition(0, currentLine);
        ogConsole.WriteLine(EmptyLine);
        ogConsole.SetCursorPosition(0, currentLine);
        ogConsole.Write("[");
        var p = (int)(ProgressBarSize * percent * 0.01);
        ogConsole.ForegroundColor = progress;
        var full = new string('■', p);
        var empty = new string(' ', ProgressBarSize - p);
        ogConsole.Write(full);
        ogConsole.Write(empty);
        ogConsole.ForegroundColor = foreground;
        ogConsole.Write("] {0,5:##0.##}%", percent);
        ogConsole.SetCursorPosition(0, currentLine);
        ogConsole.ResetColor();
    }

    /// <summary>
    /// Outputs progress bar filled according to <paramref name="display"/>
    /// <para>
    /// When called consecutively, it overrides the previous
    /// </para>
    /// <para>Make sure to print a new line after the last call, otherwise your outputs will override the progress bar</para>
    /// </summary>
    /// <remarks>
    /// <para>Test before usage in release, when updated too quickly, the progress bar may fail to override previous lines and will make a mess</para>
    /// <para>If that happens, consider restricting the updates yourself by wrapping the call</para>
    /// <para>If the progress bar is interrupted, you clear the used lines with ClearNextLines()</para>
    /// </remarks>
    /// <param name="display"></param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void UpdateProgressBar(ProgressBarDisplay display) {
        ogConsole.ResetColor();
        ogConsole.ForegroundColor = display.Foreground;
        var currentLine = ogConsole.CursorTop;
        ogConsole.SetCursorPosition(0, currentLine);
        ogConsole.Write(EmptyLine);
        ogConsole.Write(EmptyLine);
        ogConsole.SetCursorPosition(0, currentLine);
        if (!string.IsNullOrWhiteSpace(display.Header)) {
            ogConsole.WriteLine(display.Header);
        }
        ogConsole.Write("[");
        var p = (int)(ProgressBarSize * display.Percentage * 0.01);
        ogConsole.ForegroundColor = display.Progress;
        var full = new string('■', p);
        var empty = new string(' ', ProgressBarSize - p);
        ogConsole.Write(full);
        ogConsole.Write(empty);
        ogConsole.ForegroundColor = display.Foreground;
        ogConsole.Write("] {0,5:##0.##}%", display.Percentage);
        ogConsole.SetCursorPosition(0, currentLine);
        ogConsole.ResetColor();
    }

    /// <summary>
    /// Clears the current line and overrides it with <paramref name="output"/>
    /// </summary>
    /// <param name="output"></param>
    public static void OverrideCurrentLine(string output) {
        OverrideCurrentLine(output, Colors.Default);
    }

    /// <summary>
    /// Clears the current line and overrides it with <paramref name="output"/>
    /// </summary>
    /// <param name="output"></param>
    /// <param name="color">foreground color (i.e text color)</param>
    public static void OverrideCurrentLine(string output, ConsoleColor color) {
        ogConsole.ResetColor();
        ogConsole.ForegroundColor = color;
        var currentLine = ogConsole.CursorTop;
        ogConsole.SetCursorPosition(0, currentLine);
        ogConsole.Write(EmptyLine);
        ogConsole.SetCursorPosition(0, currentLine);
        ogConsole.Write(output);
        ogConsole.ResetColor();
    }

    /// <summary>
    /// Clears the next <paramref name="lines"/>
    /// </summary>
    /// <param name="lines">Amount of lines to clear</param>
    /// <remarks>
    /// Useful for clearing output of overriding functions, like the ProgressBar
    /// </remarks>
    public static void ClearNextLines(int lines) {
        ogConsole.ResetColor();
        var currentLine = ogConsole.CursorTop;
        ogConsole.SetCursorPosition(0, currentLine);
        var emptyLines = new string(' ', lines * ogConsole.BufferWidth);
        ogConsole.Write(emptyLines);
        ogConsole.SetCursorPosition(0, currentLine);
    }

    /// <summary>
    /// Used to clear all previous outputs to the console
    /// </summary>
    public static void Clear() {
        ogConsole.Clear();
    }

    /// <summary>
    /// Used to end current line or write an empty one, depends whether the current line has any text
    /// </summary>
    public static void NewLine() {
        ogConsole.WriteLine();
    }
}
