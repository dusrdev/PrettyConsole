using System;
using System.Collections.Generic;
using System.ComponentModel;
using b = System.Console;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PrettyConsole {
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
        private static readonly int ProgressBarSize = b.BufferWidth - 10;

        // Constant pattern containing the characters needed for the indeterminate progress bar
        private const string Twirl = "-\\|/";

        // A whitespace the length of 10 spaces
        private const string ExtraBuffer = "          ";

        /// <summary>
        /// Write any object to the console in the default color
        /// </summary>
        /// <param name="o">The object to write</param>
        /// <remarks>
        /// To end line, use <b>WriteLine</b> with the same parameters
        /// </remarks>
        public static void Write(object o) {
            Write(o, Colors.Default);
        }

        /// <summary>
        /// Write any object to the console in <paramref name="color"/>
        /// </summary>
        /// <param name="o">The object to write</param>
        /// <param name="color">The color in which the output will be displayed</param>
        /// <remarks>
        /// To end line, use <b>WriteLine</b> with the same parameters
        /// </remarks>
        public static void Write(object o, ConsoleColor color) {
            b.ResetColor();
            b.ForegroundColor = color;
            b.Write(o);
            b.ResetColor();
        }

        /// <summary>
        /// Write any object to the console in the default color and ends line
        /// </summary>
        /// <param name="o">The object to write</param>
        /// <remarks>
        /// To write without ending line, use <b>Write</b> with the same parameters
        /// </remarks>
        public static void WriteLine(object o) {
            Write(o, Colors.Default);
            NewLine();
        }

        /// <summary>
        /// Write any object to the console in <paramref name="color"/> and ends line
        /// </summary>
        /// <param name="o">The object to write</param>
        /// <param name="color">The color in which the output will be displayed</param>
        /// <remarks>
        /// To write without ending line, use <b>Write</b> with the same parameters
        /// </remarks>
        public static void WriteLine(object o, ConsoleColor color) {
            Write(o, color);
            NewLine();
        }

        /// <summary>
        /// Write tuples of (<b>element</b>, <b>color</b>) to the console
        /// </summary>
        /// <remarks>
        /// To end line, use <b>WriteLine</b> with the same parameters
        /// </remarks>
        public static void Write(params (object item, ConsoleColor color)[] elements) {
            if (elements is null || elements.Length is 0) {
                throw new ArgumentException("Invalid parameters");
            }
            b.ResetColor();
            foreach (var (o, c) in elements) {
                b.ForegroundColor = c;
                b.Write(o);
            }
            b.ResetColor();
        }

        /// <summary>
        /// Write tuples of (<b>element</b>, <b>color</b>) to the console and ends line
        /// </summary>
        /// <remarks>
        /// To write without ending line, use <b>Write</b> with the same parameters
        /// </remarks>
        public static void WriteLine(params (object item, ConsoleColor color)[] elements) {
            Write(elements);
            NewLine();
        }

        /// <summary>
        /// Write any object to the console as a label, Colors.Default background - black text
        /// </summary>
        /// <param name="o">The object to write</param>
        /// <remarks>
        /// To end line, call <b>NewLine()</b> after.
        /// </remarks>
        public static void Label(object o) {
            Label(o, ConsoleColor.Black, Colors.Default);
        }

        /// <summary>
        /// Write any object to the console as a label, modified foreground and background
        /// </summary>
        /// <param name="o">The object to write</param>
        /// <param name="foreground">foreground color - i.e: color of the string representation</param>
        /// <param name="background">background color</param>
        /// <remarks>
        /// To end line, call <b>NewLine()</b> after.
        /// </remarks>
        public static void Label(object o, ConsoleColor foreground, ConsoleColor background) {
            b.ResetColor();
            b.ForegroundColor = foreground;
            b.BackgroundColor = background;
            b.Write(o);
            b.ResetColor();
        }

        /// <summary>
        /// Write tuples of (<b>element</b>, <b>color</b>) to the console
        /// </summary>
        /// <remarks>
        /// To end line, use <b>WriteLine</b> with the same parameters
        /// </remarks>
        public static void Label(params (object item, ConsoleColor foreground, ConsoleColor background)[] elements) {
            if (elements is null || elements.Length is 0) {
                throw new ArgumentException("Invalid parameters");
            }
            b.ResetColor();
            foreach (var (o, foreground, background) in elements) {
                b.ForegroundColor = foreground;
                b.BackgroundColor = background;
                b.Write(o);
            }
            b.ResetColor();
        }

        /// <summary>
        /// Used to wait for user input, you can customize <paramref name="message"/> or leave as default
        /// </summary>
        /// <param name="message"><b>Default value:</b> "Press any key to continue"</param>
        public static void RequestAnyInput(string message = "Press any key to continue") {
            Write((message, Colors.Default), ("... ", Colors.Highlight));
            b.ForegroundColor = Colors.Input;
            _ = b.Read();
            b.ForegroundColor = Colors.Default;
            b.ResetColor();
        }

        /// <summary>
        /// Used to get user confirmation
        /// </summary>
        /// <param name="message"></param>
        /// <remarks>
        /// The user can confirm by entering <b>"y"</b>/<b>"yes"</b> or just pressing <b>enter</b>, anything else is regarded as <c>false</c>.
        /// </remarks>
        public static bool Confirm(ReadOnlySpan<char> message) {
            Write((message.ToString(), Colors.Default), ("? ", Colors.Highlight), ("[", Colors.Default), ("y", Colors.Success),
                ("/", Colors.Default), ("n", Colors.Error), ("]: ", Colors.Default)); ;
            b.ForegroundColor = Colors.Input;
            var input = b.ReadLine();
            b.ResetColor();
            return string.IsNullOrEmpty(input) || input.ToLower() is "y" or "yes";
        }

        /// <summary>
        /// Enumerates a list of strings and allows the user to select one by number
        /// </summary>
        /// <param name="title"><b>Optional</b>, null or whitespace will not be displayed</param>
        /// <param name="choices">Any collection of strings</param>
        /// <returns>The selected string</returns>
        /// <remarks>
        /// This validates the input for you.
        /// </remarks>
        public static string Selection(ReadOnlySpan<char> title, IEnumerable<string> choices) {
            if (!Extensions.IsEmptyOrWhiteSpace(title)) {
                WriteLine(title.ToString(), Colors.Highlight);
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

            var selected = ReadLine<int>("Enter your choice:");

            if (!dict.ContainsKey(selected)) {
                throw new IndexOutOfRangeException(nameof(selected));
            }

            return dict[selected];
        }

        /// <summary>
        /// Enumerates a list of strings and allows the user to select multiple strings by any order
        /// </summary>
        /// <param name="title"><b>Optional</b>, null or whitespace will not be displayed</param>
        /// <param name="choices">Any collection of strings</param>
        /// <returns>A list containing any selected choices by order of selection</returns>
        /// <remarks>
        /// This validates the input for you.
        /// </remarks>
        public static List<string> MultiSelection(ReadOnlySpan<char> title, IEnumerable<string> choices) {
            if (!Extensions.IsEmptyOrWhiteSpace(title)) {
                WriteLine(title.ToString(), Colors.Highlight);
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
            var input = ReadLine<string>("Enter your choices separated with spaces:");

            // get selected indexes from user
            var selected = Extensions.SplitAsSpan(input, ' ');

            // evaluate and add selections to results
            foreach (var choice in selected) {
                var trimmed = choice.Trim();
                if (!int.TryParse(trimmed, out var num)) {
                    throw new ArgumentException(nameof(choice));
                }
                if (!dict.ContainsKey(num)) {
                    throw new IndexOutOfRangeException(nameof(choice));
                }
                results.Add(dict[num]);
            }

            b.ResetColor();
            return results;
        }

        /// <summary>
        /// Enumerates a menu containing main option as well as sub options and allows the user to select both.
        /// <para>
        /// * This function is great where more options or categories are required than <b>Selection</b> can provide.
        /// </para>
        /// </summary>
        /// <param name="title"><b>Optional</b>, null or whitespace will not be displayed</param>
        /// <param name="menu">A nested dictionary containing menu titles</param>
        /// <returns>The selected main option and selected sub option</returns>
        /// <remarks>
        /// This validates the input for you.
        /// </remarks>
        public static (string option, string subOption) TreeMenu(ReadOnlySpan<char> title, Dictionary<string, List<string>> menu) {
            if (!Extensions.IsEmptyOrWhiteSpace(title)) {
                WriteLine(title.ToString(), Colors.Highlight);
                NewLine();
            }
            var maxMainOption = Extensions.MaxStringLength(menu.Keys); // Used to make sub-tree prefix spaces uniform
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

            var input = ReadLine<string>("Enter your choice separated with spaces:");

            // get the 2 arguments, as in tree options and sub-tree option
            var selected = Extensions.SplitAsSpan(input, ' ');

            // save the 2 arguments
            using var enumerator = selected.GetEnumerator();
            enumerator.MoveNext();
            var main = enumerator.Current;
            enumerator.MoveNext();
            var sub = enumerator.Current;

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

            b.ResetColor();

            // Return matching options
            var selectedMainOption = menu.Keys.ToArray()[mainNum - 1];
            var selectedSubOption = menu[selectedMainOption][subNum - 1];
            return (selectedMainOption, selectedSubOption);
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
        public static T ReadLine<T>(ReadOnlySpan<char> message) {
            Write($"{message.ToString()} ", Colors.Default);
            b.ForegroundColor = Colors.Input;
            var input = b.ReadLine();
            b.ResetColor();

            if (string.IsNullOrWhiteSpace(input)) {
                return default;
            }

            input = input.Trim();

            // Convert input to desired type
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFromString(input);
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

            b.ResetColor();
            b.ForegroundColor = color;
            Stopwatch stopwatch = null;
            if (displayElapsedTime) {
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }
            var lineNum = b.CursorTop;

            title = string.IsNullOrWhiteSpace(title) ? string.Empty : $"{title} ";

            while (!task.IsCompleted) { // Await until the TaskAwaiter informs of completion
                foreach (char c in Twirl) { // Cycle through the characters of twirl
                    if (displayElapsedTime) {
                        b.Write($"{title}{c} [Elapsed: {stopwatch.Elapsed.ToFriendlyString()}]{ExtraBuffer}"); // Remove last character and re-write
                    } else {
                        b.Write($"{title}{c}{ExtraBuffer}"); // Remove last character and re-write
                    }
                    b.SetCursorPosition(0, lineNum);
                    await Task.Delay(updateRate); // The update rate
                }
            }

            stopwatch?.Stop();

            NewLine(); // Break line after completion

            b.ResetColor();

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

            b.ResetColor();
            b.ForegroundColor = color;
            Stopwatch stopwatch = null;
            if (displayElapsedTime) {
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }
            var lineNum = b.CursorTop;

            title = string.IsNullOrWhiteSpace(title) ? string.Empty : $"{title} ";

            while (!task.IsCompleted) { // Await until the TaskAwaiter informs of completion
                foreach (char c in Twirl) { // Cycle through the characters of twirl
                    if (displayElapsedTime) {
                        b.Write($"{title}{c} [Elapsed: {stopwatch.Elapsed.ToFriendlyString()}]{ExtraBuffer}"); // Remove last character and re-write
                    } else {
                        b.Write($"{title}{c}{ExtraBuffer}"); // Remove last character and re-write
                    }
                    b.SetCursorPosition(0, lineNum);
                    await Task.Delay(updateRate); // The update rate
                }
            }

            stopwatch?.Stop();

            NewLine(); // Break line after completion

            b.ResetColor();
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
            b.ResetColor();
            b.ForegroundColor = color;
            var currentLine = b.CursorTop;
            b.Write("[");
            var p = (ProgressBarSize * percent) / 100;
            for (var i = 0; i < ProgressBarSize; i++) {
                b.Write(i >= p ? ' ' : '■');
            }
            b.Write("] {0,3:##0}%", percent);
            b.SetCursorPosition(0, currentLine);
            b.ResetColor();
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
            b.Clear();
        }

        /// <summary>
        /// Used to end current line or write an empty one, depends whether the current line has any text
        /// </summary>
        public static void NewLine() {
            b.WriteLine();
        }
    }
}
