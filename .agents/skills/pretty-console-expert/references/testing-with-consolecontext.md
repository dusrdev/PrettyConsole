# Testing PrettyConsole CLIs With `ConsoleContext`

Use this file when a task involves testing a CLI or command layer that already uses PrettyConsole internally.

## When This Applies

Prefer this approach when:

- the CLI logic already lives in callable commands, handlers, or functions
- those commands write via PrettyConsole APIs such as `Console.WriteInterpolated(...)`, `Console.WriteLineInterpolated(...)`, `Console.NewLine(...)`, `Console.TryReadLine(...)`, or `Console.Confirm(...)`
- you want fast, deterministic tests for stdout/stderr routing, prompts, parsed input, and returned exit codes/results

Do not default to `Process` in those cases. PrettyConsole already gives you an in-process seam through `ConsoleContext.Out`, `ConsoleContext.Error`, and `ConsoleContext.In`.

## Default Testing Pattern

1. Save the current `ConsoleContext.Out`, `ConsoleContext.Error`, and `ConsoleContext.In`.
2. Replace them with test doubles such as `StringWriter` and `StringReader`.
3. Invoke the same command handler or function the CLI entrypoint uses internally.
4. Assert on:
   - returned exit code or result
   - stdout text from `ConsoleContext.Out`
   - stderr text from `ConsoleContext.Error`
5. Restore the original streams in `finally`.

## Output Assertions

- Use separate writers for `Out` and `Error`. Do not merge them unless the test intentionally treats both pipes as one stream.
- Assert on the selected pipe instead of parsing combined process output.
- If the test only cares about visible content, assert on the text written to the injected `StringWriter`.
- When testing pipe routing, assert that expected content landed on the correct writer and the other one stayed empty.

Example:

```csharp
var originalOut = ConsoleContext.Out;
var originalError = ConsoleContext.Error;
var stdout = new StringWriter();
var stderr = new StringWriter();

try {
    ConsoleContext.Out = stdout;
    ConsoleContext.Error = stderr;

    int exitCode = await RunCommandAsync(options, cancellationToken);

    await Assert.That(exitCode).IsEqualTo(0);
    await Assert.That(stdout.ToString()).Contains("Completed");
    await Assert.That(stderr.ToString()).IsEmpty();
} finally {
    ConsoleContext.Out = originalOut;
    ConsoleContext.Error = originalError;
}
```

## Input Assertions

- Feed stdin with `ConsoleContext.In = new StringReader("value" + Environment.NewLine)`.
- Call the real command/handler code directly.
- Assert on both the parsed result and the prompt written to the selected output pipe.

Example:

```csharp
var originalOut = ConsoleContext.Out;
var originalIn = ConsoleContext.In;
var stdout = new StringWriter();

try {
    ConsoleContext.Out = stdout;
    ConsoleContext.In = new StringReader("42" + Environment.NewLine);

    int value = ReadPortFromUser();

    await Assert.That(value).IsEqualTo(42);
    await Assert.That(stdout.ToString()).Contains("Port");
} finally {
    ConsoleContext.Out = originalOut;
    ConsoleContext.In = originalIn;
}
```

## Suggested Shape For Consumer CLIs

Prefer this structure when a consumer CLI is testable but currently shells out to itself:

- thin entrypoint: parse args, compose dependencies, call a command handler/function
- command handler/function: returns an `int`, `Task<int>`, result object, or domain value
- all console I/O stays inside PrettyConsole calls, so tests can intercept it through `ConsoleContext`

This lets tests call the command handler directly instead of launching a child process and scraping console output.

## When `Process` Is Still Appropriate

Use `Process` only when the behavior truly exists at the process boundary, for example:

- verifying entrypoint wiring or host bootstrapping
- testing the real argument parser if it only runs in `Main`
- checking environment variables, current directory, exit codes from the actual executable, or shell integration
- validating published-binary behavior or packaging/install flows

Even then, keep the number of process-level tests small and focused. Most behavioral coverage should stay in-process.

## Anti-Pattern To Avoid

Avoid this default approach for PrettyConsole-based CLIs:

- launch the full executable with `Process`
- pass arguments
- capture combined output
- scrape strings to infer stdout/stderr behavior that the code already exposes directly through `ConsoleContext`

That style is slower, less precise, and makes it harder to verify separate pipes or inject interactive input.

## Extra Notes

- If the command uses both `Out` and `Error`, always keep two writers so routing bugs stay visible.
- If the command uses `Console.TryReadLine(...)`, `Console.ReadLine(...)`, or `Console.Confirm(...)`, inject `ConsoleContext.In` instead of trying to fake terminal input through a spawned process.
- If you need to verify transient UI behavior at a lower level, prefer calling the underlying render/update methods directly and asserting on the injected writers before reaching for process tests.
