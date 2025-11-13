# PrettyConsole

PrettyConsole is a high-performance, allocation-conscious extension layer over `System.Console`. The library uses C# extension members (`extension(Console)`) so every API lights up directly on `System.Console` once `using PrettyConsole;` (and optionally `using static System.Console;`) is in scope. It targets **.NET 10.0**, is trimming/AOT ready, preserves SourceLink metadata, and keeps the familiar console experience while adding structured rendering, menus, progress bars, and advanced input helpers.

## Features

* 🚀 Zero-allocation interpolated string handler (`PrettyConsoleInterpolatedStringHandler`) for inline colors and formatting
* 🎨 Inline color composition with `ConsoleColor` tuples and helpers (`DefaultForeground`, `DefaultBackground`, `Default`)
* 🔁 Advanced rendering primitives (`Overwrite`, `ClearNextLines`, `GoToLine`, progress bars) that respect console pipes
* 🧰 Rich input helpers (`TryReadLine`, `Confirm`, `RequestAnyInput`) with `IParsable<T>` and enum support
* ⚙️ Allocation-conscious span-first APIs (`ISpanFormattable`, `ReadOnlySpan<char>`, `TextWriter.WriteWhiteSpaces`)
* ⛓ Output routing through `OutputPipe.Out` and `OutputPipe.Error` so piping/redirects continue to work

## Installation

```bash
dotnet add package PrettyConsole
```

## Usage

### Bring PrettyConsole APIs into scope

```csharp
using PrettyConsole;          // Extension members + OutputPipe
using static System.Console;  // Optional for terser call sites
```

This setup lets you call `Console.WriteInterpolated`, `Console.Overwrite`, `Console.TryReadLine`, etc. The original `System.Console` APIs remain available—call `System.Console.ReadKey()` or `System.Console.SetCursorPosition()` directly whenever you need something the extensions do not provide.

### Interpolated strings & inline colors

`PrettyConsoleInterpolatedStringHandler` streams interpolated content directly to the selected pipe without allocating. Colors auto-reset at the end of each call.

```csharp
Console.WriteInterpolated($"Hello {ConsoleColor.Green / ConsoleColor.DefaultBackground}world{ConsoleColor.Default}!");
Console.WriteInterpolated(OutputPipe.Error, $"{ConsoleColor.Yellow / ConsoleColor.DefaultBackground}warning{ConsoleColor.Default}: {message}");

if (!Console.TryReadLine(out int choice, $"Pick option {ConsoleColor.Cyan / ConsoleColor.DefaultBackground}1-5{ConsoleColor.Default}: ")) {
    Console.WriteLineInterpolated($"{ConsoleColor.Red / ConsoleColor.DefaultBackground}Not a number.{ConsoleColor.Default}");
}
```

`ConsoleColor.DefaultForeground`, `ConsoleColor.DefaultBackground`, and the `/` operator overload make it easy to compose foreground/background tuples inline (`ConsoleColor.Red / ConsoleColor.White`).

#### Formatting & alignment helpers

- **`TimeSpan :hr` format** — the interpolated string handler understands the custom `:hr` specifier. It renders the span using the most appropriate unit (e.g., `950ms`, `12s`, `03m`, `02h`, `1d`) without allocating temporaries:

  ```csharp
  var elapsed = stopwatch.Elapsed;
  Console.WriteInterpolated($"Completed in {elapsed:hr}");
  ```

- **Alignment** — standard alignment syntax works the same way it does with regular interpolated strings, but the handler writes directly into the console buffer. This keeps columnar output zero-allocation friendly:

  ```csharp
  Console.WriteInterpolated($"|{"Label",-10}|{value,10:0.00}|");
  ```

You can combine both, e.g., `$"{elapsed,8:hr}"`, to keep progress/status displays tidy.

### Basic outputs

```csharp
// Interpolated text
Console.WriteInterpolated($"Processed {items} items in {elapsed:hr}");
Console.WriteLineInterpolated(OutputPipe.Error, $"{ConsoleColor.Magenta}debug{ConsoleColor.Default}");

// Span + color overloads (no boxing)
ReadOnlySpan<char> header = "Title";
Console.Write(header, OutputPipe.Out, ConsoleColor.White, ConsoleColor.DarkBlue);
Console.NewLine(); // writes newline to the default output pipe

// ISpanFormattable (works with ref structs)
Console.Write(percentage, OutputPipe.Out, ConsoleColor.Cyan, ConsoleColor.DefaultBackground, format: "F2", formatProvider: null);
```

Behind the scenes these overloads rent buffers via `BufferPool` and route output to the correct pipe through `PrettyConsoleExtensions.GetWriter`.

### Basic inputs

```csharp
if (!Console.TryReadLine(out int port, $"Port ({ConsoleColor.Green}5000{ConsoleColor.Default}): ")) {
    port = 5000;
}

// `TryReadLine<TEnum>` and `TryReadLine` with defaults
if (!Console.TryReadLine(out DayOfWeek day, ignoreCase: true, $"Day? ")) {
    day = DayOfWeek.Monday;
}

var apiKey = Console.ReadLine($"Enter API key ({ConsoleColor.DarkGray}optional{ConsoleColor.Default}): ");
```

All input helpers work with `IParsable<T>` and enums, respect the active culture, and honor `OutputPipe` when prompts are colored.

### Advanced inputs

```csharp
Console.RequestAnyInput($"Press {ConsoleColor.Yellow}any key{ConsoleColor.Default} to continue…");

if (!Console.Confirm($"Deploy to production? ({ConsoleColor.Green}y{ConsoleColor.Default}/{ConsoleColor.Red}n{ConsoleColor.Default}) ")) {
    return;
}

var customTruths = new[] { "sure", "do it" };
bool overwrite = Console.Confirm(customTruths, emptyIsTrue: false, $"Overwrite existing files? ");
```

