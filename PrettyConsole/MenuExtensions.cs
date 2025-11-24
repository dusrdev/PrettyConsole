using System.Buffers;

namespace PrettyConsole;

/// <summary>
/// Provides methods extending <see cref="Console"/> with menu rendering controls.
/// </summary>
public static class MenuExtensions {
    extension(Console) {
        /// <summary>
        /// Enumerates a list of strings and allows the user to select one by number
        /// </summary>
        /// <param name="choices">Any collection of strings</param>
        /// <param name="handler">title</param>
        /// <returns>The selected string, or empty if the choice was invalid.</returns>
        /// <remarks>
        /// This validates the input for you.
        /// </remarks>
        public static string Selection<TList>(TList choices, [InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default)
            where TList : IList<string> {
            handler.ResetColors();
            handler.AppendNewLine();

            for (int i = 0; i < choices.Count; i++) {
                Console.WriteLineInterpolated($" {i + 1}) {choices[i]}");
            }

            Console.NewLine();

            if (!Console.TryReadLine(out int selected, $"Enter your choice: ")) {
                return string.Empty;
            }

            selected--;

            if ((uint)selected >= (uint)choices.Count) {
                return string.Empty;
            }

            return choices[selected];
        }

        /// <summary>
        /// Enumerates a list of strings and allows the user to select multiple strings by any order, and uses the default index color (White)
        /// </summary>
        /// <param name="choices">Any collection of strings</param>
        /// <param name="handler">title</param>
        /// <returns>An array containing any selected choices by order of selection, or empty array if any choice is invalid</returns>
        /// <remarks>
        /// This validates the input for you.
        /// </remarks>
        public static string[] MultiSelection<TList>(TList choices, [InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default)
            where TList : IList<string> {
            handler.ResetColors();
            handler.AppendNewLine();

            for (int i = 0; i < choices.Count; i++) {
                Console.WriteLineInterpolated($" {i + 1}) {choices[i]}");
            }

            Console.NewLine();

            string input = Console.ReadLine(string.Empty, $"Enter your choices separated with spaces: ");

            if (input.Length == 0) {
                return [];
            }

            var entries = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (entries.Length is 0) {
                return [];
            }

            var arr = new string[entries.Length];
            for (int i = 0; i < arr.Length; i++) {
                var entry = entries[i];
                if (!int.TryParse(entry, out var selected) || selected < 1 || selected > choices.Count) {
                    return [];
                }

                selected--;
                arr[i] = choices[selected];
            }

            return arr;
        }

        /// <summary>
        /// Enumerates a menu containing main option as well as sub options and allows the user to select both.
        /// <para>
        /// This function is great where more options or categories are required than <see cref="Selection"/> can provide.
        /// </para>
        /// </summary>
        /// <param name="menu">A nested dictionary containing menu titles</param>
        /// <param name="handler">title</param>
        /// <returns>The selected main option and selected sub option</returns>
        /// <remarks>
        /// This validates the input for you.
        /// </remarks>
        public static (string option, string subOption) TreeMenu<TList>(Dictionary<string, TList> menu, [InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) where TList : IList<string> {
            handler.ResetColors();
            handler.AppendNewLine();

            var menuKeys = menu.Keys.ToArray();
            var maxMainOption = menuKeys.Max(static x => x.Length) + 10; // Used to make sub-tree prefix spaces uniform

            var pool = ArrayPool<char>.Shared;
            var width = ConsoleContext.GetWidthOrDefault();
            var array = pool.Rent(width);
            try {
                var span = new Span<char>(array);

                //Enumerate options and sub-options
                for (int i = 0; i < menuKeys.Length; i++) {
                    var mainEntry = menuKeys[i];
                    var subChoices = menu[mainEntry];

                    span.TryWrite($"  {i + 1}) {mainEntry}", out int written);
                    ConsoleContext.Out.Write(span.Slice(0, written));

                    var remainingLength = maxMainOption - written;
                    if (remainingLength > 0) {
                        ConsoleContext.Out.WriteWhiteSpaces(remainingLength);
                    }

                    for (int j = 0; j < subChoices.Count; j++) {
                        if (j is not 0) {
                            ConsoleContext.Out.WriteWhiteSpaces(maxMainOption);
                        }

                        span.TryWrite($"  {j + 1}) {subChoices[j]}", out written);
                        ConsoleContext.Out.WriteLine(span.Slice(0, written));
                    }

                    Console.NewLine();
                }
            } finally {
                pool.Return(array);
            }

            string input = Console.ReadLine(string.Empty, $"Enter your main choice and sub choice separated with space: ");

            var selected = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (selected.Length is not 2) {
                throw new ArgumentException("Invalid input, must have 2 selections");
            }

            // Validate
            if (!int.TryParse(selected[0], out var mainNum) || mainNum < 1 || mainNum > menuKeys.Length) {
                throw new ArgumentException(nameof(mainNum));
            }

            var mainChoice = menuKeys[mainNum - 1];

            if (!int.TryParse(selected[1], out var subNum) || subNum < 1 || subNum > menu[mainChoice].Count) {
                throw new ArgumentException(nameof(subNum));
            }

            var subChoice = menu[mainChoice][subNum - 1];

            return (mainChoice, subChoice);
        }

        /// <summary>
        /// Draws a table
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="columns"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Table<TList>(TList headers, ReadOnlySpan<TList> columns) where TList : IList<string> {
            if (headers.Count != columns.Length) {
                throw new ArgumentException("Headers and columns must be of the same length");
            }

            const char rowSeparator = '-';
            const string columnSeparator = " | ";

            Span<int> lengths = stackalloc int[columns.Length];
            for (int i = 0; i < lengths.Length; i++) {
                lengths[i] = columns[i].Max(y => y.Length);
            }

            var height = int.MinValue;
            foreach (var column in columns) {
                if (column.Count > height) {
                    height = column.Count;
                }
            }


            var columnsLength = columns.Length;
            List<string> buffer = new(columnsLength);

            for (int i = 0; i < columnsLength; i++) {
                buffer.Add(headers[i].PadRight(lengths[i]));
            }

            var header = string.Join(columnSeparator, buffer);
            buffer.Clear();

            Span<char> rowSeparation = stackalloc char[header.Length];
            rowSeparation.Fill(rowSeparator);

            ConsoleContext.Out.WriteLine(header);
            ConsoleContext.Out.WriteLine(rowSeparation);
            for (int row = 0; row < height; row++) {
                for (int i = 0; i < columnsLength; i++) {
                    buffer.Add(columns[i][row].PadRight(lengths[i]));
                }

                var line = string.Join(columnSeparator, buffer);
                buffer.Clear();
                ConsoleContext.Out.WriteLine(line);
            }

            ConsoleContext.Out.WriteLine(rowSeparation);
        }
    }
}