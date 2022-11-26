using System;
using System.Collections.Generic;
using System.ComponentModel;
using ogConsole = System.Console;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using PrettyConsole.Models;

namespace PrettyConsole;
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

    // Gets an entire bufferlength string full with whitespaces, used to override lines when using the progressbar
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
    public static void Write<T>(T item) {
        Write(item, Colors.Default);
    }

    /// <summary>
    /// Write any object to the console in the default color
    /// </summary>
    /// <param name="output">Output</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    public static void Write(string output) {
        Write(output, Colors.Default);
    }

    /// <summary>
    /// Write any object to the console in <paramref name="color"/>
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void Write<T>(T item, ConsoleColor color) {
        //lock (_lock) {
        ogConsole.ResetColor();
        ogConsole.ForegroundColor = color;
        if (item is string output) {
            ogConsole.Write(output);
        } else {
            ogConsole.Write(item.ToString());
        }
        ogConsole.ResetColor();
        //}
    }

    /// <summary>
    /// Write any object to the error console in Colors.Error
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLineError</ogConsole> with the same parameters
    /// </remarks>
    public static void WriteError<T>(T item) {
        WriteError(item.ToString(), Colors.Error);
    }

    /// <summary>
    /// Write any object to the error console in <paramref name="color"/>
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <param name="color">The color in which the output will be displayed</param>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLineError</ogConsole> with the same parameters
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void WriteError<T>(T item, ConsoleColor color) {
        //lock (_lock) {
        ogConsole.ResetColor();
        ogConsole.ForegroundColor = color;
        if (item is string output) {
            ogConsole.Error.Write(output);
        } else {
            ogConsole.Error.Write(item.ToString());
        }
        ogConsole.ResetColor();
        //}
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
    public static void WriteLine<T>(T item) {
        Write(item.ToString(), Colors.Default);
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
    public static void WriteLine<T>(T item, ConsoleColor color) {
        Write(item.ToString(), color);
        NewLine();
    }

    /// <summary>
    /// Write any object to the error console in the Colors.Error and ends line
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
    public static void WriteLineError<T>(T item) {
        WriteError(item.ToString(), Colors.Error);
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
    public static void WriteLineError<T>(T item, ConsoleColor color) {
        WriteError(item.ToString(), color);
        ogConsole.Error.WriteLine();
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
        //lock (_lock) {
        if (elements is null || elements.Length is 0) {
            throw new ArgumentException("Invalid parameters");
        }
        ogConsole.ResetColor();
        foreach (var (o, c) in elements) {
            ogConsole.ForegroundColor = c;
            ogConsole.Write(o);
        }
        ogConsole.ResetColor();
        //}
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console
    /// </summary>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void Write(params (string item, ConsoleColor color)[] elements) {
        //lock (_lock) {
        if (elements is null || elements.Length is 0) {
            throw new ArgumentException("Invalid parameters");
        }
        ogConsole.ResetColor();
        foreach (var (o, c) in elements) {
            ogConsole.ForegroundColor = c;
            ogConsole.Write(o);
        }
        ogConsole.ResetColor();
        //}
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
    /// Write any object to the console as a label, Colors.Default background - black text
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <remarks>
    /// To end line, call <ogConsole>NewLine()</ogConsole> after.
    /// </remarks>
    public static void Label<T>(T item) {
        Label(item, ConsoleColor.Black, Colors.Default);
    }

    /// <summary>
    /// Write any object to the console as a label, modified foreground and background
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <param name="foreground">foreground color - i.e: color of the string representation</param>
    /// <param name="background">background color</param>
    /// <remarks>
    /// To end line, call <ogConsole>NewLine()</ogConsole> after.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void Label<T>(T item, ConsoleColor foreground, ConsoleColor background) {
        //lock (_lock) {
        ogConsole.ResetColor();
        ogConsole.ForegroundColor = foreground;
        ogConsole.BackgroundColor = background;
        if (item is string output) {
            ogConsole.Write(output);
        } else {
            ogConsole.Write(item.ToString());
        }
        ogConsole.ResetColor();
        //}
    }

    /// <summary>
    /// Write any object to the error console as a label, modified foreground and background
    /// </summary>
    /// <param name="item">The item which value to write to the console</param>
    /// <param name="foreground">foreground color - i.e: color of the string representation</param>
    /// <param name="background">background color</param>
    /// <remarks>
    /// To end line, call <ogConsole>NewLine()</ogConsole> after.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void ErrorLabel<T>(T item, ConsoleColor foreground, ConsoleColor background) {
        //lock (_lock) {
        ogConsole.ResetColor();
        ogConsole.ForegroundColor = foreground;
        ogConsole.BackgroundColor = background;
        if (item is string output) {
            ogConsole.Error.Write(output);
        } else {
            ogConsole.Error.Write(item.ToString());
        }
        ogConsole.ResetColor();
        //}
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
        //lock (_lock) {
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
        //}
    }

    /// <summary>
    /// Write tuples of (<ogConsole>element</ogConsole>, <ogConsole>color</ogConsole>) to the console
    /// </summary>
    /// <remarks>
    /// To end line, use <ogConsole>WriteLine</ogConsole> with the same parameters
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
    public static void Label(params (string item, ConsoleColor foreground, ConsoleColor background)[] elements) {
        //lock (_lock) {
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
        //}
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
    public static bool Confirm(string message) {
        Write((message, Colors.Default), ("? ", Colors.Highlight), ("[", Colors.Default), ("y", Colors.Success),
            ("/", Colors.Default), ("n", Colors.Error), ("]: ", Colors.Default)); ;
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
    /// <returns>The selected string</returns>
    /// <remarks>
    /// This validates the input for you.
    /// </remarks>
    public static string Selection(string title, IEnumerable<string> choices) {
        if (!Extensions.IsEmptyOrWhiteSpace(title)) {
            WriteLine(title, Colors.Highlight);
        }
        Dictionary<int, string> dict = new();
        var i = 1;
        // Enumerate list with numbers to allow selection by index
        foreach (var choice in choices) {
            WriteLine(($"\t{i}", Colors.Highlight), ($". {choice}", Colors.Default));
            dict.Add(i, choice);
            i++;
        }
        NewLine();

        var selected = int.Parse(ReadLine("Enter your choice:"));

        if (!dict.ContainsKey(selected)) {
            throw new IndexOutOfRangeException(nameof(selected));
        }

        return dict[selected];
    }

    /// <summary>
    /// Enumerates a list of strings and allows the user to select multiple strings by any order
    /// </summary>
    /// <param name="title"><ogConsole>Optional</ogConsole>, null or whitespace will not be displayed</param>
    /// <param name="choices">Any collection of strings</param>
    /// <returns>A list containing any selected choices by order of selection</returns>
    /// <remarks>
    /// This validates the input for you.
    /// </remarks>
    public static List<string> MultiSelection(string title, IEnumerable<string> choices) {
        if (!Extensions.IsEmptyOrWhiteSpace(title)) {
            WriteLine(title, Colors.Highlight);
        }
        Dictionary<int, string> dict = new();
        var i = 1;
        // Enumerate list of choices
        foreach (var choice in choices) {
            WriteLine(($"\t{i}", Colors.Highlight), ($". {choice}", Colors.Default));
            dict.Add(i, choice);
            i++;
        }
        List<string> results = new();

        NewLine();
        var input = ReadLine("Enter your choices separated with spaces:");

        // get selected indexes from user
        var selected = Extensions.Split(input, ' ');

        // evaluate and add selections to results
        foreach (var choice in selected) {
            if (!int.TryParse(choice, out var num)) {
                throw new ArgumentException(nameof(choice));
            }
            if (!dict.ContainsKey(num)) {
                throw new IndexOutOfRangeException(nameof(choice));
            }
            results.Add(dict[num]);
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
    public static (string option, string subOption) TreeMenu(string title, Dictionary<string, List<string>> menu) {
        if (!Extensions.IsEmptyOrWhiteSpace(title)) {
            WriteLine(title, Colors.Highlight);
            NewLine();
        }
        var maxMainOption = menu.Keys.ToList().Max(static x => x.Length); // Used to make sub-tree prefix spaces uniform
        var Dict = new Dictionary<int, List<int>>();
        maxMainOption += 10;
        int i = 1, j = 1;

        //Enumerate options and sub-options
        foreach (var (mainChoice, subChoices) in menu) {
            var lst = new List<int>();
            var prefixLength = i.ToString().Length + 2;
            Write(($"{i}", Colors.Highlight), ($". {Extensions.SuffixWithSpaces(mainChoice, maxMainOption - prefixLength)}",
                Colors.Default));
            foreach (var subChoice in subChoices) {
                lst.Add(j);
                if (j is 1) {
                    WriteLine(($"{j}", Colors.Highlight), ($". {subChoice}", Colors.Default));
                } else {
                    WriteLine(($"{Extensions.SuffixWithSpaces(null, maxMainOption)}{j}", Colors.Highlight), ($". {subChoice}",
                        Colors.Default));
                }
                j++;
            }
            Dict.Add(i, lst);
            j = 1;
            i++;
            NewLine();
        }

        var input = ReadLine("Enter your choice separated with spaces:");

        var selected = Extensions.Split(input, ' ').Take(2).ToArray();

        var (sub, main) = (selected[0], selected[1]);

        // Validate
        if (!int.TryParse(main, out var mainNum)) {
            throw new ArgumentException(nameof(mainNum));
        }
        if (!Dict.ContainsKey(mainNum)) {
            throw new IndexOutOfRangeException(nameof(mainNum));
        }

        if (!int.TryParse(sub, out var subNum)) {
            throw new ArgumentException(nameof(subNum));
        }
        if (!Dict[mainNum].Contains(subNum)) {
            throw new IndexOutOfRangeException(nameof(subNum));
        }

        ogConsole.ResetColor();

        // Return matching options
        var selectedMainOption = menu.Keys.ToArray()[mainNum - 1];
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
    public static T ReadLine<T>(string message, Type type, ConsoleColor outputColor, ConsoleColor inputColor) {
        var input = ReadLine(message, outputColor, inputColor);
        return (T)Convert.ChangeType(input, type);
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
    public static T ReadLine<T>(string message, Type type, ConsoleColor inputColor) {
        return ReadLine<T>(message, type, Colors.Default, inputColor);
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
    public static T ReadLine<T>(string message, Type type) {
        return ReadLine<T>(message, type, Colors.Default, Colors.Input);
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
    public static T ReadLine<T>(string message, ConsoleColor outputColor, ConsoleColor inputColor) {
        var input = ReadLine(message, outputColor, inputColor);

        // Convert input to desired type
        var converter = TypeDescriptor.GetConverter(typeof(T));
        return (T)converter.ConvertFromString(input);
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
    public static T ReadLine<T>(string message, ConsoleColor inputColor) {
        return ReadLine<T>(message, Colors.Default, inputColor);
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
    public static T ReadLine<T>(string message) {
        return ReadLine<T>(message, Colors.Default, Colors.Input);
    }

    /// <summary>
    /// Used to request user input
    /// </summary>
    /// <param name="message">Request title</param>
    /// <param name="outputColor"></param>
    /// <param name="inputColor"></param>
    /// <returns>Trimmed string</returns>
    public static string ReadLine(string message, ConsoleColor outputColor, ConsoleColor inputColor) {
        Write(message, outputColor);
        ogConsole.ForegroundColor = inputColor;
        var input = ogConsole.ReadLine();
        ogConsole.ResetColor();

        if (string.IsNullOrWhiteSpace(input)) {
            return default;
        }

        return input.Trim();
    }

    /// <summary>
    /// Used to request user input
    /// </summary>
    /// <param name="message">Request title</param>
    /// <param name="inputColor"></param>
    /// <returns>Trimmed string</returns>
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
    /// </summary>
    /// <param name="task">The TaskAwaiter of the background performing task</param>
    /// <param name="color">The color in which to display the progress bar</param>
    /// <param name="title">Message to display alongside the progress bar</param>
    /// <param name="displayElapsedTime">Display elapsed time</param>
    /// <param name="updateRate">Rate at which the progress bar refreshes in milliseconds</param>
    /// <returns></returns>
    public static async Task<T> IndeterminateProgressBar<T>(Task<T> task, ConsoleColor color, string title, bool displayElapsedTime, int updateRate = 50) {
        try {
            if (task.Status is not TaskStatus.Running) {
                task.Start();
            }
        } catch {
            //ignore
        }

        ogConsole.ResetColor();
        ogConsole.ForegroundColor = color;
        Stopwatch stopwatch = null;
        if (displayElapsedTime) {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }
        var lineNum = ogConsole.CursorTop;

        title = string.IsNullOrWhiteSpace(title) ? string.Empty : $"{title} ";

        while (!task.IsCompleted) { // Await until the TaskAwaiter informs of completion
            foreach (char c in Twirl) { // Cycle through the characters of twirl
                if (displayElapsedTime) {
                    ogConsole.Write($"{title}{c} [Elapsed: {stopwatch.Elapsed.ToFriendlyString()}]{ExtraBuffer}"); // Remove last character and re-write
                } else {
                    ogConsole.Write($"{title}{c}{ExtraBuffer}"); // Remove last character and re-write
                }
                ogConsole.SetCursorPosition(0, lineNum);
                await Task.Delay(updateRate); // The update rate
            }
        }

        stopwatch?.Stop();

        NewLine(); // Break line after completion

        ogConsole.ResetColor();

        return await task;
    }

    /// <summary>
    /// A simple twirl style indeterminate progress bar to signal the user that the app is not stuck but rather is performing a time consuming task.
    /// </summary>
    /// <param name="task">The TaskAwaiter of the background performing task</param>
    /// <param name="color">The color in which to display the progress bar</param>
    /// <param name="title">Message to display alongside the progress bar</param>
    /// <param name="displayElapsedTime">Display elapsed time</param>
    /// <param name="updateRate">Rate at which the progress bar refreshes in milliseconds</param>
    /// <returns></returns>
    public static async Task IndeterminateProgressBar(Task task, ConsoleColor color, string title, bool displayElapsedTime, int updateRate = 50) {
        try {
            if (task.Status is not TaskStatus.Running) {
                task.Start();
            }
        } catch {
            //ignore
        }

        ogConsole.ResetColor();
        ogConsole.ForegroundColor = color;
        Stopwatch stopwatch = null;
        if (displayElapsedTime) {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }
        var lineNum = ogConsole.CursorTop;

        title = string.IsNullOrWhiteSpace(title) ? string.Empty : $"{title} ";

        while (!task.IsCompleted) { // Await until the TaskAwaiter informs of completion
            foreach (char c in Twirl) { // Cycle through the characters of twirl
                if (displayElapsedTime) {
                    ogConsole.Write($"{title}{c} [Elapsed: {stopwatch.Elapsed.ToFriendlyString()}]{ExtraBuffer}"); // Remove last character and re-write
                } else {
                    ogConsole.Write($"{title}{c}{ExtraBuffer}"); // Remove last character and re-write
                }
                ogConsole.SetCursorPosition(0, lineNum);
                await Task.Delay(updateRate); // The update rate
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
    /// <param name="foregound">color of the bounds and percentage</param>
    /// <param name="progress">color of the progress bar fill</param>
    public static void UpdateProgressBar(int percent, ConsoleColor foregound, ConsoleColor progress) {
        ogConsole.ResetColor();
        ogConsole.ForegroundColor = foregound;
        var currentLine = ogConsole.CursorTop;
        ogConsole.WriteLine(EmptyLine);
        ogConsole.SetCursorPosition(0, currentLine);
        ogConsole.Write("[");
        var p = (ProgressBarSize * percent) / 100;
        ogConsole.ForegroundColor = progress;
        for (var i = 0; i < ProgressBarSize; i++) {
            ogConsole.Write(i >= p ? ' ' : '■');
        }
        ogConsole.ForegroundColor = foregound;
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
    /// </remarks>
    /// <param name="display"></param>
    public static void UpdateProgressBar(ProgressBarDisplay display) {
        ogConsole.ResetColor();
        ogConsole.ForegroundColor = display.Foreground;
        var currentLine = ogConsole.CursorTop;
        ogConsole.WriteLine(EmptyLine);
        ogConsole.WriteLine(EmptyLine);
        ogConsole.SetCursorPosition(0, currentLine);
        if (!string.IsNullOrWhiteSpace(display.Header)) {
            ogConsole.Write($"{display.Header}\n");
        }
        ogConsole.Write("[");
        var p = ProgressBarSize * (int)display.Percentage / 100;
        ogConsole.ForegroundColor = display.Progress;
        for (var i = 0; i < ProgressBarSize; i++) {
            ogConsole.Write(i >= p ? ' ' : '■');
        }
        ogConsole.ForegroundColor = display.Foreground;
        ogConsole.Write("] {0,5:##0.##}%", display.Percentage);
        ogConsole.SetCursorPosition(0, currentLine);
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
