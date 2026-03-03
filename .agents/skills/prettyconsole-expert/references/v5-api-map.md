# PrettyConsole v5 API Map

Use this file when implementing or reviewing PrettyConsole usage so code compiles against modern APIs and keeps allocation-conscious patterns.

## 1. Version Sync (Required)

Compare skill and package versions before coding:

```bash
cat .agents/skills/prettyconsole-expert/VERSION
dotnet list package
rg -n "PrettyConsole" --glob "*.csproj" .
# optionally also check central package management if present:
# rg -n "PrettyConsole" Directory.Packages.props
```

Resolve mismatches before implementation:

- If package is missing, install `PrettyConsole` using the skill version.
- If package version > skill version, update the skill folder from GitHub:
  ```bash
  mkdir -p .agents/skills
  curl -Ls https://codeload.github.com/dusrdev/PrettyConsole/tar.gz/refs/heads/stable \
    | tar -xz -C .agents/skills --strip-components=3 PrettyConsole-stable/.agents/skills/prettyconsole-expert
  ```
- If skill version > package version, check upgrade availability:
  ```bash
  dotnet list package --outdated | rg PrettyConsole
  dotnet add package PrettyConsole
  ```
  Upgrade package when available.
- Proceed only when skill and package versions match, or when the user explicitly accepts a mismatch.

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

### Spinner with dynamic header

```csharp
var spinner = new Spinner();
await spinner.RunAsync(workTask, (builder, out handler) =>
    handler = builder.Build(OutputPipe.Error, $"Processing {DateTime.Now:T}"));
```

### Progress bar

```csharp
var progress = new ProgressBar {
    ProgressColor = ConsoleColor.Green,
    ForegroundColor = ConsoleColor.DarkGray
};

progress.Update(40, "Downloading", sameLine: true);
ProgressBar.Render(OutputPipe.Error, 40, ConsoleColor.Green);
```

### Overwrite loop cleanup

```csharp
Console.Overwrite(() => {
    Console.WriteLineInterpolated(OutputPipe.Error, $"Running...");
    ProgressBar.Render(OutputPipe.Error, percent, ConsoleColor.Cyan);
}, lines: 2, pipe: OutputPipe.Error);

Console.ClearNextLines(2, OutputPipe.Error);
```

## 6. Performance Checklist

- Prefer interpolated handlers over string concatenation.
- Treat span/formattable `Write`/`WriteLine` overloads as advanced escape hatches, not default app-level APIs.
- Keep ANSI/decorations in interpolation holes, not raw literal spans.
- Use `OutputPipe.Error` for transient rendering.
- Avoid introducing wrapper abstractions when direct PrettyConsole APIs already solve the task.
