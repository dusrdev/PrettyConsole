# PrettyConsole v5 API Map

Use this file when implementing or reviewing PrettyConsole usage so code compiles against modern APIs and keeps allocation-conscious patterns.

## 1. Version First

Read installed version before coding:

```bash
dotnet list package
rg -n "PrettyConsole" --glob "*.csproj" .
# optionally also check central package management if present:
# rg -n "PrettyConsole" Directory.Packages.props
```

If version and request conflict, keep the installed version and adapt code accordingly.

## 2. Namespace and Setup

```csharp
using PrettyConsole;
using static System.Console; // optional
```

PrettyConsole methods are extension members on `System.Console`.

## 3. Correct Modern APIs

- Styled writes:
  - `Console.WriteInterpolated(...)`
  - `Console.WriteLineInterpolated(...)`
- Inputs:
  - `Console.TryReadLine(...)`
  - `Console.ReadLine(...)`
  - `Console.Confirm(...)`
  - `Console.RequestAnyInput(...)`
- Rendering:
  - `Console.Overwrite(...)`
  - `Console.ClearNextLines(...)`
  - `Console.SkipLines(...)`
- Progress:
  - `ProgressBar.Update(...)`
  - `ProgressBar.Render(...)`
  - `Spinner.RunAsync(...)`
- Menus/tables:
  - `Console.Selection(...)`
  - `Console.MultiSelection(...)`
  - `Console.TreeMenu(...)`
  - `Console.Table(...)`

### Interpolated-handler special formats

- `TimeSpan` with `:duration`:
  - `Console.WriteInterpolated($"Elapsed {elapsed:duration}")`
  - Example output: `Elapsed 5h 32m 12s`
- `double` with `:bytes`:
  - `Console.WriteInterpolated($"Downloaded {size:bytes}")`
  - Example output: `Downloaded 12.3 MB`

### Low-level escape hatch (rare)

Use these only when intentionally bypassing the interpolated handler for a custom formatting pipeline:

- `Console.Write<T>(...) where T : ISpanFormattable`
- `Console.Write(ReadOnlySpan<char> ...)`
- `Console.WriteLine<T>(...)`

## 4. Old -> New Migration Table

- `IndeterminateProgressBar` -> `Spinner`
- `AnimationSequence` -> `Pattern`
- `ProgressBar.WriteProgressBar` -> `ProgressBar.Render`
- `PrettyConsoleExtensions` -> `ConsoleContext`
- Legacy `ColoredOutput`/`Color` types -> `ConsoleColor` helpers and tuples

## 5. Compile-Safe Patterns

### Styled output

```csharp
Console.WriteInterpolated($"[{ConsoleColor.Cyan}info{ConsoleColor.Default}] {message}");
Console.WriteLineInterpolated(OutputPipe.Error, $"{ConsoleColor.Yellow}warn{ConsoleColor.Default}");
```

### Typed input

```csharp
if (!Console.TryReadLine(out int port, $"Port ({ConsoleColor.Green}5000{ConsoleColor.Default}): "))
    port = 5000;
```

### Confirmation

```csharp
bool yes = Console.Confirm(["y", "yes"], $"Continue? ", emptyIsTrue: false);
```

### Wizard-style menus

```csharp
static string PromptSelection(string title, string[] options) {
    string selection = string.Empty;

    while (selection.Length == 0) {
        Console.Overwrite(() => {
            selection = Console.Selection(options, $"{ConsoleColor.Cyan}{title}{ConsoleColor.DefaultForeground}:");
            if (selection.Length == 0)
                Console.WriteLineInterpolated(OutputPipe.Error, $"{ConsoleColor.Red}Invalid choice.");
        }, lines: options.Length + 3, pipe: OutputPipe.Out);
    }

    return selection;
}
```

Use this when you want multi-step prompts to behave like page transitions instead of adding scrollback on each retry.

### Spinner with shared progress state

```csharp
string[] steps = ["Restore", "Compile", "Pack"];
var step = 0;

var workTask = Task.Run(async () => {
    for (; step < steps.Length; Interlocked.Increment(ref step))
        await Task.Delay(500);
});

var spinner = new Spinner();
await spinner.RunAsync(workTask, (builder, out handler) => {
    var current = Math.Min(Volatile.Read(ref step), steps.Length - 1);
    handler = builder.Build(OutputPipe.Error, $"Current step: {ConsoleColor.Green}{steps[current]}");
});
```

Use this when the spinner header should reflect concurrently changing state without locking around the render path.

### Stateful overwrite rendering

```csharp
Console.Overwrite(percent, static current => {
    ProgressBar.Render(OutputPipe.Error, current, ConsoleColor.Cyan, maxLineWidth: 40);
    Console.NewLine(OutputPipe.Error);
    Console.WriteInterpolated(OutputPipe.Error, $"Downloading assets... {ConsoleColor.Cyan}{current}");
}, lines: 2, pipe: OutputPipe.Error);
```

Prefer this shape for live `status + progress` regions. It keeps the state explicit, avoids closure allocations, and makes the rendered height obvious.

### Overwrite loop cleanup

```csharp
Console.Overwrite(() => {
    Console.WriteLineInterpolated(OutputPipe.Error, $"Running...");
    ProgressBar.Render(OutputPipe.Error, percent, ConsoleColor.Cyan);
}, lines: 2, pipe: OutputPipe.Error);

Console.ClearNextLines(2, OutputPipe.Error);
```

### High-frequency concurrent status updates

Use one reader task to own all `Console.Overwrite(...)` calls and let concurrent workers publish snapshots through a bounded channel:

```csharp
using System.Threading.Channels;

var channel = Channel.CreateBounded<Stats>(new BoundedChannelOptions(1) {
    SingleWriter = false,
    SingleReader = true,
    FullMode = BoundedChannelFullMode.DropWrite
});

_ = Task.Run(async () => {
    await foreach (var stats in channel.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false)) {
        Console.Overwrite(stats, static current => {
            PrintMetrics(current);
        }, lines: 2, pipe: OutputPipe.Error);
    }

    Console.ClearNextLines(2, OutputPipe.Error);
}, cancellationToken);

// Workers stay non-blocking and may skip intermediate frames when the UI is busy.
channel.Writer.TryWrite(latestStats);
```

Why this works:

- only one reader ever renders, so `Overwrite` calls do not race each other
- capacity `1` + `DropWrite` avoids backpressure on workers during high-frequency updates
- this pattern is best when dropped intermediate states are acceptable and only recent snapshots matter

## 6. Performance Checklist

- Prefer interpolated handlers over string concatenation.
- Treat span/formattable `Write`/`WriteLine` overloads as advanced escape hatches, not default app-level APIs.
- Keep ANSI/decorations in interpolation holes, not raw literal spans.
- Use `OutputPipe.Error` for transient rendering.
- Avoid introducing wrapper abstractions when direct PrettyConsole APIs already solve the task.
