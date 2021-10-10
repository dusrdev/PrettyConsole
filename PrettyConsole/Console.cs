using System;
using System.Collections.Generic;
using System.ComponentModel;

using b = System.Console;
using PrettyConsole.Helpers;
using System.Linq;

namespace PrettyConsole {
    public static class Console {
        /// <summary>
        /// The base color of all texts
        /// </summary>
        /// <remarks>By default is <c>ConsoleColor.White</c></remarks>
        public static ConsoleColor ColorBase { get; set; } = ConsoleColor.White;

        /// <summary>
        /// The title color for advanced input options like selection or multi-selection
        /// </summary>
        /// <remarks>By default is <c>ConsoleColor.Cyan</c></remarks>
        public static ConsoleColor ColorTitle { get; set; } = ConsoleColor.Cyan;

        /// <summary>
        /// The highlight color
        /// </summary>
        /// <remarks>By default is <c>ConsoleColor.Green</c></remarks>
        public static ConsoleColor ColorHighlight { get; set; } = ConsoleColor.Green;

        /// <summary>
        /// The color of user inputs when requested from this class
        /// </summary>
        /// <remarks>By default is <c>ConsoleColor.Green</c></remarks>
        public static ConsoleColor ColorInput { get; set; } = ConsoleColor.Green;

        /// <summary>
        /// Write any object to the console in <b>ColorBase</b>
        /// </summary>
        /// <remarks>
        /// To end line, use <b>WriteLine</b> with the same parameters
        /// </remarks>
        public static void Write(object o) {
            Write(o, ColorBase);
        }

        /// <summary>
        /// Write any object to the console in <b>ColorBase</b> and ends line
        /// </summary>
        /// <remarks>
        /// To write without ending line, use <b>Write</b> with the same parameters
        /// </remarks>
        public static void WriteLine(object o) {
            WriteLine(o, ColorBase);
        }

        /// <summary>
        /// Write any object to the console in <paramref name="color"/>
        /// </summary>
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
        /// Write any object to the console in <paramref name="color"/> and ends line
        /// </summary>
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
        /// Used to wait for user input, you can customize <paramref name="message"/> or leave as default
        /// </summary>
        /// <param name="message"><b>Default value:</b> "Press any key to continue"</param>
        public static void RequestAnyInput(string message = "Press any key to continue") {
            Write((message, ColorBase), ("... ", ColorHighlight));
            b.ForegroundColor = ColorInput;
            _ = b.Read();
            b.ForegroundColor = ColorBase;
        }

