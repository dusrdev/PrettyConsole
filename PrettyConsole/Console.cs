using System;
using System.Collections.Generic;
using System.ComponentModel;

using b = System.Console;
using PrettyConsole.Helpers;
using System.Linq;

namespace PrettyConsole {
    public static class Console {
        /// <summary>
        /// Provides easy access to the colors which are used throughout this class
        /// <para>Using this while optionally changing the default colors will make the interface more streamlined</para>
        /// </summary>
        public enum Color {
            Primary,
            Secondary,
            Success,
            Error,
            Highlight
        };

        /// <summary>
        /// Converts local colors to use the defaults that can be changed in this class
        /// <para>this allows using different colors even without calling the built in System.ConsoleColor's</para>
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static ConsoleColor ConvertFromColor(Color color) {
            return color switch {
                Color.Primary => ColorPrimary,
                Color.Secondary => ColorSecondary,
                Color.Success => ColorSuccess,
                Color.Error => ColorError,
                Color.Highlight => ColorHighlight,
                _ => ColorPrimary,
            };
        }

        /// <summary>
        /// The base color of all texts
        /// </summary>
        /// <remarks>By default is <c>ConsoleColor.White</c></remarks>
        public static ConsoleColor ColorPrimary { get; set; } = ConsoleColor.White;

        /// <summary>
        /// <para>The title color for advanced input options like selection or multi-selection</para>
        /// </summary>
        /// <remarks>By default is <c>ConsoleColor.Blue</c></remarks>
        public static ConsoleColor ColorHighlight { get; set; } = ConsoleColor.Blue;

        /// <summary>
        /// The color for success
        /// </summary>
        /// <remarks>By default is <c>ConsoleColor.Green</c></remarks>
        public static ConsoleColor ColorSuccess { get; set; } = ConsoleColor.Green;

        /// <summary>
        /// The color for error
        /// </summary>
        /// <remarks>By default is <c>ConsoleColor.Red</c></remarks>
        public static ConsoleColor ColorError { get; set; } = ConsoleColor.Red;

        /// <summary>
        /// Secondary color - used for subtexts
        /// </summary>
        /// <remarks>By default is <c>ConsoleColor.Gray</c></remarks>
        public static ConsoleColor ColorSecondary { get; set; } = ConsoleColor.Gray;

        /// <summary>
        /// The color of user inputs when requested from this class
        /// </summary>
        /// <remarks>By default is <c>ConsoleColor.Gray</c></remarks>
        public static ConsoleColor ColorInput { get; set; } = ConsoleColor.Gray;

        /// <summary>
        /// Used to set the colors which are used by default in most functions of the class
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <param name="success"></param>
        /// <param name="error"></param>
        /// <param name="highlight"></param>
        /// <param name="input"></param>
        private static void SetColors(ConsoleColor primary = ConsoleColor.White, ConsoleColor secondary = ConsoleColor.Gray,
            ConsoleColor success = ConsoleColor.Green, ConsoleColor error = ConsoleColor.Red,
            ConsoleColor highlight = ConsoleColor.Blue, ConsoleColor input = ConsoleColor.Gray) {
            ColorPrimary = primary;
            ColorSecondary = secondary;
            ColorSuccess = success;
            ColorError = error;
            ColorHighlight = highlight;
            ColorInput = input;
        }

        /// <summary>
        /// Write any object to the console in <b>ColorBase</b>
        /// </summary>
        /// <remarks>
        /// To end line, use <b>WriteLine</b> with the same parameters
        /// </remarks>
        public static void Write(object o) {
            Write(o, ColorPrimary);
        }

