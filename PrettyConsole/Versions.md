# CHANGELOG

## v3.0.0 - Alpha

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

## v2.1.1

* Removed redirection of `ReadOnlySpan<char>` print overload that degraded performance.
* Improved internal handling of array and memory pooling.
* Decreased formatted length limit for `ISpanFormattable` to 50 characters, as it was too long and frequent calls could cause the os to lag. Also changed to use rented buffer instead of stack allocation.

## v2.1.0

* Fixed default colors, previously colors where defaulted to `Color.Gray` for foreground and `Color.Black` for background, however many shells have custom colors that they render by default, which means the default colors seemed to render in a non ordinary fashion. The default colors now are `Color.Unknown` which will actually use the colors of the shell.
* `ProgressBar` now implements `IDisposable` since the implementation has been optimized, make sure to dispose of it, lest you induce a penalty on other such optimization which are now spread across the library.
* Introduced an overload to `Write` that accepts any `T : ISpanFormattable`. In many cases users might want to print structs or other types that implement this interface such as `int`, `double`, `DateTime` and more... splitting the output into a few lines and using this overload will enable you completely avoid the string allocation for this object, the overload is very optimized, writes it directly to the stack and prints it using a span. However, this limitation means that if the formatted item is longer than 256 characters, an exception will be thrown indicating that you should use a different overload.

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
* The progress bar types `ProgressBar` and `IndeterminateProgressBar` are now classes, and also have been substantially optimized.

## v1.6.1

* `TextRenderingScheme` type was added for better handling of outputs consisting of mixed colors
* `TypeWrite` now has a `TextRenderingScheme` overload and the `delay` in all overloads was increased to 200 (ms) to look more natural
* `Write`, `WriteLine`, `ReadLine` and `OverrideCurrentLine` now also have overloads with `TextRenderingScheme`, and they are the recommended ones for mixed color use, using them instead of the `params array` overload may allow further optimization during JIT compilation
* `Write` and `WriteLine` now have overloads for `ReadOnlySpan<char>` for even better performance
* More `try-finally` blocks have been implemented to further reduce the possibility of render failure upon exceptions
* More methods have been re-implemented to call their overloads for better maintainability and consistency
* More methods were marked as `Pure` to provide more information to the end users
* After testing it became rather clear, the best way to avoid glitches in frequently updated outputs inside event handlers, such as `ProgressBar`, `OverrideCurrentLine` and the likes is to firstly create an early return based on elapsed time after previous render, secondly, Use the `[MethodImpl(MethodImplOptions.Synchronized)]` On the event, and if the output is a Task, use `.Wait` instead of making the event async and awaiting it
* `ProgressBarDisplay` was modified to be a `record struct` instead of `ref struct`, This is aimed to increase usage flexibility which is limited with `ref struct`s, Also it may increase performance in edge cases due to higher potential for compiler optimization

## v1.6.0

* Re-structured `Console` as a `static partial class` into many files to separate the code into categorized section for better maintainability and improved workflow for adding new features
* Removed `synchronized method` compiler enforcements that could in some case limit usage flexibility when intentionally using `Task`s, if that feature is needed, simply create your own wrapper method with the attribute
* Update all places where the color of the output is changed to use `try-finally` block to ensure color reset even if an exception was thrown, which before could cause bugged colors
* Added more safeguards in key places
* Improved performance of various methods
* Merged closely related method implementations to reduce possibility of future errors
* **[POSSIBLE BREAKING CHANGE]** `ProgressBarDisplay` has been restructured to safeguard against improper initialization. Also a `ProgressChar` property was added to allow further customization of the progress bar look
* Added `TypeWrite` and `TypeWriteLine` methods that provide a simple type-writer effect for text.
* Fixed issue in `RequestAnyInput` that could read a pre-existing character from the input stream and accept it, basically skipping the entire request.

## v1.5.2

* **FROM THIS VERSION ON, THE PACKAGE WILL BE SIGNED**
* Updated many string interpolation instances to use `string.Concat` instead to improve performance
* Modified calculations in progress bars to use functions which are more hardware optimized
* Modified evaluated strings in progress bars to only be printed when done
* Added direction to synchronize progress bars to reduce buggy outputs
* Updated csproj file for better compatibility with the nuget website

This is a quality of life update, in most use cases the performance benefit, whether time wise or memory allocation will be visible and sometimes very significant.

This update doesn't introduce any breaking changes so updating is highly encouraged.

## v1.5.1

* Fixed issues with progress bar where the next override could keep artifacts of previous render.
* Added `OverrideCurrentLine()` which allows showing of progress with string only.
* Added `ClearNextLines(num)` which allows to remove next `num` lines, this is useful when something overriding was used, such as progress bar, but after parts of are left because the new output is shorter.

## v1.5.0

* Optimized code across the board for better performance and less memory allocation
* Improved organization and documentation
* Added more method overloads
* Added options to output errors to error pipeline to improve stds down the line
* Added trimming safe overloads for generic methods
* Added trim warnings for generic and reflection methods
* Added optional header to the progress bar

## v1.4.0

* Greatly improved performance by taking generics or strings into output methods instead of objects to avoid boxing.

### Upgrade notes

* Most methods, including `Write` and `WriteLine` with single parameter should not require any code modification as they will use generics
* Outputs methods which use `tuples` will require modification to enjoy the performance upgrade, with that said, existing code will be break because legacy `object` implementation was not removed but rather just marked as obsolete for now.

## v1.3.0

* Fixed memory leak in `TreeMenu`
* Improved performance and reduced complexity
* Removed `Colors.Primary` - it reduced uniformity as default was very close in color. Now default will be used in place of both, if you like primary more, consider overriding the `Colors.Default` to `ConsoleColor.White`
* Added Labels - outputs with configurable background color.

## v1.2.0

* Removed `Color` enum, use the `Colors` property instead
* Added progress bars, both regular and indeterminate
* Added documentation file so summaries will be visible

## v1.1.0

* Added indeterminate progress-bar
* Added regular progress-bar
* Changed parameter-less `Write` and `WriteLine` to use color version with default values to trim unnecessary code.

## v1.0.2

* Changed secondary color to be called the default color and also act as such, meaning by default `Write` and `WriteLine` will use it
* Internal outputs such as ReadLine or others will still use primary color

## v1.0.1

* Moved `Color` enum into Console to reduce using statements
* Changed accessibility of extensions, they were meant to be used internally
