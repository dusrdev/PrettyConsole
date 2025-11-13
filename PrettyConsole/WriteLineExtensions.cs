namespace PrettyConsole;

/// <summary>
/// Provides methods extending the overloads of <see cref="Console.WriteLine(string)"/>.
/// </summary>
public static class WriteLineExtensions {
    extension(Console) {
        /// <summary>
        /// Writes interpolated content using <see cref="PrettyConsoleInterpolatedStringHandler"/> to <see cref="OutputPipe.Out"/>.
        /// </summary>
        /// <param name="handler">Interpolated string handler that streams the content.</param>
        [OverloadResolutionPriority(int.MaxValue)]
        public static void WriteLineInterpolated([InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) {
            Console.ResetColor();
            Console.NewLine(OutputPipe.Out);
        }

        /// <summary>
        /// Writes interpolated content using <see cref="PrettyConsoleInterpolatedStringHandler"/>.
        /// </summary>
        /// <param name="pipe">Destination pipe. Defaults to <see cref="OutputPipe.Out"/>.</param>
        /// <param name="handler">Interpolated string handler that streams the content.</param>
        public static void WriteLineInterpolated(OutputPipe pipe, [InterpolatedStringHandlerArgument(nameof(pipe))] PrettyConsoleInterpolatedStringHandler handler = default) {
            Console.ResetColor();
            Console.NewLine(pipe);
        }

        /// <summary>
        /// WriteLine an item that implements <see cref="ISpanFormattable"/> without boxing directly to the selected <see cref="OutputPipe"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pipe">The output pipe to use</param>
        /// <typeparam name="T"></typeparam>
        /// <remarks>
        /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
        /// </remarks>
        public static void WriteLine<T>(T item, OutputPipe pipe = OutputPipe.Out)
        where T : ISpanFormattable, allows ref struct {
            WriteLine(item, pipe, ConsoleColor.DefaultForeground, ConsoleColor.DefaultBackground, ReadOnlySpan<char>.Empty, null);
        }

        /// <summary>
        /// WriteLine an item that implements <see cref="ISpanFormattable"/> without boxing directly to the selected <see cref="OutputPipe"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pipe">The output pipe to use</param>
        /// <param name="foreground">foreground color</param>
        /// <typeparam name="T"></typeparam>
        /// <remarks>
        /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
        /// </remarks>
        public static void WriteLine<T>(T item, OutputPipe pipe, ConsoleColor foreground)
        where T : ISpanFormattable, allows ref struct {
            WriteLine(item, pipe, foreground, ConsoleColor.DefaultBackground, ReadOnlySpan<char>.Empty, null);
        }

        /// <summary>
        /// WriteLine an item that implements <see cref="ISpanFormattable"/> without boxing directly to the selected <see cref="OutputPipe"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pipe">The output pipe to use</param>
        /// <param name="foreground">foreground color</param>
        /// <param name="background">background color</param>
        /// <typeparam name="T"></typeparam>
        /// <remarks>
        /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
        /// </remarks>
        public static void WriteLine<T>(T item, OutputPipe pipe, ConsoleColor foreground,
            ConsoleColor background) where T : ISpanFormattable, allows ref struct {
            WriteLine(item, pipe, foreground, background, ReadOnlySpan<char>.Empty, null);
        }

        /// <summary>
        /// WriteLine an item that implements <see cref="ISpanFormattable"/> without boxing directly to the selected <see cref="OutputPipe"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pipe">The output pipe to use</param>
        /// <param name="foreground">foreground color</param>
        /// <param name="background">background color</param>
        /// <param name="format">item format</param>
        /// <param name="formatProvider">format provider</param>
        /// <typeparam name="T"></typeparam>
        /// <remarks>
        /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
        /// </remarks>
        public static void WriteLine<T>(T item, OutputPipe pipe, ConsoleColor foreground,
            ConsoleColor background, ReadOnlySpan<char> format, IFormatProvider? formatProvider)
        where T : ISpanFormattable, allows ref struct {
            Console.Write(item, pipe, foreground, background, format, formatProvider);
            Console.NewLine(pipe);
        }

        /// <summary>
        /// WriteLine a <see cref="ReadOnlySpan{Char}"/> without boxing directly to the selected <see cref="OutputPipe"/>.
        /// </summary>
        /// <param name="span"></param>
        /// <param name="pipe">The output pipe to use</param>
        public static void WriteLine(ReadOnlySpan<char> span, OutputPipe pipe) {
            Console.WriteLine(span, pipe, ConsoleColor.DefaultForeground, ConsoleColor.DefaultBackground);
        }

        /// <summary>
        /// WriteLine a <see cref="ReadOnlySpan{Char}"/> without boxing directly to the selected <see cref="OutputPipe"/>.
        /// </summary>
        /// <param name="span"></param>
        /// <param name="pipe">The output pipe to use</param>
        /// <param name="foreground">foreground color</param>
        public static void WriteLine(ReadOnlySpan<char> span, OutputPipe pipe, ConsoleColor foreground) {
            Console.WriteLine(span, pipe, foreground, ConsoleColor.DefaultBackground);
        }

        /// <summary>
        /// WriteLine a <see cref="ReadOnlySpan{Char}"/> without boxing directly to the selected <see cref="OutputPipe"/>.
        /// </summary>
        /// <param name="span"></param>
        /// <param name="pipe">The output pipe to use</param>
        /// <param name="foreground">foreground color</param>
        /// <param name="background">background color</param>
        public static void WriteLine(ReadOnlySpan<char> span, OutputPipe pipe, ConsoleColor foreground, ConsoleColor background) {
            Console.Write(span, pipe, foreground, background);
            Console.NewLine(pipe);
        }
    }
}