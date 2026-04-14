# PrettyConsole v6 API Map

Use this file when implementing or reviewing PrettyConsole usage so code compiles against the current surface and keeps the library's allocation-conscious patterns.

## 1. Namespace and Setup

```csharp
using PrettyConsole;
```

PrettyConsole methods are extension members on `System.Console`.

## 2. Correct v6 APIs

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
  - `Console.NewLine(...)`
- Live retained UI:
  - `LiveConsoleRegion.WriteLine(...)`
  - `LiveConsoleRegion.Render(...)`
  - `LiveConsoleRegion.RenderProgress(...)`
  - `LiveConsoleRegion.Clear()`
- Progress:
  - `ProgressBar.Update(...)`
  - `ProgressBar.Render(...)`
  - `Spinner.RunAsync(...)`
- Menus/tables:
  - `Console.Selection(...)`
  - `Console.MultiSelection(...)`
  - `Console.TreeMenu(...)`
  - `Console.Table(...)`

### Output routing

- Keep prompts, menus, tables, final user-facing output, and machine-readable non-error output on `OutputPipe.Out` unless you intentionally need a different split.
- Use `OutputPipe.Error` for transient live UI and for actual errors, diagnostics, and warnings so stdout remains pipe-friendly and error output stays distinct.
- `LiveConsoleRegion` should usually live on `OutputPipe.Error` in interactive CLIs. Keep durable lines that must coordinate with it flowing through the region instance instead of writing directly to the same pipe elsewhere.
- Disposing a `LiveConsoleRegion` clears the retained snapshot and then permanently closes the region. Call `Clear()` only when you want to remove the current snapshot but keep the region instance reusable for later renders.
- Avoid mixing a single interactive exchange across `Out` and `Error` unless the split is intentional.

### Interpolated-handler styling model

- Prefer `Color`, `Markup`, and guarded `AnsiToken` inside interpolation holes:

  ```csharp
  Console.WriteLineInterpolated($"{Color.Green}ready{Color.Default}");
  Console.WriteLineInterpolated($"{Markup.Bold}build{Markup.ResetBold} complete");
  ```

- `ConsoleColor` interpolation still works for compatibility, but `Color.*` is the primary v6 surface for styled output.
- For APIs that explicitly take `AnsiToken`, use `Color.*` or `new AnsiToken("...")`.
- Do not embed raw escape sequences directly in string literals. Keep ANSI inside interpolation holes so PrettyConsole can isolate the token safely.
- If you must branch on ANSI capability outside the guarded token model, use `ConsoleContext.IsAnsiSupported`.

### Interpolated-handler special formats

- `TimeSpan` with `:duration`:
  - `Console.WriteInterpolated($"Elapsed {elapsed:duration}")`
  - Example output: `Elapsed 5h 32m 12s`
- `double` with `:bytes`:
  - `Console.WriteInterpolated($"Downloaded {size:bytes}")`
  - Example output: `Downloaded 12.3 MB`
- `ReadOnlySpan<char>` and `ISpanFormattable` values work directly in interpolation holes:

  ```csharp
  ReadOnlySpan<char> prefix = "artifact:".AsSpan()[..8];
  int count = 42;
  Console.WriteInterpolated($"{prefix} {count:D4}");
  ```

  Prefer this over dropping to low-level span `Write(...)` APIs when you still want normal interpolated output composition.

### Low-level escape hatch (rare)

Use these only when intentionally bypassing the interpolated handler for a custom formatting pipeline:

- `Console.Write<T>(...) where T : ISpanFormattable`
- `Console.Write(ReadOnlySpan<char> ...)`
- `Console.WriteLine<T>(...)`

### New lines and blank lines

- Use `Console.NewLine(pipe)` when the intent is only to end the current line or emit a blank line.
- Do not use `Console.WriteLineInterpolated($"")` or reset-only payloads such as `$"{Color.Default}"` just to force a newline.
- Use `WriteLineInterpolated(...)` when you are actually writing content and also want the trailing newline.

## 3. Old -> New Migration Table

