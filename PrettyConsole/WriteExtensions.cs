using System.Buffers;

namespace PrettyConsole;

/// <summary>
/// Provides methods extending the overloads of <see cref="Console.Write(string)"/>.
/// </summary>
public static class WriteExtensions {
    extension(Console) {
        /// <summary>
        /// Writes interpolated content using <see cref="PrettyConsoleInterpolatedStringHandler"/> to <see cref="OutputPipe.Out"/>.
        /// </summary>
        /// <param name="handler">Interpolated string handler that streams the content.</param>
        public static void WriteInterpolated([InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) {
            Console.ResetColor();
        }

        /// <summary>
        /// Writes interpolated content using <see cref="PrettyConsoleInterpolatedStringHandler"/>.
        /// </summary>
        /// <param name="pipe">Destination pipe. Defaults to <see cref="OutputPipe.Out"/>.</param>
        /// <param name="handler">Interpolated string handler that streams the content.</param>
        public static void WriteInterpolated(OutputPipe pipe, [InterpolatedStringHandlerArgument(nameof(pipe))] PrettyConsoleInterpolatedStringHandler handler = default) {
            Console.ResetColor();
        }

        /// <summary>
        /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the selected <see cref="OutputPipe"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pipe">The output pipe to use</param>
        /// <typeparam name="T"></typeparam>
        /// <remarks>
        /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
        /// </remarks>
        public static void Write<T>(T item, OutputPipe pipe = OutputPipe.Out)
        where T : ISpanFormattable, allows ref struct {
            Write(item, pipe, ConsoleColor.DefaultForeground, ConsoleColor.DefaultBackground, ReadOnlySpan<char>.Empty, null);
        }

        /// <summary>
        /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the selected <see cref="OutputPipe"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pipe">The output pipe to use</param>
        /// <param name="foreground">foreground color</param>
        /// <typeparam name="T"></typeparam>
        /// <remarks>
        /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
        /// </remarks>
        public static void Write<T>(T item, OutputPipe pipe, ConsoleColor foreground)
        where T : ISpanFormattable, allows ref struct {
            Write(item, pipe, foreground, ConsoleColor.DefaultBackground, ReadOnlySpan<char>.Empty, null);
        }

        /// <summary>
        /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the selected <see cref="OutputPipe"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pipe">The output pipe to use</param>
        /// <param name="foreground">foreground color</param>
        /// <param name="background">background color</param>
        /// <typeparam name="T"></typeparam>
        /// <remarks>
        /// This function iteratively grows a rented span until formatting is successful, starting at capacity = 256, to ensure the fastest execution speed, it is recommend that <typeparamref name="T"/> would be able to format to a smaller length string than that.
        /// </remarks>
        public static void Write<T>(T item, OutputPipe pipe, ConsoleColor foreground, ConsoleColor background)
        where T : ISpanFormattable, allows ref struct {
            Write(item, pipe, foreground, background, ReadOnlySpan<char>.Empty, null);
        }

        /// <summary>
        /// Writes an item that implements <see cref="ISpanFormattable"/> without boxing directly to the selected <see cref="OutputPipe"/>.
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
        public static void Write<T>(T item, OutputPipe pipe, ConsoleColor foreground,
            ConsoleColor background, ReadOnlySpan<char> format, IFormatProvider? formatProvider)
        where T : ISpanFormattable, allows ref struct {
            int lowerBound = 4096;
            var pool = ArrayPool<char>.Shared;

            while (true) {
                var array = pool.Rent(lowerBound);
                try {
                    var buffer = new Span<char>(array);
                    if (item.TryFormat(array, out int charsWritten, format, formatProvider)) {
                        Write(buffer.Slice(0, charsWritten), pipe, foreground, background);
                        return;
                    }
                } finally {
                    pool.Return(array);
                }
                lowerBound *= 2;
            }
        }

        /// <summary>
        /// Writes a <see cref="ReadOnlySpan{Char}"/> without boxing directly to the selected <see cref="OutputPipe"/>.
        /// </summary>
        /// <param name="span"></param>
        /// <param name="pipe">The output pipe to use</param>
        public static void Write(ReadOnlySpan<char> span, OutputPipe pipe) {
            Write(span, pipe, ConsoleColor.DefaultForeground, ConsoleColor.DefaultBackground);
        }

        /// <summary>
        /// Writes a <see cref="ReadOnlySpan{Char}"/> without boxing directly to the selected <see cref="OutputPipe"/>.
        /// </summary>
        /// <param name="span"></param>
        /// <param name="pipe">The output pipe to use</param>
        /// <param name="foreground">foreground color</param>
        public static void Write(ReadOnlySpan<char> span, OutputPipe pipe, ConsoleColor foreground) {
            Write(span, pipe, foreground, ConsoleColor.DefaultBackground);
        }

        /// <summary>
        /// Writes a <see cref="ReadOnlySpan{Char}"/> without boxing directly to the selected <see cref="OutputPipe"/>.
        /// </summary>
        /// <param name="span"></param>
        /// <param name="pipe">The output pipe to use</param>
        /// <param name="foreground">foreground color</param>
        /// <param name="background">background color</param>
        public static void Write(ReadOnlySpan<char> span, OutputPipe pipe, ConsoleColor foreground, ConsoleColor background) {
            Console.SetColors(foreground, background);
            PrettyConsoleExtensions.GetWriter(pipe).Write(span);
            Console.ResetColor();
        }
    }
}