# AGENTS.md

Repository: PrettyConsole

Summary

- PrettyConsole is a high-performance, allocation-conscious wrapper over System.Console that provides structured colored output, input helpers, rendering controls, menus, and progress bars. It targets net9.0 and is AOT compatible.
- Solution layout:
  - PrettyConsole/ — main library
  - PrettyConsole.Tests/ — interactive/demo runner (manually selects visual feature demos)
  - PrettyConsole.Tests.Unit/ — xUnit v3 unit tests using Microsoft Testing Platform

Commands you’ll use often

- Build
  - Build library only:
    - dotnet build PrettyConsole/PrettyConsole.csproj
  - Build unit tests only:
    - dotnet build PrettyConsole.Tests.Unit/PrettyConsole.Tests.Unit.csproj
  - Build the whole solution:
    - dotnet build PrettyConsole.sln
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
- Pack - DOT NOT DO THIS YOURSELF!

Repo-specific agent rules and conventions

- Prefer dotnet CLI for making, verifying, and running changes.
- When changing a specific project, build/run just that project to validate, not the entire solution.
- For tests using Microsoft Testing Platform and/or xUnit v3, use dotnet run, never dotnet test.
- Adhere to .editorconfig in the repo for style and analyzers.
- If code needs to be “removed” as part of a change, do not delete files; comment out their contents so they won’t compile.
- Avoid reflection/dynamic assembly loading in published library code unless explicitly requested.

High-level architecture and key concepts

- Console wrapper
  - PrettyConsole.Console is a static wrapper around System.Console, exposing In, Out, Error as OutputPipe and providing helpers like NewLine, GoToLine, ClearNextLines, SetColors, ResetColors. The wrapper keeps the API surface practical for common console tasks while preserving piping behavior.
- Coloring model
  - ColoredOutput and Color provide a terse, equation-like syntax for color composition: "Text" * Color.Red / Color.Blue
  - Colors and outputs are designed to be composed with minimal allocations, often using ReadOnlySpan<char> and span-based overloads to avoid string allocations.
- Interpolated string handler
  - `PrettyConsoleInterpolatedStringHandler` powers zero-allocation `$"..."` writes, reads, confirmations, and menu prompts, automatically resetting colors per call and supporting pipe selection.
- Output pipes
  - OutputPipe abstracts the output stream (Out, Error). All Write/WriteLine APIs take an optional pipe so output can be routed appropriately while remaining pipe-friendly for shell usage.
- Inputs
  - ReadLine helpers support typed parsing (IParsable<T>), TryReadLine variants with defaults, enum parsing with optional case-insensitivity, and confirmation prompts with configurable true values.
- Rendering controls
  - Methods like GetCurrentLine, GoToLine, ClearNextLines enable in-place updates and structured screen management without external dependencies.
- Advanced outputs
  - Overwrite helpers (`OverwriteCurrentLine`, `Overwrite`, `Overwrite<TState>`) enable transient sections over error/out pipes for textual progress and reactive components.
  - TypeWrite/TypeWriteLine animate character-by-character output with a configurable delay.
- Menus and tables
  - Selection and MultiSelection render indexed lists and return chosen items; TreeMenu renders a two-level selection; Table prints header+columns from IList<string> inputs.
- Progress bars
  - IndeterminateProgressBar binds to a Task/Task<T> and renders an animated pattern until completion; its AnimationSequence is customizable and common patterns are provided.
  - ProgressBar tracks percentage (int/double 0–100), exposes properties like ProgressChar, ForegroundColor, ProgressColor.
- Packaging and targets
  - PrettyConsole.csproj multi-targets net9.0 and net8.0, is AOT compatible, and includes SourceLink. Internals are visible to PrettyConsole.Tests.Unit for deeper validation.

Testing structure and workflows

- PrettyConsole.Tests (interactive)
  - Program.cs constructs a list of visual tests/demos (e.g., IndeterminateProgressBarTest) and awaits each Render(). Modify the tests array for the scenarios you want to render live.
- PrettyConsole.Tests.Unit (xUnit v3)
  - Uses Microsoft.NET.Test.Sdk with the Microsoft Testing Platform runner; xunit.runner.json is included. Execute with dotnet run as shown above; pass filters after to narrow to a class or method.

Notes and gotchas

- The library aims to minimize allocations; prefer span-based overloads (ReadOnlySpan<char>, ReadOnlySpan<ColoredOutput>) for best performance when contributing.
- When authoring new features, pick the appropriate OutputPipe to keep CLI piping behavior intact.
- On macOS terminals, ANSI is supported; Windows legacy terminals are handled via ANSI-compatible rendering in the library.
