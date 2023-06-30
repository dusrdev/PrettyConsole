using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using PrettyConsole.Models;

namespace PrettyConsole;

public static partial class Console {
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
            WriteLine(new TextRenderingScheme((string.Concat("\t", i.ToString()), Colors.Highlight),
                                              (string.Concat(". ", choice), Colors.Default)));
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
            WriteLine(new TextRenderingScheme((string.Concat("\t", i.ToString()), Colors.Highlight),
                                              (string.Concat(". ", choice), Colors.Default)));
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
    public static (string option, string subOption) TreeMenu(string title,
                                                             Dictionary<string, List<string>> menu) {
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
            Write(new TextRenderingScheme((i.ToString(), Colors.Highlight),
                                          (string.Concat(
                                              ". ",
                                              Extensions.SuffixWithSpaces(mainChoice, maxMainOption -           prefixLength)), Colors.Default)));
            foreach (var subChoice in subChoices) {
                lst.Add(j);
                if (j is 1) {
                    WriteLine(new TextRenderingScheme((j.ToString(), Colors.Highlight),
                                                      (string.Concat(". ", subChoice), Colors.Default)));
                } else {
                    WriteLine(new TextRenderingScheme((string.Concat(Extensions.SuffixWithSpaces(null, maxMainOption), j), Colors.Highlight), (string.Concat(". ", subChoice),
                        Colors.Default)));
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

        if (selected.Length is not 2) {
            throw new ArgumentException("Invalid input, must have 2 selections");
        }

        var (sub, main) = (selected[0], selected[1]);

        // Validate
        if (!int.TryParse(main, out var mainNum) || !dict.ContainsKey(mainNum)) {
            throw new ArgumentException(nameof(mainNum));
        }

        if (!int.TryParse(sub, out var subNum) || !dict[mainNum].Contains(subNum)) {
            throw new ArgumentException(nameof(subNum));
        }

        var selectedMainOption = menuKeys[mainNum - 1];
        var selectedSubOption = menu[selectedMainOption][subNum - 1];
        return (selectedMainOption, selectedSubOption);
    }
}