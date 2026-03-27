# PrettyConsole

[![NuGet](https://img.shields.io/nuget/v/PrettyConsole.svg?style=flat-square)](https://www.nuget.org/packages/PrettyConsole)
[![NuGet Downloads](https://img.shields.io/nuget/dt/PrettyConsole?style=flat&label=Downloads)](https://www.nuget.org/packages/PrettyConsole)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](License.txt)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

PrettyConsole is a high-performance, ultra-low-latency, allocation-free extension layer over `System.Console`. The library uses C# extension members (`extension(Console)`) so every API lights up directly on `System.Console` once `using PrettyConsole;` is in scope. It is trimming/AOT ready, preserves SourceLink metadata, and keeps the familiar console experience while adding structured rendering, menus, progress bars, live console regions, and advanced input helpers.

## Features

- 🚀 Zero-allocation interpolated string handler (`PrettyConsoleInterpolatedStringHandler`) for inline colors and formatting
- 🎨 Guarded ANSI tokens for interpolation via `Color`, `Markup`, and custom `AnsiToken`, with `ConsoleColor` compatibility for APIs that explicitly require it
- 🔁 Advanced rendering primitives (`Overwrite`, `ClearNextLines`, `GoToLine`, `SkipLines`, progress bars) that respect console pipes
- 📌 `LiveConsoleRegion` for a retained live line/region that stays pinned while durable status lines stream above it on the same pipe
- 🧱 Handler-aware `WhiteSpace` struct for zero-allocation padding directly inside interpolated strings
- 🧰 Rich input helpers (`TryReadLine`, `Confirm`, `RequestAnyInput`) with `IParsable<T>` and enum support
- ⚙️ Low-level output escape hatches (`Console.Write/WriteLine` span and `ISpanFormattable` overloads) for advanced custom formatting pipelines
- ⛓ Output routing through `OutputPipe.Out` (default) and `OutputPipe.Error` so piping/redirects continue to work

## Performance

BenchmarkDotNet measures [styled output performance](Benchmarks/BenchmarkDotNet.Artifacts/results/Benchmarks.StyledOutputBenchmarks-report-github.md) for a single line write:

| Method         | Mean        | Ratio         | Gen0   | Allocated | Alloc Ratio   |
|--------------- |------------:|--------------:|-------:|----------:|--------------:|
| PrettyConsole  |    53.10 ns | 92.14x faster |      - |         - |            NA |
| SpectreConsole | 4,889.25 ns |      baseline | 2.0880 |   17840 B |               |
| SystemConsole  |    70.48 ns | 69.37x faster | 0.0022 |      24 B | 743.333x less |

PrettyConsole is **the go-to choice for ultra-low-latency, allocation-free console rendering**, running 90X faster than **Spectre.Console** while allocating nothing and even beating the manual unrolling with the BCL.

## Installation

```bash
dotnet add package PrettyConsole
```

## Included Skill

`PrettyConsole` ships a universal skill named `pretty-console-expert`. The package uses `Payload` to copy it into the consuming repository during build at `.agents/skills/pretty-console-expert`, giving the repo its own PrettyConsole expert.

Consumers can control that payload with `PayloadPolicy`:

```xml
<ItemGroup>
  <PayloadPolicy Include="PrettyConsole"
                 Tag="PrettyConsoleExpert"
                 CopyOnBuild="true" />
</ItemGroup>
```

- `CopyOnBuild="true"` is the default and keeps the skill synchronized on build. Set it to `false` to stop future copies.
- `OverridePath` is optional and changes the destination base path. The package-defined `TargetPath` stays `.agents/skills/pretty-console-expert`, so `OverridePath="/custom/root"` copies the skill to `/custom/root/.agents/skills/pretty-console-expert`.

For .NET 10 file-based apps, if repo-root detection is not available, set `PayloadRootDirectory` explicitly:

```csharp
#:property PayloadRootDirectory=.
```

## Examples

Standalone samples made with .NET 10 file-based apps with preview clips are available in [Examples](Examples/README.md).

## Usage

### Bring PrettyConsole APIs into scope

```csharp
using PrettyConsole;          // Extension members + OutputPipe
using static System.Console;  // Optional for terser call sites
using static PrettyConsole.Color; // Optional for terser color tokens
```

This setup lets you call `Console.WriteInterpolated`, `Console.Overwrite`, `Console.TryReadLine`, etc. The original `System.Console` APIs remain available—call `System.Console.ReadKey()` or `System.Console.SetCursorPosition()` directly whenever you need something the extensions do not provide.

### Interpolated strings & inline colors

`Console.WriteInterpolated` and `Console.WriteLineInterpolated` are the main output APIs for styled text. Colors reset automatically at the end of each call, and both methods return the number of visible characters written so you can reuse the result in padding or layout calculations.

For interpolation, prefer `Color` and `Markup`. `ConsoleColor` interpolation still works, but `Color` is the primary public surface for styled output.

```csharp
Console.WriteInterpolated($"Hello {Green}world{Default}!");
Console.WriteInterpolated(OutputPipe.Error, $"{Yellow}warning{Default}: {message}");

if (!Console.TryReadLine(out int choice, $"Pick option {Cyan}1-5{Default}: ")) {
    Console.WriteLineInterpolated($"{Red}Not a number.{Default}");
}

// Zero-allocation padding directly from the handler
Console.WriteInterpolated($"Header{new WhiteSpace(6)}Value");
```

`Color` exposes tokens for both foreground and background colors (`Green`, `GreenBackground`, `DefaultForeground`, `DefaultBackground`, `Default`). `AnsiColors` is available when you want to convert an existing `ConsoleColor` value into the same style of token.

For color-specific APIs that take `AnsiToken`, prefer using `Color.*`. `ConsoleColor` still works in many foreground-only call sites through implicit conversion, but `Color` is the clearer and more expressive API.

#### Inline decorations via `Markup`

`Markup` exposes ready-to-use guarded tokens for underline, bold, italic, and strikethrough:

```csharp
Console.WriteLineInterpolated($"{Markup.Bold}Build{Markup.ResetBold} {Markup.Underline}completed{Markup.ResetUnderline} in {elapsed:duration}"); // e.g. "completed in 2h 3m 17s"
```

Use `Markup.Reset` if you want to reset every decoration and color at once.

#### Formatting & alignment helpers

- **`TimeSpan :duration` format** — use the custom `:duration` specifier to render values like `5h 32m 12s`, `27h 12m 3s`, or `123h 0m 0s`. The hour component keeps growing past 24 so long-running tasks stay accurate, and minutes/seconds stay compact:

  ```csharp
  var elapsed = stopwatch.Elapsed;
  Console.WriteInterpolated($"Completed in {elapsed:duration}"); // Completed in 12h 5m 33s
  ```

- **`double :bytes` format** — pass any `double` (cast integral sizes if needed) with the `:bytes` specifier to render human-friendly binary size units. Values scale by powers of 1024 through `B`, `KB`, `MB`, `GB`, `TB`, `PB`, and use the `#,##0.##` format so thousands separators and up to two decimal digits follow the current culture:

  ```csharp
  var transferred = 12_884_901d;
  Console.WriteInterpolated($"Uploaded {transferred:bytes}"); // Uploaded 12.3 MB
  Console.WriteInterpolated($"Remaining {remaining,8:bytes}"); // right-aligned units stay tidy
  ```

- **Alignment** — standard alignment syntax works the same way it does with regular interpolated strings, so columnar console output stays straightforward:

  ```csharp
  Console.WriteInterpolated($"|{"Label",-10}|{value,10:0.00}|");
  ```

You can combine both, e.g., `$"{elapsed,8:duration}"`, to keep progress/status displays tidy.

- **`WhiteSpace` struct for padding** — pass `new WhiteSpace(length)` inside an interpolated string when you want explicit padding without building a separate string first.

- **Custom ANSI tokens** — if you need your own ANSI code for extra markup or color, wrap it in `AnsiToken` and keep it in an interpolated hole:

```csharp
var rose = new AnsiToken("\u001b[38;5;213m"); // custom 256-color escape
Console.WriteInterpolated($"{rose}accent text{Color.Default}");
```

Avoid embedding the escape directly in the literal (`"\u001b[38;5;213maccent text"`), which can interfere with width-sensitive output. If you want PrettyConsole to handle ANSI safely for you, use `Color`, `Markup`, or `AnsiToken`.

### Basic outputs

```csharp
// Interpolated text
Console.WriteInterpolated($"Processed {items} items in {elapsed:duration}"); // Processed 42 items in 3h 44m 9s
Console.WriteLineInterpolated(OutputPipe.Error, $"{Magenta}debug{Default}");
```

`WriteInterpolated` / `WriteLineInterpolated` should be your default output path for styled console text.

### Basic inputs

```csharp
if (!Console.TryReadLine(out int port, $"Port ({Green}5000{Default}): ")) {
    port = 5000;
}

// `TryReadLine<TEnum>` and `TryReadLine` with defaults
if (!Console.TryReadLine(out DayOfWeek day, ignoreCase: true, $"Day? ")) {
    day = DayOfWeek.Monday;
}

var apiKey = Console.ReadLine($"Enter API key ({DarkGray}optional{Default}): ");
```

All input helpers work with `IParsable<T>` and enums, respect the active culture, and honor `OutputPipe` when prompts are colored.

### Advanced inputs

```csharp
Console.RequestAnyInput($"Press {Yellow}any key{Default} to continue…");

if (!Console.Confirm($"Deploy to production? ({Green}y{Default}/{Red}n{Default}) ")) {
    return;
}

string[] customTruths = ["sure", "do it"];
bool overwrite = Console.Confirm(customTruths, $"Overwrite existing files? ", emptyIsTrue: false);
```

### Rendering helpers

```csharp
Console.ClearNextLines(3, OutputPipe.Error);
int line = Console.GetCurrentLine();
// … draw something …
Console.GoToLine(line);
Console.SetColors(ConsoleColor.White, ConsoleColor.DarkBlue);
Console.ResetColors();
Console.SkipLines(2); // keep multi-line UIs (progress bars, dashboards) and continue writing below them
```

`ConsoleContext.Out`/`Error` expose the active writers, which is useful for tests or custom writer-based output. Use `Console.WriteWhiteSpaces(int length, OutputPipe pipe)` for convenient padding from call sites, or call `WriteWhiteSpaces(int)` on an existing writer. `Console.SkipLines(n)` advances the cursor without clearing so you can keep overwritten UI (progress bars, spinners, dashboards) visible after completion:

```csharp
Console.WriteWhiteSpaces(8, OutputPipe.Error); // pad status blocks
ConsoleContext.Error.WriteWhiteSpaces(4);      // same via writer
```

### Advanced outputs

```csharp
Console.Overwrite(() => {
    Console.WriteLineInterpolated(OutputPipe.Error, $"{Cyan}Working…{Default}");
    Console.WriteInterpolated(OutputPipe.Error, $"{DarkGray}Elapsed:{Default} {stopwatch.Elapsed:duration}"); // Elapsed: 0h 1m 12s
}, lines: 2);

// Prevent closure allocations with state + generic overload
Console.Overwrite((left, right), tuple => {
    Console.WriteInterpolated($"{tuple.left} ←→ {tuple.right}");
}, lines: 1);

await Console.TypeWrite("Booting systems…", (Color.Green, Color.BlackBackground));
await Console.TypeWriteLine("Ready.", (Color.DefaultForeground, Color.DefaultBackground));
```

Always call `Console.ClearNextLines(totalLines, pipe)` once after the last `Overwrite` to erase the region when you are done.

### Live console regions

`LiveConsoleRegion` is useful when status lines should continue streaming normally while a pinned transient line keeps updating at the bottom.

```csharp
using var live = new LiveConsoleRegion(OutputPipe.Error);

live.Render($"Resolving graph");
live.WriteLine($"Updated package-a");
live.WriteLine($"Updated package-b");

live.RenderProgress(42, (builder, out handler) =>
    handler = builder.Build(OutputPipe.Error, $"Compiling"));

live.Render($"Linking {elapsed:duration}");
live.Clear();
```

Use `WriteLine` for lines that should scroll above the live region, `Render` for transient snapshots, and `RenderProgress` when you want the built-in progress bar inside the region. In interactive CLIs, `OutputPipe.Error` is usually the right pipe so stdout stays machine-friendly.

### Menus and tables

```csharp
var choice = Console.Selection("Pick an environment:", ["Dev", "QA", "Prod"]);
var multi = Console.MultiSelection("Services to restart:", ["API", "Worker", "Scheduler"]);
var (area, action) = Console.TreeMenu("Actions", new Dictionary<string, IList<string>> {
    ["Users"] = ["List", "Create", "Disable"],
    ["Jobs"] = ["Queue", "Retry"]
});

Console.Table(
    headers: ["Name", "Status"],
    columns: [
        ["API", "Worker"],
        ["Running", "Stopped"]
    ]
);
```

Menus validate user input (throwing `ArgumentException` on invalid selections) and use the padding helpers internally to keep columns aligned.

### Progress bars

```csharp
var progress = new ProgressBar {
    ProgressChar = '■',
    ForegroundColor = Color.DarkGray,
    ProgressColor = Color.Green,
};

for (int i = 0; i <= 100; i += 5) {
    progress.Update(i, $"Downloading chunk {i / 5}");
    await Task.Delay(50);
}

// Need separate status + bar lines? sameLine: false
progress.Update(42.5, "Syncing", sameLine: false);

// One-off render without state
ProgressBar.Render(OutputPipe.Error, 75, Color.Magenta, '*', maxLineWidth: 32);
```

`ProgressBar.Update` lets you keep refreshing status text and progress together. You can also set `ProgressBar.MaxLineWidth` on the instance to constrain the rendered line before each update, mirroring the `maxLineWidth` option on `ProgressBar.Render`. The helper `ProgressBar.Render` keeps the cursor on the same line, which is ideal inside `Console.Overwrite`. For dynamic headers, use the overload that accepts a `PrettyConsoleInterpolatedStringHandlerFactory`, mirroring the spinner pattern.

#### Spinner (indeterminate progress)

`Spinner` renders animated frames on the error pipe. For dynamic per-frame headers, use the `PrettyConsoleInterpolatedStringHandlerFactory` overload:

```csharp
var spinner = new Spinner();
await spinner.RunAsync(workTask, (builder, out handler) =>
    handler = builder.Build(OutputPipe.Error, $"Syncing {DateTime.Now:T}"));
```

The factory runs each frame, so the header can reflect changing status.

#### Multiple progress bars with tasks + channels

```csharp
using System.Linq;
using System.Threading.Channels;

string[] downloads = ["Video.mp4", "Archive.zip", "Assets.pak"];
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
                ProgressBar.Render(OutputPipe.Error, state[i], Color.Cyan);
            }
        }, lines: downloads.Length, pipe: OutputPipe.Error);
    }
});

await Task.WhenAll(producers);
updates.Writer.Complete();
await consumer;

Console.ClearNextLines(downloads.Length, OutputPipe.Error); // ensure no artifacts remain
```

Each producer reports progress over the channel, the consumer redraws the stacked bars on every update, and the region is cleared once at the end.

### Pipes & writers

PrettyConsole keeps the original console streams accessible (and settable for tests) via `ConsoleContext`:

```csharp
TextWriter @out = ConsoleContext.Out;
TextWriter @err = ConsoleContext.Error;
TextReader @in = ConsoleContext.In;
```

Use these when you need direct writer access (custom buffering, `WriteWhiteSpaces`, etc.) or swap in mocks for testing. In cases where you must call raw `System.Console` APIs (e.g., `Console.ReadKey(true)`), do so explicitly—PrettyConsole never hides the built-in console.

## Color model

- Prefer `Color` and `Markup` inside `WriteInterpolated` / `WriteLineInterpolated`.
- Use `new AnsiToken("...")` for custom ANSI you want to interpolate like any other style token.
- Use `Color.*` for color-specific APIs that take `AnsiToken`, including `ProgressBar`, `Spinner`, `TypeWrite`, and `LiveConsoleRegion.RenderProgress`.
- `ConsoleColor` remains part of the API for compatibility and for members that still use explicit console colors, such as low-level `Write`/`WriteLine` overloads and `Console.SetColors`.
- `AnsiColors` maps `ConsoleColor` values into the same token-based color model.
- If you use raw ANSI strings directly, PrettyConsole will not manage them for you.

## Contributing

Contributions are welcome! Fork the repo, create a branch, and open a pull request. Bug reports and feature requests are tracked through GitHub issues.

## Contact

For bug reports, feature requests, or sponsorship inquiries reach out at <dusrdev@gmail.com>.

> This project is proudly made in Israel 🇮🇱 for the benefit of mankind.
