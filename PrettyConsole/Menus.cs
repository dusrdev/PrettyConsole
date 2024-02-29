using System.Buffers;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Enumerates a list of strings and allows the user to select one by number, and uses the default index color (White)
    /// </summary>
    /// <param name="title"/>
    /// <param name="choices">Any collection of strings</param>
    /// <returns>The selected string, or null if the choice was invalid.</returns>
    /// <remarks>
    /// This validates the input for you.
    /// </remarks>
    public static string? Selection(ColoredOutput title, IList<string> choices)
    => Selection(title, ConsoleColor.White, choices);

    /// <summary>
    /// Enumerates a list of strings and allows the user to select one by number
    /// </summary>
    /// <param name="title"/>
    /// <param name="indexForeground"></param>
    /// <param name="choices">Any collection of strings</param>
    /// <returns>The selected string, or null if the choice was invalid.</returns>
    /// <remarks>
    /// This validates the input for you.
    /// </remarks>
    public static string? Selection(ColoredOutput title, ConsoleColor indexForeground, IList<string> choices) {
        if (title.Value.Length is not 0) {
            WriteLine(title);
        }

        Span<char> buffer = stackalloc char[ogConsole.BufferWidth];

        for (int i = 0; i < choices.Count; i++) {
            (i + 1).TryFormat(buffer, out int numWritten, "  0) ");
            ogConsole.ForegroundColor = indexForeground;
            ogConsole.Out.Write(buffer[..numWritten]);
            ResetColors();
            WriteLine(choices[i]);
        }
        NewLine();

        if (!TryReadLine("Enter your choice: ", out int selected)) {
            return null;
        }

        selected--;

        if (selected < 0 || selected >= choices.Count) {
            return null;
        }

        return choices[selected];
    }

    /// <summary>
    /// Enumerates a list of strings and allows the user to select multiple strings by any order, and uses the default index color (White)
    /// </summary>
    /// <param name="title"><ogConsole>Optional</ogConsole>, null or whitespace will not be displayed</param>
    /// <param name="choices">Any collection of strings</param>
    /// <returns>A list containing any selected choices by order of selection, or default if choice is invalid</returns>
    /// <remarks>
    /// This validates the input for you.
    /// </remarks>
    public static string[] MultiSelection(ColoredOutput title, IList<string> choices)
    => MultiSelection(title, ConsoleColor.White, choices);

    /// <summary>
    /// Enumerates a list of strings and allows the user to select one by number
    /// </summary>
    /// <param name="title"/>
    /// <param name="indexForeground"></param>
    /// <param name="choices">Any collection of strings</param>
    /// <returns>The selected string, or null if the choice was invalid.</returns>
    /// <remarks>
    /// This validates the input for you.
    /// </remarks>
    public static string[] MultiSelection(ColoredOutput title, ConsoleColor indexForeground, IList<string> choices) {
        if (title.Value.Length is not 0) {
            WriteLine(title);
        }

        Span<char> buffer = stackalloc char[ogConsole.BufferWidth];

        for (int i = 0; i < choices.Count; i++) {
            (i + 1).TryFormat(buffer, out int numWritten, "  0) ");
            ogConsole.ForegroundColor = indexForeground;
            ogConsole.Out.Write(buffer[..numWritten]);
            ResetColors();
            WriteLine(choices[i]);
        }
        NewLine();

        var input = ReadLine("Enter your choices separated with spaces: ");

        if (string.IsNullOrWhiteSpace(input)) {
            return Array.Empty<string>();
        }

        var entries = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (entries.Length is 0) {
            return Array.Empty<string>();
        }

        var arr = GC.AllocateUninitializedArray<string>(entries.Length);
        for (int i = 0; i < arr.Length; i++) {
            var entry = entries[i];
            if (!int.TryParse(entry, out var selected) || selected < 1 || selected > choices.Count) {
                return Array.Empty<string>();
            }
            selected--;
            arr[i] = choices[selected];
        }
        return arr;
    }

    /// <summary>
    /// Enumerates a menu containing main option as well as sub options and allows the user to select both. Uses the default index color (White)
    /// <para>
    /// This function is great where more options or categories are required than <see cref="Selection(ColoredOutput, IList{string})"/> can provide.
    /// </para>
    /// </summary>
    /// <param name="title"><ogConsole>Optional</ogConsole>, null or whitespace will not be displayed</param>
    /// <param name="menu">A nested dictionary containing menu titles</param>
    /// <returns>The selected main option and selected sub option</returns>
    /// <remarks>
    /// This validates the input for you.
    /// </remarks>
    public static (string option, string subOption) TreeMenu<T>(ColoredOutput title,
                                                             Dictionary<string, T> menu) where T : IList<string>
    => TreeMenu(title, ConsoleColor.White, menu);

    /// <summary>
    /// Enumerates a menu containing main option as well as sub options and allows the user to select both.
    /// <para>
    /// This function is great where more options or categories are required than <see cref="Selection(ColoredOutput, IList{string})"/> can provide.
    /// </para>
    /// </summary>
    /// <param name="title"><ogConsole>Optional</ogConsole>, null or whitespace will not be displayed</param>
    /// <param name="indexForeground"></param>
    /// <param name="menu">A nested dictionary containing menu titles</param>
    /// <returns>The selected main option and selected sub option</returns>
    /// <remarks>
    /// This validates the input for you.
    /// </remarks>
    public static (string option, string subOption) TreeMenu<T>(ColoredOutput title,
                                                            ConsoleColor indexForeground,
                                                             Dictionary<string, T> menu) where T : IList<string> {
        if (title.Value.Length is not 0) {
            WriteLine(title);
            NewLine();
        }
        var menuKeys = menu.Keys.ToArray();
        var maxMainOption = menuKeys.Max(static x => x.Length) + 10; // Used to make sub-tree prefix spaces uniform

        Span<char> buffer = stackalloc char[ogConsole.BufferWidth];
        Span<char> emptySpaces = stackalloc char[maxMainOption];
        emptySpaces.Fill(' ');

        //Enumerate options and sub-options
        for (int i = 0; i < menuKeys.Length; i++) {
            var mainEntry = menuKeys[i];
            var subChoices = menu[mainEntry];
            var lst = new List<int>();
            var prefixLength = i.ToString().Length + 2;
            (i + 1).TryFormat(buffer, out int mainIndexWritten, "0) ");
            var mainIndex = buffer[..mainIndexWritten];
            ogConsole.ForegroundColor = indexForeground;
            ogConsole.Out.Write(mainIndex);
            ResetColors();
            // Find a way to pad to a buffer
            ogConsole.Out.Write(mainEntry);
            var remainingLength = maxMainOption - mainIndex.Length - mainEntry.Length;
            ogConsole.Out.Write(emptySpaces[..remainingLength]);
            for (int j = 0; j < subChoices.Count; j++) {
                if (j is not 0) {
                    ogConsole.Out.Write(emptySpaces);
                }
                (j + 1).TryFormat(buffer, out int numWritten, "0) ");
                ogConsole.ForegroundColor = indexForeground;
                ogConsole.Out.Write(buffer[..numWritten]);
                ResetColors();
                WriteLine(subChoices[j]);
            }
            NewLine();
        }

        var input = ReadLine("Enter your main choice and sub choice separated with space: ") ?? "";

        var selected = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (selected.Length is not 2) {
            throw new ArgumentException("Invalid input, must have 2 selections");
        }

        var (sub, main) = (selected[0], selected[1]);

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
    public static void Table<T>(T headers, ReadOnlySpan<T> columns) where T : IList<string> {
        if (headers.Count != columns.Length) {
            throw new ArgumentException("Headers and columns must be of the same length");
        }

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

        var headersArray = ArrayPool<string>.Shared.Rent(columns.Length);
        var headersSegment = new ArraySegment<string>(headersArray, 0, columns.Length);
        for (int i = 0; i < headersSegment.Count; i++) {
            headersSegment[i] = headers[i].PadRight(lengths[i]);
        }
        var header = string.Join(string.Intern(" | "), headersSegment);
        ArrayPool<string>.Shared.Return(headersArray);
        WriteLine(header);
        Span<char> buffer = stackalloc char[header.Length];
        buffer.Fill('-');
        ogConsole.Out.WriteLine(buffer);
        for (int row = 0; row < height; row++) {
            var rowsArray = ArrayPool<string>.Shared.Rent(columns.Length);
            var rowValues = new ArraySegment<string>(rowsArray, 0, columns.Length);
            for (int i = 0; i < columns.Length; i++) {
                rowValues[i] = columns[i][row].PadRight(lengths[i]);
            }
            var line = string.Join(string.Intern(" | "), rowValues);
            ArrayPool<string>.Shared.Return(rowsArray);
            WriteLine(line);
        }

        ogConsole.Out.WriteLine(buffer);
    }
}