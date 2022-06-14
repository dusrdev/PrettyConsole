using System;
using System.Collections.Generic;
using System.ComponentModel;

using b = System.Console;
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
            Highlight,
            Input
        };

        /// <summary>
        /// Converts local colors to use the defaults that can be changed in this class
        /// <para>this allows using different colors even without calling the built in System.ConsoleColor's</para>
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static ConsoleColor ConvertFromColor(Color color) {
            return color switch {
                Color.Primary => Colors.Primary,
                Color.Secondary => Colors.Secondary,
                Color.Success => Colors.Success,
                Color.Error => Colors.Error,
                Color.Highlight => Colors.Highlight,
                Color.Input => Colors.Input,
                _ => Colors.Primary,
            };
        }

        /// <summary>
        /// The default color scheme
        /// </summary>
        /// <remarks>
        /// Modify using SetColors
        /// </remarks>
        public static Colors Colors { get; private set; } = new();

        /// <summary>
        /// Used to set the colors which are used by default in most functions of the class
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <param name="success"></param>
        /// <param name="error"></param>
        /// <param name="highlight"></param>
        /// <param name="input"></param>
        public static void SetColors(Action<Colors> modification) {
            modification.Invoke(Colors);
        }

        /// <summary>
        /// Write any object to the console in <b>ColorBase</b>
        /// </summary>
        /// <remarks>
        /// To end line, use <b>WriteLine</b> with the same parameters
        /// </remarks>
        public static void Write(object o) {
            Write(o, Colors.Primary);
        }

        /// <summary>
        /// Write any object to the console in <b>ColorBase</b> and ends line
        /// </summary>
        /// <remarks>
        /// To write without ending line, use <b>Write</b> with the same parameters
        /// </remarks>
        public static void WriteLine(object o) {
            WriteLine(o, Colors.Primary);
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
            Write((message, Colors.Primary), ("... ", Colors.Highlight));
            b.ForegroundColor = Colors.Input;
            _ = b.Read();
            b.ForegroundColor = Colors.Primary;
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
            Write((message.ToString(), Colors.Primary), ("? ", Colors.Highlight), ("[", Colors.Primary), ("y", Colors.Success), 
                ("/", Colors.Primary), ("n", Colors.Error), ("]: ", Colors.Primary)); ;
            b.ForegroundColor = Colors.Input;
            string input = b.ReadLine();
            b.ResetColor();
            if (string.IsNullOrEmpty(input) || input.ToLower() is "y" or "yes") {
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
        public static string Selection(ReadOnlySpan<char> title, IEnumerable<string> choices) {
            if (!Extensions.IsEmptyOrWhiteSpace(title)) {
                WriteLine(title.ToString(), Colors.Highlight);
            }
            Dictionary<int, string> dict = new();
            int i = 1;
            // Enumerate list with numbers to allow selection by index
            foreach (string choice in choices) {
                WriteLine(($"\t{i}", Colors.Highlight), ($". {choice}", Colors.Primary));
                dict.Add(i, choice);
                i++;
            }
            NewLine();

            int selected = ReadLine<int>("Enter your choice:");

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
            int i = 1;
            // Enumerate list of choices
            foreach (string choice in choices) {
                WriteLine(($"\t{i}", Colors.Highlight), ($". {choice}", Colors.Primary));
                dict.Add(i, choice);
                i++;
            }
            List<string> results = new();

            NewLine();
            string input = ReadLine<string>("Enter your choices separated with spaces:");

            // get selected indexes from user
            var selected = Extensions.SplitAsSpan(input, ' ');

            // evaluate and add selections to results
            foreach (string choice in selected) {
                var trimmed = choice.Trim();
                if (!int.TryParse(trimmed, out int num)) {
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
                int prefixLength = i.ToString().Length + 2;
                Write(($"{i}", Colors.Highlight), ($". {Extensions.SuffixWithSpaces(mainChoice, maxMainOption - prefixLength)}",
                    Colors.Primary));
                foreach (var subChoice in subChoices) {
                    lst.Add(j);
                    if (j is 1) {
                        WriteLine(($"{j}", Colors.Highlight), ($". {subChoice}", Colors.Primary));
                    } else {
                        WriteLine(($"{Extensions.SuffixWithSpaces(null, maxMainOption)}{j}", Colors.Highlight), ($". {subChoice}", 
                            Colors.Primary));
                    }
                    j++;
                }
                Dict.Add(i, lst);
                j = 1;
                i++;
                NewLine();
            }

            string input = ReadLine<string>("Enter your choice separated with spaces:");

            // get the 2 arguments, as in tree options and sub-tree option
            var selected = Extensions.SplitAsSpan(input, ' ');

            // save the 2 arguments
            var enumerator = selected.GetEnumerator();
            enumerator.MoveNext();
            string main = enumerator.Current;
            enumerator.MoveNext();
            string sub = enumerator.Current;

            // Validate
            if (!int.TryParse(main, out int mainNum)) {
                throw new ArgumentException(nameof(mainNum));
            }
            if (!Dict.ContainsKey(mainNum)) {
                throw new IndexOutOfRangeException(nameof(mainNum));
            }

            if (!int.TryParse(sub, out int subNum)) {
                throw new ArgumentException(nameof(subNum));
            }
            if (!Dict[mainNum].Contains(subNum)) {
                throw new IndexOutOfRangeException(nameof(subNum));
            }

            b.ResetColor();

            // Return matching options
            string selectedMainOption = menu.Keys.ToArray()[mainNum - 1];
            string selectedSubOption = menu[selectedMainOption][subNum - 1];
            return (selectedMainOption, selectedSubOption);
        }

        /// <summary>
        /// Used to request user input, validates and converts common types.
        /// </summary>
        /// <typeparam name="T">Any common type</typeparam>
        /// <param name="message">Request message</param>
        /// <returns>Converted input</returns>
        /// <remarks>
        /// For complex types request a string and validate/convert yourself
        /// </remarks>
        public static T ReadLine<T>(ReadOnlySpan<char> message) {
            Write($"{message} ", Colors.Primary);
            b.ForegroundColor = Colors.Input;
            string input = b.ReadLine();
            b.ResetColor();

            input = input.Trim();

            // Convert input to desired type
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
        public static void ResetDefaultColors() {
            Colors = new();
        }

        /// <summary>
        /// Used to end current line or write an empty one, depends whether the current line has any text
        /// </summary>
        public static void NewLine() {
            b.WriteLine();
        }
    }
}
