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
