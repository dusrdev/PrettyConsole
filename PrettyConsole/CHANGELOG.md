# CHANGELOG

## v1.5.2

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
