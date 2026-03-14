---
name: pretty-console-expert
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
- Dynamic rendering and line control: `Console.Overwrite`, `Console.ClearNextLines`, `Console.SkipLines`, `Console.NewLine`.
- Progress UI: `ProgressBar.Update`, `ProgressBar.Render`, `Spinner.RunAsync`.
- Menus/tables: `Console.Selection`, `Console.MultiSelection`, `Console.TreeMenu`, `Console.Table`.
- Low-level override only: use `Console.Write(...)` / `Console.WriteLine(...)` span+`ISpanFormattable` overloads only when you intentionally bypass the handler for a custom formatting pipeline.

4. Route output deliberately.
- Keep normal prompts, menus, tables, durable user-facing output, and machine-readable non-error output on `OutputPipe.Out` unless there is a specific reason not to.
- Use `OutputPipe.Error` for transient live UI and for actual errors/diagnostics/warnings so stdout stays pipe-friendly and error output remains distinguishable.
- Do not bounce a single interaction between `Out` and `Error` unless you intentionally want that split; mixed-pipe prompts and retry messages are usually awkward in consumer CLIs.

## Handler Special Formats

- Use `:duration` with `TimeSpan` to render compact elapsed time text from the handler:
  `Console.WriteInterpolated($"Elapsed {elapsed:duration}")` -> `Elapsed 12h 5m 33s`
- Use `:bytes` with `double` to render human-readable file sizes from the handler:
  `Console.WriteInterpolated($"Transferred {bytes:bytes}")` -> `Transferred 12.3 MB`
- Interpolation holes accept `ReadOnlySpan<char>` directly and prefer `ISpanFormattable`, so slices and span-format-capable values stay on the high-performance handler path without dropping to low-level `Write(ReadOnlySpan<char>)` APIs.
- Prefer these formats in status/progress output instead of manual formatting logic.

## Performance Rules

- Prefer interpolated-handler APIs over manually concatenated strings.
- Avoid span/formattable `Write`/`WriteLine` overloads in normal app code; reserve them for rare advanced/manual formatting scenarios.
- If the intent is only to end the current line or emit a blank line, use `Console.NewLine(pipe)` instead of `WriteLineInterpolated($"")` or reset-only interpolations such as `$"{ConsoleColor.Default}"`.
- Keep ANSI/decorations inside interpolation holes (for example, `$"{Markup.Bold}..."`) instead of literal escape codes inside string literals.
- Route transient UI (spinner/progress/overwrite loops) to `OutputPipe.Error` to keep stdout pipe-friendly, and use `OutputPipe.Error` for genuine errors/diagnostics. Keep ordinary non-error interaction flow on `OutputPipe.Out`.
- Spinner/progress/overwrite output is caller-owned after rendering completes. Explicitly remove it with `Console.ClearNextLines(totalLines, pipe)` or intentionally keep the region with `Console.SkipLines(totalLines)`.
- Only use the bounded `Channel<T>` snapshot pattern when multiple producers must update the same live region at high frequency. For single-producer or modest-rate updates, keep the rendering loop simple.

## Practical Patterns

- For wizard-like flows, wrap `Console.Selection(...)` / `Console.MultiSelection(...)` in retrying `Console.Overwrite(...)` loops so each step reuses one screen region instead of scrolling. Keep the whole prompt/retry exchange on `OutputPipe.Out` unless the message is genuinely diagnostic.
- Prefer `Console.Overwrite(state, static ...)` for fixed-height live regions such as `status + progress`; it avoids closure captures and keeps the rendered surface explicit through `lines`.
- For dynamic spinner/progress headers tied to concurrent work, keep the mutable step/progress state outside the renderer and read it with `Volatile.Read` / `Interlocked` inside the handler factory.
- If a live region should disappear after completion, pair the last render with an explicit `ClearNextLines(...)`. If it should remain visible as completed output, advance past it with `SkipLines(...)`.

## Testing CLI Code

- When a CLI already routes its behavior through callable command handlers or functions that use PrettyConsole directly, prefer in-process tests over spawning the whole app with `Process`.
- Inject `ConsoleContext.Out`, `ConsoleContext.Error`, and `ConsoleContext.In` with `StringWriter`/`StringReader`, invoke the same handler the CLI entrypoint uses internally, and assert on writers plus returned exit codes/results.
- Keep separate writers for `Out` and `Error` so pipe routing remains testable.
- Save and restore the original `ConsoleContext` streams in `try/finally` or a scoped helper.
- Reserve `Process` for true end-to-end coverage such as entrypoint wiring, shell integration, environment/current-directory behavior, published-binary checks, or argument parsing that is only exercised at the process boundary.

## API Guardrails (Current Surface)

- Use `Spinner`, not `IndeterminateProgressBar`.
- Use `Pattern`, not `AnimationSequence`.
- Use `ProgressBar.Render(...)`, not `ProgressBar.WriteProgressBar(...)`.
- Use `ConsoleContext`, not `PrettyConsoleExtensions`.
- Use `ConsoleColor` helpers/tuples (for example `ConsoleColor.Red / ConsoleColor.White`), not removed `ColoredOutput`/`Color` types.
- Use `Console.NewLine(pipe)` when you only need a newline or blank line; do not use `WriteLineInterpolated` with empty/reset-only payloads just to move the cursor.
- Use `Confirm(ReadOnlySpan<string> trueValues, ref PrettyConsoleInterpolatedStringHandler handler, bool emptyIsTrue = true)` (boolean parameter is last).
- Use handler factory overloads for dynamic spinner/progress headers:
  `(builder, out handler) => handler = builder.Build(OutputPipe.Error, $"...")`.

## Fast Templates

```csharp
// Colored/status output
Console.WriteLineInterpolated($"{ConsoleColor.Green / ConsoleColor.DefaultBackground}OK{ConsoleColor.Default}");
Console.NewLine();

// Typed input
if (!Console.TryReadLine(out int port, $"Port ({ConsoleColor.Cyan}5000{ConsoleColor.Default}): "))
    port = 5000;

// Confirm with custom truthy tokens
bool deploy = Console.Confirm(["y", "yes", "deploy"], $"Deploy now? ", emptyIsTrue: false);

// Spinner
var spinner = new Spinner();
await spinner.RunAsync(workTask, (builder, out handler) =>
    handler = builder.Build(OutputPipe.Error, $"Syncing..."));
Console.ClearNextLines(1, OutputPipe.Error); // or Console.SkipLines(1) to keep the final row

// Progress rendering
var bar = new ProgressBar { ProgressColor = ConsoleColor.Green };
bar.Update(65, "Downloading", sameLine: true);
ProgressBar.Render(OutputPipe.Error, 65, ConsoleColor.Green);
```

## Reference File

Read [references/v5-api-map.md](references/v5-api-map.md) when you need exact usage snippets, migration mapping from old APIs, or a compile-fix checklist.
Read [references/testing-with-consolecontext.md](references/testing-with-consolecontext.md) when the task involves testing a PrettyConsole-based CLI or command handler.

If public API usage changes in the edited project, ask whether to update `README.md` and changelog/release-notes files.
