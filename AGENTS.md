# AGENTS.md

Repository: PrettyConsole

Summary

- PrettyConsole is a high-performance, allocation-conscious wrapper over System.Console that provides structured colored output, input helpers, rendering controls, menus, and progress bars. It targets net9.0, is trimming/AOT ready, and ships SourceLink metadata for debugging.
- Solution layout:
  - PrettyConsole/ — main library
  - PrettyConsole.Tests/ — interactive/demo runner (manually selects visual feature demos)
  - PrettyConsole.Tests.Unit/ — xUnit v3 unit tests using Microsoft Testing Platform

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
  - `PrettyConsole.Console` is a static, partial wrapper over `System.Console`. It exposes the live `In`, `Out`, and `Error` streams, and adds helpers like `NewLine`, `Clear`, `ClearNextLines`, `GetCurrentLine`, `GoToLine`, `SetColors`, and `ResetColors` for structured rendering.
- Output routing
  - `OutputPipe` is a two-value enum (`Out`, `Error`). Most write APIs accept an optional pipe; internally `Console.GetWriter` resolves the correct `TextWriter` so sequences remain redirect-friendly.
- Interpolated string handler
  - `PrettyConsoleInterpolatedStringHandler` enables zero-allocation `$"..."` calls for `Write`, `WriteLine`, `ReadLine`, `TryReadLine`, `Confirm`, and `RequestAnyInput`. Colors automatically reset after each invocation, and handlers respect the selected pipe and optional `IFormatProvider`.
- Coloring model
  - `ColoredOutput` and the `Color` record provide terse composition via `"Text" * Color.Red / Color.Blue` and implicit conversions. Default foreground/background values are stored on `Color` so spans render without string allocations.
- Write APIs
  - `Write`/`WriteLine` cover interpolated strings, `ColoredOutput` spans, raw `ReadOnlySpan<char>`, and generic `ISpanFormattable` values (including `ref struct`s) with optional foreground/background colors and format providers. Internally they rely on `BufferPool` to avoid allocation spikes.
- Inputs
  - `ReadLine`/`TryReadLine` support `IParsable<T>` types, optional defaults, enum parsing with `ignoreCase`, and interpolated prompts. `Confirm` exposes `DefaultConfirmValues`, overloads for custom truthy tokens, and interpolated prompts; `RequestAnyInput` blocks on `ReadKey` with colored prompts if desired.
- Rendering controls
  - `ClearNextLines`, `GoToLine`, and `GetCurrentLine` coordinate bounded screen regions; `Clear` wipes the buffer when safe. These helpers underpin progress rendering and overwrite scenarios.
- Advanced outputs
  - `OverwriteCurrentLine`, `Overwrite`, and `Overwrite<TState>` run user actions while clearing a configurable number of lines, enabling reactive text dashboards without leaving artifacts. `TypeWrite`/`TypeWriteLine` animate character-by-character output with adjustable delays.
- Menus and tables
  - `Selection` returns a single choice or empty string on invalid input; `MultiSelection` parses space-separated indices into string arrays; `TreeMenu` renders two-level hierarchies and validates input (throwing `ArgumentException` when selections are invalid); `Table` renders headers + columns with width calculations.
- Progress bars
  - `IndeterminateProgressBar` binds to running `Task` instances, optionally starts tasks, supports cancellable `RunAsync` overloads, exposes `AnimationSequence`, `Patterns`, `ForegroundColor`, `DisplayElapsedTime`, and `UpdateRate`. Frames render on the error pipe and auto-clear.
  - `ProgressBar` maintains a single-line bar on the error pipe. `Update` accepts `int`/`double` percentages plus optional status spans, and exposes `ProgressChar`, `ForegroundColor`, and `ProgressColor` for customization.
- Packaging and targets
  - `PrettyConsole.csproj` targets net9.0, enables trimming/AOT (`IsTrimmable`, `IsAotCompatible`), embeds SourceLink, and grants `InternalsVisibleTo` the unit-test project.

Testing structure and workflows

- PrettyConsole.Tests (interactive)
  - `Program.cs` allows to test things that need to be verified visually and can't be tested easily or at all using unit tests. It contains tests for various things like menues, tables, progress bar, etc... and at occations new overloads and other things. It's content doesn't need to be tracked, it is more like a playground.
- PrettyConsole.Tests.Unit (xUnit v3)
  - Uses Microsoft.NET.Test.Sdk with the Microsoft Testing Platform runner; xunit.runner.json is included. Execute with dotnet run as shown above; pass filters after to narrow to a class or method.

Notes and gotchas

- The library aims to minimize allocations; prefer span-based overloads (ReadOnlySpan<char>, ReadOnlySpan<ColoredOutput>) for best performance when contributing.
- When authoring new features, pick the appropriate OutputPipe to keep CLI piping behavior intact.
- On macOS terminals, ANSI is supported; Windows legacy terminals are handled via ANSI-compatible rendering in the library.
