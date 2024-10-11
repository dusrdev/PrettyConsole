# PrettyConsole

An abstraction over `System.Console` that adds new input and output methods, colors and advanced outputs like progress bars and menus. And everything is ansi supported so it works on legacy systems and terminals.

## Features

* 🚀 High performance, Low memory usage and allocation
* 🪶 Very lightweight (No external dependencies)
* Easy to use (no need to learn a new syntax while still writing less boilerplate code)
* 💾 Supports legacy ansi terminals (like Windows 7)
* 🔥 Complete NativeAOT compatibility
* Supports all major platforms (Windows, Linux, Mac)
* ⛓ Uses original output pipes, so that your cli's can be piped properly.

## Installation [![NUGET DOWNLOADS](https://img.shields.io/nuget/dt/PrettyConsole?label=Downloads)](https://www.nuget.org/packages/PrettyConsole/)

> dotnet add package PrettyConsole

## Usage

Everything starts off with the using statements, I recommend using the `Console` statically

```csharp
using static PrettyConsole.Console; // Access to all Console methods
using PrettyConsole; // Access to the Color struct
```

### ColoredOutput

PrettyConsole uses an equation inspired syntax to colorize text. The syntax is as follows:

```csharp
WriteLine("Test" * Color.Red / Color.Blue);
```

i.e `TEXT * FOREGROUND / BACKGROUND`

Any the 2 colors can be played with just like a real equation, omit the foreground and the default will be used,
same goes for the background.

### Basic Outputs

The most basic method for outputting is `Write`, which has multiple overloads:

```csharp
Write(ColoredOutput);
Write(ReadOnlySpan<ColoredOutput>); // use collections expression for the compiler to inline the array
Write(ReadOnlySpan<char>, ConsoleColor); // no string allocation with ReadOnlySpan<char>
Write(ReadOnlySpan<char>, ConsoleColor, ConsoleColor);
Write(T, ConsoleColor); // no string allocation with T : ISpanFormattable
Write(T, ConsoleColor, ConsoleColor);
Write(T, ConsoleColor, ConsoleColor, ReadOnlySpan<char>, IFormatProvider?);
```

There are also overloads for `WriteLine` and `WriteError`, `WriteError` has the same overloads exactly as `Write` but uses the `Error` stream, this means you can output metrics or whatever, and if you pipe the output to another cli, it will be displayed correctly. `WriteLine` only has overloads for `ColoredOutput` and `ReadOnlySpan<ColoredOutput>`. But you can use `Write` followed by `NewLine` to bridge the gap.

### Basic Inputs

These are the methods for reading user input:

```csharp
string? ReadLine(); // ReadLine<string>
string? ReadLine(ReadOnlySpan<ColoredOutput>);
T? ReadLine<T>(ReadOnlySpan<ColoredOutput>); // T : IParsable<T>
T ReadLine<T>(ReadOnlySpan<ColoredOutput>, T @default); // @default will be returned if parsing fails
bool TryReadLine<T>(ReadOnlySpan<ColoredOutput>, out T?); // T : IParsable<T>
bool TryReadLine<T>(ReadOnlySpan<ColoredOutput>, T @default, out T); // @default will be returned if parsing fails
bool TryReadLine<TEnum>(ReadOnlySpan<ColoredOutput>, bool ignoreCase, out TEnum?); // TEnum : struct, Enum
bool TryReadLine<TEnum>(ReadOnlySpan<ColoredOutput>, bool ignoreCase, TEnum @default, out TEnum); // @default will be returned if parsing fails
```

I always recommend using `TryReadLine` instead of `ReadLine` as you need to maintain less null checks and the result,
especially with `@default` is much more concise.

### Advanced Inputs

These are some special methods for inputs:

```csharp
// These will wait for the user to press any key
void RequestAnyInput(string message = "Press any key to continue...");
void RequestAnyInput(ReadOnlySpan<ColoredOutput> output);
// These request confirmation by special input from user
bool Confirm(ReadOnlySpan<ColoredOutput> message); // uses the default values ["y", "yes"]
// the default values can also be used by you at Console.DefaultConfirmValues
bool Confirm(ReadOnlySpan<ColoredOutput> message, ReadOnlySpan<string> trueValues, bool emptyIsTrue = true);
```

