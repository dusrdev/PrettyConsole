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

## Note on possible issues with colored outputs

You might run into a case where colored output in which you customized both the foreground and background doesn't render properly, for example:

```csharp
WriteLine("Test" * Color.Red / Color.Blue);
```

This should print test with red foreground and blue background, but might use the default color for one of them instead.
For example it might print the foreground as gray (the default) and the background in blue (the right color).

**THIS IS NOT A BUG**, rather it is a way some terminal emulators handle colors, in my tests, this happens in Warp. But not in Terminal.app. So if this happens just remember that and don't try going crazy figuring out why (which happened to me 😂)

Some terminals also happen to highlight an entire character with the cursor color (instead of just staying between them - on insert mode of course), so some outputs will look weird, for example the changing character in `IndeterminateProgressBar` may have a colored background (Although it should have - Warp again...)

So generally when there are issues with colors, it most likely tied to the terminal you are using. In case you tested on multiple terminals and the issue is indeed with the package, of course you can contact me / post on the issues page. make sure to leave a detailed documentation of what you did so that I can reproduce the bug.
