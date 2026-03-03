---
name: prettyconsole-expert
description: Expert workflow for using PrettyConsole correctly and efficiently in C# console apps. Use when tasks involve console styling, colored output, regular prints, prompts, typed input parsing, confirmation prompts, menu/table rendering, overwrite-based rendering, progress bars, spinners, OutputPipe routing, or migration from Spectre.Console/manual ANSI/older PrettyConsole APIs.
---

# PrettyConsole Expert

## Core Workflow

1. Verify the installed PrettyConsole version before coding.
- Read `Directory.Packages.props`, `*.csproj`, and/or run `dotnet list package`.
- Keep implementation compatible with the installed version; do not "fix" compilation by downgrading unless the user explicitly requests downgrading.

2. Bring extension APIs into scope:

```csharp
using PrettyConsole;
using static System.Console; // optional
```

3. Choose APIs by intent.
- Styled output: `Console.WriteInterpolated`, `Console.WriteLineInterpolated`.
- Inputs/prompts: `Console.TryReadLine`, `Console.ReadLine`, `Console.Confirm`, `Console.RequestAnyInput`.
- Dynamic rendering: `Console.Overwrite`, `Console.ClearNextLines`, `Console.SkipLines`.
- Progress UI: `ProgressBar.Update`, `ProgressBar.Render`, `Spinner.RunAsync`.
- Menus/tables: `Console.Selection`, `Console.MultiSelection`, `Console.TreeMenu`, `Console.Table`.
- Low-level override only: use `Console.Write(...)` / `Console.WriteLine(...)` span+`ISpanFormattable` overloads only when you intentionally bypass the handler for a custom formatting pipeline.

## Performance Rules

- Prefer interpolated-handler APIs over manually concatenated strings.
- Avoid span/formattable `Write`/`WriteLine` overloads in normal app code; reserve them for rare advanced/manual formatting scenarios.
- Keep ANSI/decorations inside interpolation holes (for example, `$"{Markup.Bold}..."`) instead of literal escape codes inside string literals.
- Route transient UI (spinner/progress/overwrite loops) to `OutputPipe.Error` to keep stdout pipe-friendly.
- After the last overwrite/progress frame, clear the UI region once with `Console.ClearNextLines(totalLines, pipe)` or intentionally keep it with `Console.SkipLines`.

## API Guardrails (Current Surface)

- Use `Spinner`, not `IndeterminateProgressBar`.
- Use `Pattern`, not `AnimationSequence`.
- Use `ProgressBar.Render(...)`, not `ProgressBar.WriteProgressBar(...)`.
- Use `ConsoleContext`, not `PrettyConsoleExtensions`.
- Use `ConsoleColor` helpers/tuples (for example `ConsoleColor.Red / ConsoleColor.White`), not removed `ColoredOutput`/`Color` types.
- Use `Confirm(ReadOnlySpan<string> trueValues, ref PrettyConsoleInterpolatedStringHandler handler, bool emptyIsTrue = true)` (boolean parameter is last).
- Use handler factory overloads for dynamic spinner/progress headers:
  `(builder, out handler) => handler = builder.Build(OutputPipe.Error, $"...")`.

## Fast Templates

```csharp
// Colored/status output
Console.WriteLineInterpolated($"{ConsoleColor.Green / ConsoleColor.DefaultBackground}OK{ConsoleColor.Default}");

// Typed input
if (!Console.TryReadLine(out int port, $"Port ({ConsoleColor.Cyan}5000{ConsoleColor.Default}): "))
    port = 5000;

// Confirm with custom truthy tokens
bool deploy = Console.Confirm(["y", "yes", "deploy"], $"Deploy now? ", emptyIsTrue: false);

// Spinner
var spinner = new Spinner();
await spinner.RunAsync(workTask, (builder, out handler) =>
    handler = builder.Build(OutputPipe.Error, $"Syncing..."));

// Progress rendering
var bar = new ProgressBar { ProgressColor = ConsoleColor.Green };
bar.Update(65, "Downloading", sameLine: true);
ProgressBar.Render(OutputPipe.Error, 65, ConsoleColor.Green);
```

## Reference File

Read [references/v5-api-map.md](references/v5-api-map.md) when you need exact usage snippets, migration mapping from old APIs, or a compile-fix checklist.

If public API usage changes in the edited project, ask whether to update `README.md` and changelog/release-notes files.