        /// <summary>
        /// Used to get user confirmation
        /// </summary>
        /// <param name="message"></param>
        /// <remarks>
        /// The user can confirm by entering <b>"Y"</b>/<b>"y"</b> or just pressing <b>enter</b>, anything else is regarded as <c>false</c>.
        /// </remarks>
        public static bool Confirm(string message) {
            Write((message, ColorBase), ("? ", ColorHighlight), ("[", ColorBase), ("y", ColorHighlight), ("/", ColorBase), ("n", ConsoleColor.Red), ("]: ", ColorBase));
            b.ForegroundColor = ColorInput;
            string input = b.ReadLine();
            b.ResetColor();
            if (string.IsNullOrEmpty(input) || input is "y" or "Y") {
                return true;
            }
            return false;
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
        public static string Selection(string title, IEnumerable<string> choices) {
            if (!string.IsNullOrWhiteSpace(title)) {
                WriteLine(title, ColorTitle);
            }
            Dictionary<int, string> dict = new();
            int i = 1;
            foreach (string choice in choices) {
                WriteLine(($"\t{i}", ColorHighlight), ($". {choice}", ColorBase));
                dict.Add(i, choice);
                i++;
            }
            NewLine();

            int selected = Input<int>("Enter your choice");

            if (!dict.ContainsKey(selected)) {
                throw new ArgumentOutOfRangeException(nameof(selected));
            }

            return dict[selected];
        }

        /// <summary>
        /// Enumerates a list of strings and allows the user to multiple strings by any order
        /// </summary>
        /// <param name="title"><b>Optional</b>, null or whitespace will not be displayed</param>
        /// <param name="choices">Any collection of strings</param>
        /// <returns>A list containing any selected choices by order of selection</returns>
        /// <remarks>
        /// This validates the input for you.
        /// </remarks>
        public static List<string> MultiSelection(string title, IEnumerable<string> choices) {
            if (!string.IsNullOrWhiteSpace(title)) {
                WriteLine(title, ColorTitle);
            }
            Dictionary<int, string> dict = new();
            int i = 1;
            foreach (string choice in choices) {
                WriteLine(($"\t{i}", ColorHighlight), ($". {choice}", ColorBase));
                dict.Add(i, choice);
                i++;
            }
            List<string> results = new();

            NewLine();
            string input = Input<string>("Enter your choices separated with spaces");

            string[] selected = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            foreach (string choice in selected) {
                var trimmed = choice.Trim();
                if (string.IsNullOrEmpty(trimmed)) {
                    throw new ArgumentNullException(nameof(choice));
                }
                if (!int.TryParse(trimmed, out int num)) {
                    throw new ArgumentException(nameof(choice));
                }
                if (!dict.ContainsKey(num)) {
                    throw new ArgumentOutOfRangeException(nameof(choice));
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
        public static (string option, string subOption) TreeMenu(string title, Dictionary<string, List<string>> menu) {
            if (!string.IsNullOrWhiteSpace(title)) {
                WriteLine(title, ColorTitle);
                NewLine();
            }
            var maxMainOption = General.MaxStringLength(menu.Keys);
            var Dict = new Dictionary<int, List<int>>();
            maxMainOption += 10;
            int i = 1, j = 1;
            foreach (var (mainChoice, subChoices) in menu) {
                var lst = new List<int>();
                int prefixLength = i.ToString().Length + 2;
                Write(($"{i}", ColorHighlight), ($". {General.SuffixWithSpaces(mainChoice, maxMainOption - prefixLength)}", ColorBase));
                foreach (var subChoice in subChoices) {
                    lst.Add(j);
                    if (j is 1) {
                        WriteLine(($"{j}", ColorHighlight), ($". {subChoice}", ColorBase));
                    } else {
                        WriteLine(($"{General.SuffixWithSpaces(null, maxMainOption)}{j}", ColorHighlight), ($". {subChoice}", ColorBase));
                    }
                    j++;
                }
                Dict.Add(i, lst);
                j = 1;
                i++;
                NewLine();
            }

            string input = Input<string>("Enter your choices separated with spaces");

            string[] selected = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var (main, sub) = (selected[0], selected[1]);

            main = main.Trim();
            sub = sub.Trim();

            if (string.IsNullOrWhiteSpace(main)) {
                throw new ArgumentNullException(nameof(main));
            }
            if (!int.TryParse(main, out int mainNum)) {
                throw new ArgumentException(nameof(mainNum));
            }
            if (!Dict.ContainsKey(mainNum)) {
                throw new ArgumentOutOfRangeException(nameof(mainNum));
            }

            if (string.IsNullOrWhiteSpace(sub)) {
                throw new ArgumentNullException(nameof(sub));
            }
            if (!int.TryParse(sub, out int subNum)) {
                throw new ArgumentException(nameof(subNum));
            }
            if (!Dict[mainNum].Contains(subNum)) {
                throw new ArgumentOutOfRangeException(nameof(subNum));
            }

            b.ResetColor();

            string selectedMainOption = menu.Keys.ToArray()[mainNum - 1];
            string selectedSubOption = menu[selectedMainOption][subNum - 1];
            return (selectedMainOption, selectedSubOption);
        }

        /// <summary>
        /// Used to request user input, validates and converts common types.
        /// </summary>
        /// <typeparam name="T">Any common type</typeparam>
        /// <param name="message">Request message, already suffixed with ":"</param>
        /// <returns>Converted input</returns>
        /// <remarks>
        /// For complex types request a string and validate/convert yourself
        /// </remarks>
        public static T Input<T>(string message) {
            Write($"{message}: ", ColorBase);
            b.ForegroundColor = ColorInput;
            string input = b.ReadLine();
            b.ResetColor();

            input = input.Trim();

            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter is not null) {
                return (T)converter.ConvertFromString(input);
            }
            return default;
        }

        /// <summary>
        /// Used to clear all previous outputs to the console
        /// </summary>
        public static void Clear() {
            b.Clear();
        }

        /// <summary>
        /// Resets the color properties to the default values, used when the default values were altered to produce and more customizable output.
        /// </summary>
        public static void ResetColors() {
            ColorBase = ConsoleColor.White;
            ColorTitle = ConsoleColor.Cyan;
            ColorHighlight = ConsoleColor.Green;
            ColorInput = ConsoleColor.Green;
        }

        /// <summary>
        /// Used to end current line or write an empty one, depends whether the current line has any text
        /// </summary>
        public static void NewLine() {
            b.WriteLine();
        }
    }
}