        /// <summary>
        /// Write any object to the console in <b>ColorBase</b> and ends line
        /// </summary>
        /// <remarks>
        /// To write without ending line, use <b>Write</b> with the same parameters
        /// </remarks>
        public static void WriteLine(object o) {
            WriteLine(o, ColorPrimary);
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
        /// Write any object to the console in <paramref name="color"/>
        /// </summary>
        /// <remarks>
        /// To end line, use <b>WriteLine</b> with the same parameters
        /// </remarks>
        public static void Write(object o, Color color) {
            b.ResetColor();
            b.ForegroundColor = ConvertFromColor(color);
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
        /// Write any object to the console in <paramref name="color"/> and ends line
        /// </summary>
        /// <remarks>
        /// To write without ending line, use <b>Write</b> with the same parameters
        /// </remarks>
        public static void WriteLine(object o, Color color) {
            Write(o, ConvertFromColor(color));
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
        /// Write tuples of (<b>element</b>, <b>color</b>) to the console
        /// </summary>
        /// <remarks>
        /// To end line, use <b>WriteLine</b> with the same parameters
        /// </remarks>
        public static void Write(params (object item, Color color)[] elements) {
            if (elements is null || elements.Length is 0) {
                throw new ArgumentException("Invalid parameters");
            }
            b.ResetColor();
            foreach (var (o, c) in elements) {
                b.ForegroundColor = ConvertFromColor(c);
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
        /// Write tuples of (<b>element</b>, <b>color</b>) to the console and ends line
        /// </summary>
        /// <remarks>
        /// To write without ending line, use <b>Write</b> with the same parameters
        /// </remarks>
        public static void WriteLine(params (object item, Color color)[] elements) {
            Write(elements);
            NewLine();
        }

        /// <summary>
        /// Used to wait for user input, you can customize <paramref name="message"/> or leave as default
        /// </summary>
        /// <param name="message"><b>Default value:</b> "Press any key to continue"</param>
        public static void RequestAnyInput(string message = "Press any key to continue") {
            Write((message, ColorPrimary), ("... ", ColorHighlight));
            b.ForegroundColor = ColorInput;
            _ = b.Read();
            b.ForegroundColor = ColorPrimary;
        }

        /// <summary>
        /// Used to get user confirmation
        /// </summary>
        /// <param name="message"></param>
        /// <remarks>
        /// The user can confirm by entering <b>"Y"</b>/<b>"y"</b> or just pressing <b>enter</b>, anything else is regarded as <c>false</c>.
        /// </remarks>
        public static bool Confirm(string message) {
            Write((message, ColorPrimary), ("? ", ColorHighlight), ("[", ColorPrimary), ("y", ColorSuccess), ("/", ColorPrimary), ("n", ColorError), ("]: ", ColorPrimary)); ;
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
                WriteLine(title, ColorHighlight);
            }
            Dictionary<int, string> dict = new();
            int i = 1;
            foreach (string choice in choices) {
                WriteLine(($"\t{i}", ColorHighlight), ($". {choice}", ColorPrimary));
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
        /// Enumerates a list of strings and allows the user to select multiple strings by any order
        /// </summary>
        /// <param name="title"><b>Optional</b>, null or whitespace will not be displayed</param>
        /// <param name="choices">Any collection of strings</param>
        /// <returns>A list containing any selected choices by order of selection</returns>
        /// <remarks>
        /// This validates the input for you.
        /// </remarks>
        public static List<string> MultiSelection(string title, IEnumerable<string> choices) {
            if (!string.IsNullOrWhiteSpace(title)) {
                WriteLine(title, ColorHighlight);
            }
            Dictionary<int, string> dict = new();
            int i = 1;
            foreach (string choice in choices) {
                WriteLine(($"\t{i}", ColorHighlight), ($". {choice}", ColorPrimary));
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
                WriteLine(title, ColorHighlight);
                NewLine();
            }
            var maxMainOption = General.MaxStringLength(menu.Keys);
            var Dict = new Dictionary<int, List<int>>();
            maxMainOption += 10;
            int i = 1, j = 1;
            foreach (var (mainChoice, subChoices) in menu) {
                var lst = new List<int>();
                int prefixLength = i.ToString().Length + 2;
                Write(($"{i}", ColorHighlight), ($". {General.SuffixWithSpaces(mainChoice, maxMainOption - prefixLength)}", ColorPrimary));
                foreach (var subChoice in subChoices) {
                    lst.Add(j);
                    if (j is 1) {
                        WriteLine(($"{j}", ColorHighlight), ($". {subChoice}", ColorPrimary));
                    } else {
                        WriteLine(($"{General.SuffixWithSpaces(null, maxMainOption)}{j}", ColorHighlight), ($". {subChoice}", ColorPrimary));
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
            Write($"{message}: ", ColorPrimary);
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
            ColorPrimary = ConsoleColor.White;
            ColorSecondary = ConsoleColor.Gray;
            ColorHighlight = ConsoleColor.Blue;
            ColorSuccess = ConsoleColor.Green;
            ColorError = ConsoleColor.Red;
            ColorInput = ConsoleColor.Gray;
        }

        /// <summary>
        /// Used to end current line or write an empty one, depends whether the current line has any text
        /// </summary>
        public static void NewLine() {
            b.WriteLine();
        }
    }
}
