using System.Buffers;
using System.Runtime.InteropServices;

using Sharpify.Collections;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Enumerates a list of strings and allows the user to select one by number
    /// </summary>
    /// <param name="title"/>
    /// <param name="choices">Any collection of strings</param>
    /// <returns>The selected string, or empty if the choice was invalid.</returns>
    /// <remarks>
    /// This validates the input for you.
    /// </remarks>
    public static string Selection<TList>(ReadOnlySpan<ColoredOutput> title, TList choices)
        where TList : IList<string> {
        WriteLine(title);

        Span<char> buffer = stackalloc char[baseConsole.BufferWidth];

        for (int i = 0; i < choices.Count; i++) {
            var builder = StringBuffer.Create(buffer);
            builder.Append("  ");
            builder.Append(i + 1);
            builder.Append(") ");
            builder.Append(choices[i]);
            Out.WriteLine(builder.WrittenSpan);
        }

        NewLine();

        if (!TryReadLine(["Enter your choice: "], out int selected)) {
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
    /// <param name="title"><baseConsole>Optional</baseConsole>, null or whitespace will not be displayed</param>
    /// <param name="choices">Any collection of strings</param>
    /// <returns>An array containing any selected choices by order of selection, or empty array if any choice is invalid</returns>
    /// <remarks>
    /// This validates the input for you.
    /// </remarks>
    public static string[] MultiSelection<TList>(ReadOnlySpan<ColoredOutput> title, TList choices)
        where TList : IList<string> {
        WriteLine(title);

        Span<char> buffer = stackalloc char[baseConsole.BufferWidth];

        for (int i = 0; i < choices.Count; i++) {
            var builder = StringBuffer.Create(buffer);
            builder.Append("  ");
            builder.Append(i + 1);
            builder.Append(") ");
            builder.Append(choices[i]);
            Out.WriteLine(builder.WrittenSpan);
        }

        NewLine();

        var input = ReadLine(["Enter your choices separated with spaces: "]);

        if (string.IsNullOrWhiteSpace(input)) {
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
    /// This function is great where more options or categories are required than <see cref="Selection{TList}(ReadOnlySpan{ColoredOutput}, TList)"/> can provide.
    /// </para>
    /// </summary>
    /// <param name="title"><baseConsole>Optional</baseConsole>, null or whitespace will not be displayed</param>
    /// <param name="menu">A nested dictionary containing menu titles</param>
    /// <returns>The selected main option and selected sub option</returns>
    /// <remarks>
    /// This validates the input for you.
    /// </remarks>
    public static (string option, string subOption) TreeMenu<TList>(ReadOnlySpan<ColoredOutput> title,
        Dictionary<string, TList> menu) where TList : IList<string> {
        WriteLine(title);
        NewLine();

        var menuKeys = menu.Keys.ToArray();
        var maxMainOption = menuKeys.Max(static x => x.Length) + 10; // Used to make sub-tree prefix spaces uniform

        Span<char> buffer = stackalloc char[baseConsole.BufferWidth];
        Span<char> emptySpaces = stackalloc char[maxMainOption];
        emptySpaces.Fill(' ');

        //Enumerate options and sub-options
        for (int i = 0; i < menuKeys.Length; i++) {
            var mainEntry = menuKeys[i];
            var subChoices = menu[mainEntry];
            var builder = StringBuffer.Create(buffer);
            builder.Append("  ");
            builder.Append(i + 1);
            builder.Append(") ");
            builder.Append(mainEntry);
            var remainingLength = maxMainOption - builder.Position;
            builder.Append(emptySpaces.Slice(0, remainingLength));
            Out.Write(builder.WrittenSpan);
            for (int j = 0; j < subChoices.Count; j++) {
                if (j is not 0) {
                    Out.Write(emptySpaces);
                }

                builder.Reset();
                builder.Append("  ");
                builder.Append(j + 1);
                builder.Append(") ");
                builder.Append(subChoices[j]);
                Out.WriteLine(builder.WrittenSpan);
            }

            NewLine();
        }

        var input = ReadLine(["Enter your main choice and sub choice separated with space: "]) ?? "";

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
        using var memoryOwner = MemoryPool<string>.Shared.Rent(columnsLength);

        for (int i = 0; i < columnsLength; i++) {
            memoryOwner.Memory.Span[i] = headers[i].PadRight(lengths[i]);
        }

        ReadOnlyMemory<string> slice = memoryOwner.Memory.Slice(0, columnsLength);
        var enumerable = MemoryMarshal.ToEnumerable(slice);
        var header = string.Join(columnSeparator, enumerable);

        Span<char> rowSeparation = stackalloc char[header.Length];
        rowSeparation.Fill(rowSeparator);

        Out.WriteLine(header);
        Out.WriteLine(rowSeparation);
        for (int row = 0; row < height; row++) {
            for (int i = 0; i < columnsLength; i++) {
                memoryOwner.Memory.Span[i] = columns[i][row].PadRight(lengths[i]);
            }

            slice = memoryOwner.Memory.Slice(0, columnsLength);
            enumerable = MemoryMarshal.ToEnumerable(slice);
            var line = string.Join(columnSeparator, enumerable);
            Out.WriteLine(line);
        }

        Out.WriteLine(rowSeparation);
    }
}