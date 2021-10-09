using System;
using System.Collections.Generic;
using System.ComponentModel;

using b = System.Console;

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

        public static void Write(object o) {
            Write(o, ColorBase);
        }
        public static void WriteLine(object o) {
            WriteLine(o, ColorBase);
        }

        public static void Write(object o, ConsoleColor color) {
            b.ResetColor();
            b.ForegroundColor = color;
            b.Write(o);
            b.ResetColor();
        }

        public static void WriteLine(object o, ConsoleColor color) {
            Write(o, color);
            NewLine();
        }

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

        public static void WriteLine(params (object item, ConsoleColor color)[] elements) {
            Write(elements);
            NewLine();
        }

        public static void RequestAnyInput(string message) {
            Write(message, ColorBase);
            b.ForegroundColor = ColorInput;
            _ = b.Read();
        }

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

        public static string Selection(string title, IEnumerable<string> choices) {
            WriteLine(title, ColorTitle);
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

        public static List<string> MultiSelection(string title, IEnumerable<string> choices) {
            WriteLine(title, ColorTitle);
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
            return default(T);
        }

        public static void Clear() {
            b.Clear();
        }

        public static void ResetColors() {
            ColorBase = ConsoleColor.White;
            ColorTitle = ConsoleColor.Cyan;
            ColorHighlight = ConsoleColor.Green;
            ColorInput = ConsoleColor.Green;
        }

        public static void NewLine() {
            b.WriteLine();
        }
    }
}
