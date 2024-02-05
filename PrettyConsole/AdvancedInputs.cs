using System;
using System.Diagnostics.Contracts;

using PrettyConsole.Models;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Used to wait for user input, you can customize <paramref name="message"/> or leave as default
    /// </summary>
    /// <param name="message"><ogConsole>Default value:</ogConsole> "Press any key to continue"</param>
    public static void RequestAnyInput(string message = "Press any key to continue...") {
        try {
            Write(new TextRenderingScheme((message, Colors.Default), ("... ", Colors.Highlight)));
            ogConsole.ForegroundColor = Colors.Input;
            _ = ogConsole.ReadKey();
            ogConsole.ForegroundColor = Colors.Default;
        } finally {
            ogConsole.ResetColor();
        }
    }

    public static void RequestAnyInput(ColoredOutput output) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Used to get user confirmation
    /// </summary>
    /// <param name="message"></param>
    /// <remarks>
    /// The user can confirm by entering <ogConsole>"y"</ogConsole>/<ogConsole>"yes"</ogConsole> or just pressing <ogConsole>enter</ogConsole>, anything else is regarded as <c>false</c>.
    /// </remarks>
    public static bool Confirm(string message) {
        try {
            Write(new TextRenderingScheme((message, Colors.Default), ("? ", Colors.Highlight), ("[", Colors.Default), ("y", Colors.Success),
                ("/", Colors.Default), ("n", Colors.Error), ("]: ", Colors.Default)));
            ogConsole.ForegroundColor = Colors.Input;
            var input = ogConsole.ReadLine();
            return string.IsNullOrEmpty(input) || input.ToLower() is "y" or "yes";
        } finally {
            ogConsole.ResetColor();
        }
    }
}