### Rendering helpers

```csharp
Console.ClearNextLines(3, OutputPipe.Error);
int line = Console.GetCurrentLine();
// … draw something …
Console.GoToLine(line);
Console.SetColors(ConsoleColor.White, ConsoleColor.DarkBlue);
Console.ResetColors();
```

`PrettyConsoleExtensions.Out`/`Error` expose the live writers. Each writer now has `WriteWhiteSpaces(int)` for zero-allocation padding:

```csharp
PrettyConsoleExtensions.Error.WriteWhiteSpaces(8); // pad status blocks
```

### Advanced outputs

```csharp
Console.Overwrite(() => {
    Console.WriteLineInterpolated(OutputPipe.Error, $"{ConsoleColor.Cyan}Working…{ConsoleColor.Default}");
    Console.WriteInterpolated(OutputPipe.Error, $"{ConsoleColor.DarkGray}Elapsed:{ConsoleColor.Default} {stopwatch.Elapsed:hr}");
}, lines: 2);

// Prevent closure allocations with state + generic overload
Console.Overwrite((left, right), tuple => {
    Console.WriteInterpolated($"{tuple.left} ←→ {tuple.right}");
}, lines: 1);

await Console.TypeWrite("Booting systems…", (ConsoleColor.Green, ConsoleColor.Black));
await Console.TypeWriteLine("Ready.", ConsoleColor.Default);
```

Always call `Console.ClearNextLines(totalLines, pipe)` once after the last `Overwrite` to erase the region when you are done.

### Menus and tables

```csharp
var choice = Console.Selection("Pick an environment:", new[] { "Dev", "QA", "Prod" });
var multi = Console.MultiSelection("Services to restart:", new[] { "API", "Worker", "Scheduler" });
var (area, action) = Console.TreeMenu("Actions", new Dictionary<string, IList<string>> {
    ["Users"] = new[] { "List", "Create", "Disable" },
    ["Jobs"] = new[] { "Queue", "Retry" }
});

Console.Table(
    headers: new[] { "Name", "Status" },
    columns: new[] {
        new[] { "API", "Worker" },
        new[] { "Running", "Stopped" }
    }
);
```

Menus validate user input (throwing `ArgumentException` on invalid selections) and use the padding helpers internally to keep columns aligned.

### Progress bars

```csharp
using var progress = new ProgressBar {
    ProgressChar = '■',
    ForegroundColor = ConsoleColor.DarkGray,
    ProgressColor = ConsoleColor.Green,
};

for (int i = 0; i <= 100; i += 5) {
    progress.Update(i, $"Downloading chunk {i / 5}");
    await Task.Delay(50);
}

// Need separate status + bar lines? sameLine: false
progress.Update(42.5, "Syncing", sameLine: false);

// One-off render without state
ProgressBar.WriteProgressBar(OutputPipe.Error, 75, ConsoleColor.Magenta, '*');
```

`ProgressBar.Update` always re-renders (even if the percentage didn't change) so you can refresh status text. The helper `ProgressBar.WriteProgressBar` keeps the cursor on the same line, which is ideal inside `Console.Overwrite`.

#### Multiple progress bars with tasks + channels

```csharp
using System.Linq;
using System.Threading.Channels;

var downloads = new[] { "Video.mp4", "Archive.zip", "Assets.pak" };
var progress = new double[downloads.Length];
var updates = Channel.CreateUnbounded<(int index, double percent)>();

// Producers push progress updates
var producers = downloads
    .Select((name, index) => Task.Run(async () => {
        for (int p = 0; p <= 100; p += Random.Shared.Next(5, 15)) {
            await updates.Writer.WriteAsync((index, p));
            await Task.Delay(Random.Shared.Next(40, 120));
        }
    }))
    .ToArray();

// Consumer renders stacked bars each time an update arrives
var consumer = Task.Run(async () => {
    await foreach (var (index, percent) in updates.Reader.ReadAllAsync()) {
        progress[index] = percent;

        Console.Overwrite(progress, state => {
            for (int i = 0; i < state.Length; i++) {
                Console.WriteInterpolated(OutputPipe.Error, $"Task {i + 1} ({downloads[i]}): ");
                ProgressBar.WriteProgressBar(OutputPipe.Error, state[i], ConsoleColor.Cyan);
            }
        }, lines: downloads.Length, pipe: OutputPipe.Error);
    }
});

await Task.WhenAll(producers);
updates.Writer.Complete();
await consumer;

Console.ClearNextLines(downloads.Length, OutputPipe.Error); // ensure no artifacts remain
```

Each producer reports progress over the channel, the consumer loops with `ReadAllAsync`, and `Console.Overwrite` redraws the stacked bars on every update. After the consumer completes, clear the region once to remove the progress UI.

### Pipes & writers

PrettyConsole keeps the original console streams accessible:

```csharp
TextWriter @out = PrettyConsoleExtensions.Out;
TextWriter @err = PrettyConsoleExtensions.Error;
TextReader @in = PrettyConsoleExtensions.In;
```

Use these when you need direct writer access (custom buffering, `WriteWhiteSpaces`, etc.). In cases where you must call raw `System.Console` APIs (e.g., `Console.ReadKey(true)`), do so explicitly—PrettyConsole never hides the built-in console.

## Contributing

Contributions are welcome! Fork the repo, create a branch, and open a pull request. Bug reports and feature requests are tracked through GitHub issues.

## Contact

For bug reports, feature requests, or sponsorship inquiries reach out at <dusrdev@gmail.com>.

> This project is proudly made in Israel 🇮🇱 for the benefit of mankind.
