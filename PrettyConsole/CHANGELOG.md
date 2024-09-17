# Changelog

## v3.0.0

* Removed `Write` and `WriteLine` overloads that contains multiple `ColoredOutput`s, when using the overload with `ReadOnlySpan<ColoredOutput>` and `CollectionExpression`, the compiler generates an inline array to hold the elements, this is very efficient, and the reduced complexity allows usage of multiple `ColoredOutput`s in more places.
* All overloads of all functions that previously took only one `ColoredOutput` now take a `ReadOnlySpan<ColoredOutput>` instead, as noted it will use an inline array, and the internal implementation also has fast paths for size 1.
* All fast changing outputs (`ProgressBar`, `IndeterminateProgressBar`, `OverrideCurrentLine`) now uses the error output pipe by default, this means that if the consumer will pipe the output to another cli, it won't be filled with garbage and retain only the valuable stuff.
* Added `ReadOnlySpan<ColoredOutput` overloads to `WriteError` and `WriteLineError`.
* Added overloads for all variants of `ReadLine` and `TryReadLine` that support `T @default` that will be returned if parsing fails.
