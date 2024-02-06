# CHANGELOG

## v2.0.0

* All previous variants of output types were removed, now the main output type is `ColoredOutput` (which includes a string, foreground color and background color).
Thanks to implicit and other converters and the added supporting `Color` class,
usage is even simpler, and much more performant.
* Overloads of many functions were changed, many were deleted.
* `ReadLine<T>` and new overloads `TryReadLine<T>` now support reflection free parsing for any `IParsable<T>`
implementing types (which are most of the base types, and you can create any custom implementations if you choose so).
* `ReadLine` now also has an `Enum` overload that can parse for enums, with configurable case sensitivity.
* Many functions, especially more advance outputs such as selections, menus and progress bar, had undergone tremendous performance optimizations, and show now perform extremely efficiently.
* A new table view implementation was added.
