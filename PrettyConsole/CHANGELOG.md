# CHANGELOG

## v2.1.0

* Fixed default colors, previously colors where defaulted to `Color.Gray` for foreground and `Color.Black` for background, however many shells have custom colors that they render by default, which means the default colors seemed to render in a non ordinary fashion. The default colors now are `Color.Unknown` which will actually use the colors of the shell.
* `ProgressBar` now implements `IDisposable` since the implementation has been optimized, make sure to dispose of it, lest you induce a penalty on other such optimization which are now spread across the library.
* Introduced an overload to `Write` that accepts any `T : ISpanFormattable`. In many cases users might want to print structs or other types that implement this interface such as `int`, `double`, `DateTime` and more... splitting the output into a few lines and using this overload will enable you completely avoid the string allocation for this object, the overload is very optimized, writes it directly to the stack and prints it using a span. However, this limitation means that if the formatted item is longer than 256 characters, an exception will be thrown indicating that you should use a different overload.

### Note

Reviewing the codebase, I can already see that there are many places which will greatly benefit from the upcoming features of .NET 9. Features such as the `params ReadOnlySpan<T>` feature would enable me to delete a lot of overloads without decreasing compatibility, yet gain a performance boost.
Expect a new version soon after .NET 9 is released.