- `IndeterminateProgressBar` -> `Spinner`
- `AnimationSequence` -> `Pattern`
- `ProgressBar.WriteProgressBar` -> `ProgressBar.Render`
- `PrettyConsoleExtensions` -> `ConsoleContext`
- Legacy `ColoredOutput`/`Color` types -> `Color`, `Markup`, `AnsiToken`, and `ConsoleColor` compatibility where the API explicitly asks for it
- `Markup` and `AnsiColors` raw string expectations -> guarded `AnsiToken` values

## 4. Compile-Safe Patterns

### Styled output

```csharp
Console.WriteInterpolated($"[{Color.Cyan}info{Color.Default}] {message}");
Console.WriteLineInterpolated(OutputPipe.Error, $"{Color.Red}error{Color.Default}: {message}");
Console.NewLine();
```

### Typed input

```csharp
if (!Console.TryReadLine(out int port, $"Port ({Color.Green}5000{Color.Default}): "))
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
            selection = Console.Selection(options, $"{Color.Cyan}{title}{Color.DefaultForeground}:");
            if (selection.Length == 0)
                Console.WriteLineInterpolated(OutputPipe.Error, $"{Color.Red}Invalid choice.{Color.Default}");
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
    handler = builder.Build(OutputPipe.Error, $"Current step: {Color.Green}{steps[current]}");
});
```

Use this when the spinner header should reflect concurrently changing state without locking around the render path.

### Stateful overwrite rendering

```csharp
Console.Overwrite(percent, static current => {
    ProgressBar.Render(OutputPipe.Error, current, Color.Cyan, maxLineWidth: 40);
    Console.NewLine(OutputPipe.Error);
    Console.WriteInterpolated(OutputPipe.Error, $"Downloading assets... {Color.Cyan}{current}{Color.Default}");
}, lines: 2, pipe: OutputPipe.Error);
```

Prefer this shape for live `status + progress` regions. It keeps the state explicit, avoids closure allocations, and makes the rendered height obvious.

### Retained live region

```csharp
using var live = new LiveConsoleRegion(OutputPipe.Error);

live.Render($"Resolving graph");
live.WriteLine($"Fetched package-a");
live.RenderProgress(65, (builder, out handler) =>
    handler = builder.Build(OutputPipe.Error, $"Compiling {Color.Cyan}core{Color.Default}"));
live.Render($"Linking {Color.Cyan}core{Color.Default}");
```

Prefer `LiveConsoleRegion` when durable lines must keep flowing above a pinned transient line on the same pipe over time.

### Overwrite loop cleanup

```csharp
Console.Overwrite(() => {
    Console.WriteLineInterpolated(OutputPipe.Error, $"Running...");
    ProgressBar.Render(OutputPipe.Error, percent, Color.Cyan);
}, lines: 2, pipe: OutputPipe.Error);

Console.ClearNextLines(2, OutputPipe.Error);
```

`Spinner.RunAsync(...)`, `ProgressBar.Update(...)`, and overwrite-based regions do not clean up the final area for you. Choose one of these explicitly after the last frame:

- `Console.ClearNextLines(totalLines, pipe)` to remove the live UI
- `Console.SkipLines(totalLines)` to keep the final rendered rows and continue below them

### High-frequency concurrent status updates

Use one reader task to own all `Console.Overwrite(...)` calls and let concurrent workers publish snapshots through a bounded channel only when multiple producers need to update the same live region at high frequency:

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

For single-producer or modest-rate updates, prefer a simpler render loop without the channel.

## 5. Performance Checklist

- Prefer interpolated handlers over string concatenation.
- Treat span/formattable `Write` and `WriteLine` overloads as advanced escape hatches, not default app-level APIs.
- Use `Console.NewLine(pipe)` for bare line breaks instead of empty or reset-only `WriteLineInterpolated(...)` calls.
- Keep ANSI and decorations in interpolation holes, not raw literal spans.
- Use `Color.*` for token-based color APIs such as `ProgressBar`, `Spinner`, `TypeWrite`, and `LiveConsoleRegion.RenderProgress`.
- Use `OutputPipe.Error` for transient rendering and genuine errors or diagnostics, but keep ordinary non-error interaction flow on `OutputPipe.Out`.
- Clean up live UI explicitly after the last frame with `ClearNextLines(...)` or keep it intentionally with `SkipLines(...)`.
- Avoid introducing wrapper abstractions when direct PrettyConsole APIs already solve the task.
