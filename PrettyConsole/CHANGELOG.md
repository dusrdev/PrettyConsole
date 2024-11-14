# Changelog

## v3.1.0

* Updated to support .NET 9.0
* Updated to use `Sharpify 2.5.0`
* `ProgressBar` was redesigned since it randomly produced artifacts such as printing the header multiple times.
  * Now the buffer area of `ProgressBar` is only 1 line, the header is printed on the same line, after the bar
  * Only if the header is empty, the percentage is printed instead.
  * An internal lock is now also used to prevent race conditions.
* `ClearNextLines` show now work properly, previously it could actually clear 1 line too many.
