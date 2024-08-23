# PrettyConsole

An abstraction over `System.Console` that adds new input and output methods, colors and advanced outputs like progress bars and menus. And everything is ansi supported so it works on legacy systems and terminals.

## Features

* 🚀 High performance, Low memory usage and allocation
* 🪶 Very lightweight (No external dependencies)
* Easy to use (no need to learn a new syntax while still writing less boilerplate code)
* 🔌 Plug and play (most of the time you don't need to change your code much)
* 💾 Supports legacy ansi terminals (like Windows 7)
* ✂ Trimming friendly (documented trim warnings and working alternatives for everything)
* Supports all platforms (Windows, Linux, Mac)
* ⛓ Uses original output pipes, so that your cli's can be piped properly.

## Usage

The most convenient way to use this package is to add this using statement: `using static PrettyConsole.Console;`
Then use most of the default method signatures such as `Write`, `WriteLine`, `ReadLine` and so on, conveniently they are named exactly the same as the regular C# counterparts.

For more information about these methods and additional ones, check the [Wiki](https://github.com/dusrdev/PrettyConsole/wiki).

## ColoredOutput

PrettyConsole uses an equation inspired syntax to colorize text. The syntax is as follows:

```csharp
WriteLine("Test" * Color.Red / Color.Blue);
```

i.e `TEXT * FOREGROUND / BACKGROUND`

Any the 2 colors can be played with just like a real equation, omit the foreground and the default will be used,
same goes for the background.
