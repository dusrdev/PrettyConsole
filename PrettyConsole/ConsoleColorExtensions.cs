namespace PrettyConsole;

/// <summary>
/// Provides methods extending <see cref="ConsoleColor"/>;
/// </summary>
public static class ConsoleColorExtensions {
    /// <summary>
    /// Returns the default foreground color for the shell.
    /// </summary>
    public static readonly ConsoleColor DefaultForegroundColor;

    /// <summary>
    /// Returns the default background color for the shell.
    /// </summary>
    public static readonly ConsoleColor DefaultBackgroundColor;

    static ConsoleColorExtensions() {
        Console.ResetColor();
        DefaultForegroundColor = Console.ForegroundColor;
        DefaultBackgroundColor = Console.BackgroundColor;
    }

    extension(ConsoleColor) {
        /// <summary>
        /// Returns the default foreground color for the shell.
        /// </summary>
        public static ConsoleColor DefaultForeground => DefaultForegroundColor;

        /// <summary>
        /// Returns the default background color for the shell.
        /// </summary>
        public static ConsoleColor DefaultBackground => DefaultBackgroundColor;

        /// <summary>
        /// Returns a tuple of (<see cref="DefaultForegroundColor"/>, <see cref="DefaultBackgroundColor"/>)
        /// </summary>
        public static (ConsoleColor, ConsoleColor) Default => (DefaultForegroundColor, DefaultBackgroundColor);

        /// <summary>
        /// Returns a tuple of (<paramref name="foreground"/>, <paramref name="background"/>)
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (ConsoleColor, ConsoleColor) operator /(ConsoleColor foreground, ConsoleColor background) {
            return (foreground, background);
        }

        /// <summary>
        /// Returns a tuple of (<paramref name="foreground"/>, <paramref name="colorTuple"/>)
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="colorTuple"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (ConsoleColor, ConsoleColor) operator /(ConsoleColor foreground, (ConsoleColor tupleForeground, ConsoleColor tupleBackground) colorTuple) {
            return (foreground, colorTuple.tupleBackground);
        }
    }
}