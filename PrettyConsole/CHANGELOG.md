# Changelog

## v3.1.0

* Updated to support .NET 9.0
* Updated to use `Sharpify 2.5.0`
* `ProgressBar` was redesigned since it randomly produced artifacts such as printing the header multiple times.
  * Now the buffer area of `ProgressBar` is only 1 line, the (now "status") is printed on the same line, after the bar
  * Only if the header is empty, the percentage is printed instead.
  * An internal lock is now also used to prevent race conditions.
* `ClearNextLines` show now work properly, previously it could actually clear 1 line too many.

### Breaking changes

* `OutputPipe` enum was added to unify APIs
* `ClearNextLinesError` was removed, use `ClearNextLines` with `OutputPipe.Error` instead
* `NewLineError` was removed, use `NewLine` with `OutputPipe.Error` instead
* `WriteError` was removed, use `Write` with `OutputPipe.Error` instead
* `WriteLineError` was removed, use `WriteLine` with `OutputPipe.Error` instead
* `OverrideCurrentLine` now has an option to choose the output pipe, use `OutputPipe.Error` by default
* `Write<T>` had the same treatment, now it has an option to choose the output pipe, use `OutputPipe.Out` by default, so the overload of `WriteError<T>` were removed
* `WriteLine<T>` and `WriteLine(ReadOnlySpan<char>)` were introduced thanks to this change
