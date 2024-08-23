# PrettyConsole

An abstraction over `System.Console` that adds new input and output methods, colors and advanced outputs like progress bars and menus. And everything is ansi supported so it works on legacy systems and terminals.

## Features

* 🚀 High performance, Low memory usage and optimized to reduce GC overhead.
* 🪶 Very lightweight (No external dependencies)
* 🔌 Plug and play (most of the time you don't need to change your code much)
* 💾 Supports legacy ansi terminals (like Windows 7)
* ✂ Trimming friendly (documented trim warnings and working alternatives for everything)
* Supports all platforms (Windows, Linux, Mac)
* ⛓ Uses original output pipes, so that your cli's can be piped properly.

**If you have any other suggestions or feature requests, please open an issue on [GitHub](https://github.com/dusrdev/PrettyConsole/issues)**

## ⬇ Installation

To keep build pipelines simple, since version 1.5.2, the new packages will only be available in **Nuget**.

* Nuget   [![NUGET DOWNLOADS](https://img.shields.io/nuget/dt/PrettyConsole?label=Downloads)](https://www.nuget.org/packages/PrettyConsole/)

## 📺 Output Examples

![Github combo](https://user-images.githubusercontent.com/8972626/205510891-3f53e471-b731-4ce1-82aa-90e1f0015961.png)

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

## Source Code

from the point of public release, the master branch will only contain stable and tested code, so
to get the source code you can clone the master branch.
