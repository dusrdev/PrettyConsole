# AGENTS.md

Repository: PrettyConsole

Summary

- PrettyConsole is a high-performance, allocation-conscious extension layer over System.Console (implemented via C# extension members) that provides structured colored output, input helpers, rendering controls, menus, and progress bars. It targets net10.0, is trimming/AOT ready, and ships SourceLink metadata for debugging.
- Solution layout:
  - PrettyConsole/ — main library
  - PrettyConsole.Tests/ — interactive/demo runner (manually selects visual feature demos)
  - PrettyConsole.Tests.Unit/ — xUnit v3 unit tests using Microsoft Testing Platform
- v5.1.0 (current) renames `PrettyConsoleExtensions` to `ConsoleContext`, adds `Console.WriteWhiteSpaces(length, pipe)`, and makes `Out`/`Error`/`In` settable for test doubles. v5.0.0 removed the legacy `ColoredOutput`/`Color` types; color composition now flows through `ConsoleColor` helpers and tuples exposed by the library.

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
  - `PrettyConsoleInterpolatedStringHandler` enables zero-allocation `$"..."` calls for `WriteInterpolated`, `WriteLineInterpolated`, `ReadLine`, `TryReadLine`, `Confirm`, and `RequestAnyInput`. Colors automatically reset after each invocation, and handlers respect the selected pipe and optional `IFormatProvider`. Recent changes ensure any `object` argument that implements `ISpanFormattable` is emitted through the span-based path before falling back to `IFormattable`/string allocations. `Console.WriteInterpolated`/`WriteLineInterpolated` now return the rendered character count (escape sequences emitted by the handler are excluded), which can be used for padding/layout math.
- Coloring model
  - `ConsoleColor` now exposes `DefaultForeground`, `DefaultBackground`, and `Default` tuple properties plus `/` operator overloads so you can inline foreground/background tuples (`$"{ConsoleColor.Red / ConsoleColor.White}Error"`). These tuples play nicely with the interpolated string handler and keep color resets allocation-free.
- Markup decorations
  - The `Markup` static class exposes ANSI sequences for underline, bold, italic, and strikethrough. Fields expand to escape codes only when output/error aren’t redirected; otherwise they collapse to empty strings so callers can safely interpolate them without extra checks.
- Write APIs
  - `WriteInterpolated`/`WriteLineInterpolated` host the interpolated-string handler; `Write`/`WriteLine` overloads target `ISpanFormattable` values (including `ref struct`s) and raw `ReadOnlySpan<char>` spans with optional foreground/background overrides. Implementations rent buffers from `ArrayPool<char>.Shared` to avoid allocation spikes and always reset colors.
- TextWriter helpers
  - `ConsoleContext` surfaces the live `Out`/`Error` writers (now with public setters for test doubles) and keeps helpers like `GetWidthOrDefault`. Use `Console.WriteWhiteSpaces(int length, OutputPipe pipe = OutputPipe.Out)` for direct padding from call sites; `TextWriter.WriteWhiteSpaces(int)` remains available on the writers if you already have them on hand.
- Inputs
  - `ReadLine`/`TryReadLine` support `IParsable<T>` types, optional defaults, enum parsing with `ignoreCase`, and interpolated prompts. `Confirm` exposes `DefaultConfirmValues`, overloads for custom truthy tokens, and interpolated prompts; `RequestAnyInput` blocks on `ReadKey` with colored prompts if desired.
- Rendering controls
  - `ClearNextLines`, `GoToLine`, and `GetCurrentLine` coordinate bounded screen regions; `Clear` wipes the buffer when safe. These helpers underpin progress rendering and overwrite scenarios.
- Advanced outputs
  - `OverwriteCurrentLine`, `Overwrite`, and `Overwrite<TState>` run user actions while clearing a configurable number of lines. Set the `lines` argument to however many rows you emit during the action (e.g., the multi-progress sample uses `lines: 2`) and call `Console.ClearNextLines` once after the last overwrite to remove residual UI. `TypeWrite`/`TypeWriteLine` animate character-by-character output with adjustable delays.
- Menus and tables
  - `Selection` returns a single choice or empty string on invalid input; `MultiSelection` parses space-separated indices into string arrays; `TreeMenu` renders two-level hierarchies and validates input (throwing `ArgumentException` when selections are invalid); `Table` renders headers + columns with width calculations.
- Progress bars
  - `IndeterminateProgressBar` binds to running `Task` instances, optionally starts tasks, supports cancellable `RunAsync` overloads, exposes `AnimationSequence`, `Patterns`, `ForegroundColor`, `DisplayElapsedTime`, and `UpdateRate`. Frames render on the error pipe and auto-clear.
  - `ProgressBar` maintains a single-line bar on the error pipe. `Update` accepts `int`/`double` percentages plus optional status spans, and exposes `ProgressChar`, `ForegroundColor`, and `ProgressColor` for customization. The static `ProgressBar.WriteProgressBar` helper renders one-off segments without moving the cursor (so you can stack multiple bars within an `Overwrite` block).
- Packaging and targets
  - `PrettyConsole.csproj` targets net10.0, enables trimming/AOT (`IsTrimmable`, `IsAotCompatible`), embeds SourceLink, and grants `InternalsVisibleTo` the unit-test project.

Testing structure and workflows

- PrettyConsole.Tests (interactive)
  - `Program.cs` allows to test things that need to be verified visually and can't be tested easily or at all using unit tests. It contains tests for various things like menues, tables, progress bar, etc... and at occations new overloads and other things. It's content doesn't need to be tracked, it is more like a playground.
- PrettyConsole.Tests.Unit (xUnit v3)
  - Uses Microsoft.NET.Test.Sdk with the Microsoft Testing Platform runner; xunit.runner.json is included. Execute with dotnet run as shown above; pass filters after to narrow to a class or method.
  - Progress bar coverage now includes multi-line rendering (`sameLine: false`), repeat renders at the same percentage, and the static `ProgressBar.WriteProgressBar` helper. Keep these behaviours in sync with docs.

Notes and gotchas

- The library aims to minimize allocations; prefer span-based overloads (`ReadOnlySpan<char>`, `ISpanFormattable`) plus the inline `ConsoleColor` tuples instead of recreating strings or structs.
- When authoring new features, pick the appropriate OutputPipe to keep CLI piping behavior intact.
- On macOS terminals, ANSI is supported; Windows legacy terminals are handled via ANSI-compatible rendering in the library.
- `ProgressBar.Update` re-renders on every call (even when the percentage is unchanged) and accepts `sameLine` to place the status above the bar; the static `ProgressBar.WriteProgressBar` renders one-off bars without writing a trailing newline, so rely on `Console.Overwrite`/`lines` to stack multiple bars cleanly.
- After the final `Overwrite`/`Overwrite<TState>` call in a rendering loop, call `Console.ClearNextLines(totalLines, pipe)` once more to clear the region and prevent ghost text.
