# Changelog

## v3.0.0

* Removed `Write` and `WriteLine` overloads that contains multiple `ColoredOutput`s, when using the overload with `ReadOnlySpan<ColoredOutput>` and `CollectionExpression`, the compiler generates an inline array to hold the elements, this is very efficient, and the reduced complexity allows usage of multiple `ColoredOutput`s in more places.
* All overloads of all functions that previously took only one `ColoredOutput` now take a `ReadOnlySpan<ColoredOutput>` instead, as noted it will use an inline array, and the internal implementation also has fast paths for size 1.
* All fast changing outputs (`ProgressBar`, `IndeterminateProgressBar`, `OverrideCurrentLine`) now uses the error output pipe by default, this means that if the consumer will pipe the output to another cli, it won't be filled with garbage and retain only the valuable stuff.
* Added `ReadOnlySpan<ColoredOutput>` overloads to `WriteError` and `WriteLineError`.
* Added overloads for all variants of `ReadLine` and `TryReadLine` that support `T @default` that will be returned if parsing fails.
* `Write`, `WriteLine`, `WriteError` and `WriteLineError` no longer have `params ColoredOutput[]` overloads, they instead have a `ReadOnlySpan<ColoredOutput>` overload. This performs even better as it uses an inline array, and the removal of the restrictions on where `params` can be used, allowed `ReadOnlySpan<ColoredOutput>` to replace virtually allow single `ColoredOutput` methods as well. Allowing greater customization in any function.
* You can now access the `In`, `Out`, and `Error` streams of `System.Console` directly from `PrettyConsole.Console` by using the properties `Out`, `Error`, and `In`. This reduces verbosity since the names of the classes have collisions.
* Added `SetColors` to allow setting the colors of the console output.
* `ClearNextLines` now also has a `ClearNextLinesError` brother which does the same for the `Error` stream.
* `NewLineError` was also added for this.
* To enhance customization in extreme high perf scenarios where you write a `ReadOnlySpan<char>` directly to the output stream, the `Write` and `WriteError` methods now have overloads that take a `ReadOnlySpan<char>` instead of a `ReadOnlySpan<ColoredOutput>`, along with foreground and background colors.
* Added `Write` and `WriteError` variants for `T : ISpanFormattable` as well.
* `ProgressBar` should now be even more performant as internal progress tracking allowed cutting the required operations by 20%.
* `Color` will now contain `static readonly ConsoleColor` fields for both `Foreground` and `Background` colors, they will be initialized during runtime to support all platforms (fixing the instant crashes on Windows).
  * You can also refer them when you want to use the defaults for any reason.
* The base methods which are used for outputting `ReadOnlySpan<ColoredOutput>` have been re-written to reduce assembly instructions, leading to about 15-20% runtime improvements across the board, and reducing by the binary size by a few bytes too lol.
* `Console` and `Color` now have the correct attributes to disallow compilation on unsupported platforms, if anyone tries to use them now (even thought it should've been obvious that they shouldn't be used on unsupported platforms), it should display the right message at build time.
* Removed all overloads that have options to change the input colors, it invites non-streamlined usage, and just increases the code complexity to maintain. Without them the code is simpler and more performant.
* A lot of methods that used white-spaces in any shape or form were now optimized using pre-initialized static buffer.
* `IndeterminateProgressBar` now has overloads that accept a `string header` that can be displayed before the progress char. There also was a change involving a secondary addition of catching the `CancellationToken`, removing up to 5 internal iterations.
* Added `GetCurrentLine` and `GoToLine` methods to allow efficiently using the same space in the console for continuous output, such as progress outputting, and general control flow.
