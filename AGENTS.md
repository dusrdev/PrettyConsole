# AGENTS.md

Repository: PrettyConsole

Summary

- PrettyConsole is a high-performance, allocation-conscious extension layer over System.Console (implemented via C# extension members) that provides structured colored output, input helpers, rendering controls, menus, and progress bars. It targets net10.0, is trimming/AOT ready, and ships SourceLink metadata for debugging.
- Solution layout:
  - PrettyConsole/ — main library
  - PrettyConsole.Tests/ — interactive/demo runner (manually selects visual feature demos)
  - PrettyConsole.Tests.Unit/ — xUnit v3 unit tests using Microsoft Testing Platform
  - Examples/ — standalone `.cs` sample apps plus `assets/` previews; documented in `Examples/README.md` and excluded from automated builds/tests
- v5.4.0 (current) renames `IndeterminateProgressBar` to `Spinner` (and `AnimationSequence` to `Pattern`), triggers the line reset at the start of each spinner frame, gives all `RunAsync` overloads default cancellation tokens, and renames `ProgressBar.WriteProgressBar` to `Render` while adding handler-factory overloads and switching header parameters to `string`. It still passes handlers by `ref`, adds `AppendInline`, and introduces the ctor that takes only `OutputPipe` + optional `IFormatProvider`; `SkipLines` advances the cursor while keeping overwritten UIs; `Confirm(trueValues, ref handler, bool emptyIsTrue = true)` has the boolean last; spinner header factories use `PrettyConsoleInterpolatedStringHandlerFactory` with the singleton builder; `AnsiColors` is public. v5.2.0 rewrote the handler to buffer before writing and added `WhiteSpace`; v5.1.0 renamed `PrettyConsoleExtensions` to `ConsoleContext`, added `Console.WriteWhiteSpaces(length, pipe)`, and made `Out`/`Error`/`In` settable; v5.0.0 removed the legacy `ColoredOutput`/`Color` types in favor of `ConsoleColor` helpers and tuples.
- The agent skill `.agents/skills/prettyconsole-expert` is versioned by `.agents/skills/prettyconsole-expert/VERSION` and should match `PrettyConsole` package version; when package version changes, update skill versioning/docs in lockstep.

Commands you’ll use often

- Build
  - Build library:
    - dotnet build PrettyConsole/PrettyConsole.csproj
  - Build unit tests:
    - dotnet build PrettyConsole.Tests.Unit/PrettyConsole.Tests.Unit.csproj
  - The solution using .slnx format; run it as usual but prefer to build individual projects as needed.
- Format (uses the repo’s .editorconfig conventions)
  - Check and fix code style/formatting:
    - dotnet format
- Run
  - Never run interactive/demo tests (PrettyConsole.Tests)
  - Run unit tests (xUnit v3 via Microsoft Testing Platform):
    - dotnet run --project PrettyConsole.Tests.Unit
  - Run a single unit test:
    - dotnet run --project PrettyConsole.Tests.Unit --filter-method "*UniquePartOfMethodName*"
    - Examples:
      - dotnet run --project PrettyConsole.Tests.Unit --filter-method "*WritesColoredLine*"
- Pack - DO NOT DO THIS YOURSELF!

Repo-specific agent rules and conventions

- Prefer dotnet CLI for making, verifying, and running changes.
- When changing a specific project, build/run just that project to validate, not the entire solution.
- For tests using Microsoft Testing Platform and/or xUnit v3, use dotnet run, never dotnet test.
- Adhere to .editorconfig in the repo for style and analyzers.
- If code needs to be “removed” as part of a change, do not delete files; comment out their contents so they won’t compile.
- Avoid reflection/dynamic assembly loading in published library code unless explicitly requested.

High-level architecture and key concepts

- Console facade
  - Extension members declared via `extension(Console)` attach directly to `System.Console`, so APIs such as `Console.WriteInterpolated`, `Console.TryReadLine`, `Console.Overwrite`, etc. light up once `using PrettyConsole;` (optionally with `using static System.Console;`) is in scope. `ConsoleContext` exposes the live `In`, `Out`, and `Error` streams (all now settable for testing) plus helpers like `GetWidthOrDefault`.
- Output routing
  - `OutputPipe` is a two-value enum (`Out`, `Error`). Most write APIs accept an optional pipe; internally `ConsoleContext.GetWriter` resolves the correct `TextWriter` so sequences remain redirect-friendly.
- Interpolated string handler
  - `PrettyConsoleInterpolatedStringHandler` buffers interpolated content before emitting it, stays allocation-free, now exposes additional public helpers (including `AppendInline` for composing handlers) and is constructed/consumed by `ref`. `$"..."` calls light up `WriteInterpolated`, `WriteLineInterpolated`, `ReadLine`, `TryReadLine`, `Confirm`, and `RequestAnyInput`. Colors auto-reset, handlers respect the selected pipe/`IFormatProvider`, and `object` arguments that implement `ISpanFormattable` are emitted via the span path before falling back to `IFormattable`/string. `Console.WriteInterpolated`/`WriteLineInterpolated` return the rendered character count (handler-emitted escape sequences excluded). Passing the `WhiteSpace` struct writes padding directly from the handler without allocations.
  - Mid-span ANSI sequences are intentionally unsupported: every ANSI sequence (from `ConsoleColor` conversions or `Markup`) is only safe when emitted via an interpolated hole, which lets the handler isolate the escape and keep width calculations consistent. Do not try to "account" for mid-span sequences or adjust character counts manually when discussing this repo.
- Coloring model
  - `ConsoleColor` exposes `DefaultForeground`, `DefaultBackground`, and `Default` tuple properties plus `/` operator overloads so you can inline foreground/background tuples (`$"{ConsoleColor.Red / ConsoleColor.White}Error"`). These tuples play nicely with the interpolated string handler and keep color resets allocation-free. `AnsiColors` is now public if you need raw ANSI sequences from `ConsoleColor`.
- Markup decorations
  - The `Markup` static class exposes ANSI sequences for underline, bold, italic, and strikethrough. Fields expand to escape codes only when output/error aren’t redirected; otherwise they collapse to empty strings so callers can safely interpolate them without extra checks.
- Write APIs
  - `WriteInterpolated`/`WriteLineInterpolated` are the default output APIs and host the interpolated-string handler; this path already covers high-performance formatting and coloring. Keep `Write`/`WriteLine` overloads (`ISpanFormattable`/`ReadOnlySpan<char>`) for rare low-level scenarios where callers intentionally bypass the handler with custom formatting pipelines. Those overloads still rent buffers from `ArrayPool<char>.Shared` and reset colors.
- TextWriter helpers
  - `ConsoleContext` surfaces the live `Out`/`Error` writers (now with public setters for test doubles) and keeps helpers like `GetWidthOrDefault`. Use `Console.WriteWhiteSpaces(int length)` for the default output path and specify `OutputPipe.Error` only when needed; `TextWriter.WriteWhiteSpaces(int)` remains available on the writers if you already have them on hand.
- Inputs
  - `ReadLine`/`TryReadLine` support `IParsable<T>` types, optional defaults, enum parsing with `ignoreCase`, and interpolated prompts. `Confirm` exposes `DefaultConfirmValues`, overloads for custom truthy tokens, and interpolated prompts; `RequestAnyInput` blocks on `ReadKey` with colored prompts if desired.
- Rendering controls
  - `ClearNextLines`, `GoToLine`, `GetCurrentLine`, and `SkipLines` coordinate bounded screen regions; `Clear` wipes the buffer when safe. `SkipLines` lets you advance the cursor to preserve overwritten UIs (progress bars, spinners) after completion. These helpers underpin progress rendering and overwrite scenarios.
- Advanced outputs
  - `OverwriteCurrentLine`, `Overwrite`, and `Overwrite<TState>` run user actions while clearing a configurable number of lines. Set the `lines` argument to however many rows you emit during the action (e.g., the multi-progress sample uses `lines: 2`) and call `Console.ClearNextLines` once after the last overwrite to remove residual UI. `TypeWrite`/`TypeWriteLine` animate character-by-character output with adjustable delays.
- Menus and tables
  - `Selection` returns a single choice or empty string on invalid input; `MultiSelection` parses space-separated indices into string arrays; `TreeMenu` renders two-level hierarchies and validates input (throwing `ArgumentException` when selections are invalid); `Table` renders headers + columns with width calculations.
- Progress bars
  - `Spinner` (formerly `IndeterminateProgressBar`) binds to running `Task` instances, optionally starts tasks, supports cancellable `RunAsync` overloads with default tokens, exposes `Pattern` (formerly `AnimationSequence`), `Patterns`, `ForegroundColor`, `DisplayElapsedTime`, and `UpdateRate`. Frames render on the error pipe and auto-clear. Header factories use `PrettyConsoleInterpolatedStringHandlerFactory`; call `(builder, out handler) => handler = builder.Build(OutputPipe.Error, $"status")` with the singleton `PrettyConsoleInterpolatedStringHandlerBuilder`.
  - `ProgressBar` maintains a single-line bar on the error pipe. `Update` accepts `int`/`double` percentages plus optional status spans, and exposes `ProgressChar`, `ForegroundColor`, and `ProgressColor` for customization. The static `ProgressBar.Render` helper (renamed from `WriteProgressBar`) renders one-off segments without moving the cursor, so you can stack multiple bars within an `Overwrite` block. It now also has overloads that accept `PrettyConsoleInterpolatedStringHandlerFactory` for low-allocation headers.
- Packaging and targets
  - `PrettyConsole.csproj` targets net10.0, enables trimming/AOT (`IsTrimmable`, `IsAotCompatible`), embeds SourceLink, and grants `InternalsVisibleTo` the unit-test project.

Testing structure and workflows

- PrettyConsole.Tests (interactive)
  - `Program.cs` allows to test things that need to be verified visually and can't be tested easily or at all using unit tests. It contains tests for various things like menues, tables, progress bar, etc... and at occations new overloads and other things. It's content doesn't need to be tracked, it is more like a playground.
- PrettyConsole.Tests.Unit (xUnit v3)
  - Uses Microsoft.NET.Test.Sdk with the Microsoft Testing Platform runner; xunit.runner.json is included. Execute with dotnet run as shown above; pass filters after to narrow to a class or method.
  - Progress bar coverage now includes multi-line rendering (`sameLine: false`), repeat renders at the same percentage, and the static `ProgressBar.Render` helper. Keep these behaviours in sync with docs.

Notes and gotchas

- The library aims to minimize allocations; for normal app-level output prefer interpolated-handler APIs (`WriteInterpolated`/`WriteLineInterpolated`) plus inline `ConsoleColor` tuples. Use span-based `Write`/`WriteLine` overloads only for rare low-level formatting bypass scenarios.
- When authoring new features, pick the appropriate OutputPipe to keep CLI piping behavior intact.
- On macOS terminals, ANSI is supported; Windows legacy terminals are handled via ANSI-compatible rendering in the library.
- `ProgressBar.Update` re-renders on every call (even when the percentage is unchanged) and accepts `sameLine` to place the status above the bar; the static `ProgressBar.Render` renders one-off bars without writing a trailing newline, so rely on `Console.Overwrite`/`lines` to stack multiple bars cleanly.
- After the final `Overwrite`/`Overwrite<TState>` call in a rendering loop, call `Console.ClearNextLines(totalLines, pipe)` once more to clear the region and prevent ghost text.
