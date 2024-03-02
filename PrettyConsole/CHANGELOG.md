# CHANGELOG

## v2.0.0

* All previous variants of output types were removed, now the main output type is `ColoredOutput` (which includes a string, foreground color and background color).
Thanks to implicit and other converters and the added supporting `Color` class,
usage is even simpler, and much more performant.

New you can create colored outputs in a sort of mathematical multiplication syntax:

$$string \times \dfrac{foreground}{background}$$

or in C# example

```csharp
var output = "string" * Color.Black / Color.Green; // Black Text with Green Background
var noBackground = "string" * Color.Green; // Default background, green foreground
var differentBackground = "string" / Color.Magenta; // Default foreground (gray) with Magenta background
```

As you can see it behaves like the mathematical equation above, where you can emit skip the parts you don't want to change.

It is important to use the `Color` class and its static fields for colors instead of `ConsoleColor`, this is for the implicit operators. However know that internally, each `Color` is a very light wrapper around `ConsoleColor` and implicitly converts to it when rendering to the console. So this provides very little to no overhead. There are static fields in `Color` that correspond to every `ConsoleColor` + `Default` which is `ConsoleColor.Gray`

* Overloads of many functions were changed, many were deleted.
* `ReadLine<T>` and new overloads `TryReadLine<T>` now support reflection free parsing for any `IParsable<T>`
implementing types (which are most of the base types, and you can create any custom implementations if you choose so).
* `TryReadLine` now also has an `Enum` overload that can parse for enums, with configurable case sensitivity.
* Many functions, especially more advance outputs such as selections, menus and progress bar, had undergone tremendous performance optimizations, and show now perform extremely efficiently.
* A new table view implementation was added.
* `ProgressBar` and `IndeterminateProgressBar` are now classes, which upon initialization take care of building the foundation for their respective actions, this enables better size awareness, as previously the progress bar would always render depending on the size of the terminal when launched, now, it will depend on the size of the terminal when the class was created, possibly giving producing a more accurate output if the user resized the terminal window.
* The change of `ProgressBar` to class, aside from performance improvement due to reduced copying of values from the heap/stack, now enables easier customization even mid-run. The properties of the class can be changed and will be reflected on the next render.
* `IndeterminateProgressBar` and `ProgressBar` had massive memory allocation optimizations, mainly by using a mutable span which serves as a buffer for frequently formatted output (elapsed time / progress percentage respectively), then directly printing the span, previously each format like this (possibly thousands) would create a string, which would either be pinned in the heap or need to be garbage collected, now, not a single string is created for this purpose.