### Rendering Controls

To aid in rendering and building your own complex outputs, there are many methods that simplify some processes.

```csharp
ClearNextLines(int lines); // clears the next lines
ClearNextLinesError(int lines); // clears the next lines for the error stream
NewLine(); // outputs a new line
NewLineError(); // outputs a new line for the error stream
SetColors(ConsoleColor foreground, ConsoleColor background); // sets the colors of the console output
ResetColors(); // resets the colors of the console output
```

Combining `ClearNextLines` with `System.Console.SetCursorPosition` will enable you to efficiently use the same space in the console for continuous output, such as progress outputting, for some cases there are also built-in methods for this, more on that later.

### Advanced Outputs

```csharp
// This method will essentially write a line, clear it, go back to same position
// This allows a form of text-only progress bar
void OverrideCurrentLine(ReadOnlySpan<ColoredOutput> output);
// This methods will write a character at a time, with a delay between each character
async Task TypeWrite(ColoredOutput output, int delay = TypeWriteDefaultDelay);
async Task TypeWriteLine(ColoredOutput output, int delay = TypeWriteDefaultDelay);
```

### Menus

```csharp
// This prints an index view of the list, allows the user to select by index
// returns the actual choice that corresponds to the index
string Selection<TList>(ReadOnlySpan<ColoredOutput> title, TList choices) where TList : IList<string> {}
// Same as selection but allows the user to select multiple indexes
// Separated by spaces, and returns an array of the actual choices that correspond to the indexes
string[] MultiSelection<TList>(ReadOnlySpan<ColoredOutput> title, TList choices) where TList : IList<string> {}
// This prints a tree menu of 2 levels, allows the user to select the index
// Of the first and second level and returns the corresponding choices
(string option, string subOption) TreeMenu<TList>(ReadOnlySpan<ColoredOutput> title,
        Dictionary<string, TList> menu) where TList : IList<string> {}
// This prints a table with headers, and columns for each list
void Table<TList>(TList headers, ReadOnlySpan<TList> columns) where TList : IList<string> {}
```

### Progress Bars

There are two types of progress bars here, they both are implemented using a class to maintain states.

#### IndeterminateProgressBar

```csharp
var prg = new IndeterminateProgressBar(); // this setups the internal states
// Then you need to provide either a Task or Task</T>, the progress bar binds to it and runs until the task completes
await prg.RunAsync(task, cancellationToken);
// if the task is not started before being passed to the progress bar, it will be started automatically
// It is even better this way to synchronize the runtime of the progress bar with the task
```

#### ProgressBar

`ProgressBar` is a more powerful version, but requires a percentage of progress.

```csharp
// ProgressBar implements IDisposable
using var prg = new ProgressBar();
// then on each time the progress percentage is actually changed, you call Update
Update(percentage, ReadOnlySpan<char> header);
// There are also overloads without header, and percentage can be either int or double (0-100)
// Also, you can change some of the visual properties of the progress bar after initialization
// by using the properties of the ProgressBar class
prg.ProgressChar = '■'; // Character to fill the progress bar
prg.ForegroundColor = Color.Red; // Color of the empty part
prg.ProgressColor = Color.Blue; // The color of the filled part
```

### Pipes

`Console` wraps over `System.Console` and uses its `In`, `Out`, and `Error` streams. Since the names of the classes are identical, combining them in usage is somewhat painful as the compiler doesn't know which overloads to use. To aid in most cases,
`Console` exposes those streams as static properties, and you can use them directly.

In rare cases, you will need something that there is in `System.Console` but not in `Console`, such as `ReadKey`, or `SetCursorPosition`, some events or otherwise, then you can simply call `System.Console`, this added verbosity is a worthy trade-off.

## Contributing

This project uses an MIT license, if you want to contribute, you can do so by forking the repository and creating a pull request.

If you have feature requests or bug reports, please create an issue.

## Contact

For bug reports, feature requests or offers of support/sponsorship contact <dusrdev@gmail.com>

> This project is proudly made in Israel 🇮🇱 for the benefit of mankind.
