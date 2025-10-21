using System.Runtime.InteropServices;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Represents a progress bar that can be displayed in the console.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The progress bar update isn't tied to unit of time, it's up to the user to update it as needed. By managing the when the Update method is called, the user can have a more precise control over the progress bar. More calls, means more frequent rendering but at the cost of performance (very frequent updates may cause the terminal to lose sync with the method and will produce visual bugs, such as items not rendering in the right place)
    /// </para>
    /// <para>
    /// Updating the progress bar with decreasing percentages will cause visual bugs as it is optimized to skip rendering pre-filled characters.
    /// </para>
    /// </remarks>
    public class ProgressBar {
        /// <summary>
        /// Gets or sets the character used to represent the progress.
        /// </summary>
        public char ProgressChar { get; set; } = '■';

        /// <summary>
        /// Gets or sets the foreground color of the progress bar.
        /// </summary>
        public ConsoleColor ForegroundColor { get; set; } = Color.DefaultForegroundColor;

        /// <summary>
        /// Gets or sets the color of the progress portion of the bar.
        /// </summary>
        public ConsoleColor ProgressColor { get; set; } = Color.DefaultForegroundColor;

        private int _currentProgress;

        private readonly Lock _lock = new();

        /// <summary>
        /// Updates the progress bar with the specified percentage.
        /// </summary>
        /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(int percentage) => Update(percentage, ReadOnlySpan<char>.Empty);

        /// <summary>
        /// Updates the progress bar with the specified percentage.
        /// </summary>
        /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(double percentage) => Update(percentage, ReadOnlySpan<char>.Empty);

        /// <summary>
        /// Updates the progress bar with the specified percentage and header text.
        /// </summary>
        /// <param name="percentage">The percentage value (0-100) representing the progress.</param>
        /// <param name="status">The status text to be displayed after the progress bar.</param>
        public void Update(double percentage, ReadOnlySpan<char> status) {
            // Non-locking fast path: compute the desired progress and early-return if unchanged.
            percentage = Math.Clamp(percentage, 0, 100);

            int bufferWidth = GetWidthOrDefault();
            // Compute pLength using exact overhead: " [" (2) + "] " (2) + percentage length (5)
            int pLength = Math.Max(0, bufferWidth - status.Length - 4 - 5);
            int p = Math.Clamp((int)(pLength * percentage * 0.01), 0, pLength);

            if (p == Volatile.Read(ref _currentProgress)) {
                return;
            }

            lock (_lock) {
                // It's possible another thread updated while we were waiting; re-check only the progress value.
                if (p == _currentProgress) {
                    return;
                }

                // Prepare the buffer exactly for the characters we will write for the bar (pLength)
                using var listOwner = BufferPool.Shared.Rent(out var list);
                list.EnsureCapacity(pLength);
                CollectionsMarshal.SetCount(list, pLength);
                Span<char> buf = CollectionsMarshal.AsSpan(list);

                _currentProgress = p;

                var currentLine = GetCurrentLine();
                try {
                    ResetColors();
                    baseConsole.ForegroundColor = ForegroundColor;
                    ClearNextLines(1, OutputPipe.Error);
                    if (status.Length != 0) {
                        Error.Write(status);
                    }
                    Error.Write(" [");
                    baseConsole.ForegroundColor = ProgressColor;

                    // Fill the progress portion
                    if (p > 0) {
                        Span<char> progressSpan = buf.Slice(0, p);
                        progressSpan.Fill(ProgressChar);
                    }

                    // Fill the remaining tail with spaces
                    int tailLength = Math.Max(0, pLength - p);
                    if (tailLength > 0) {
                        Span<char> whiteSpaceSpan = buf.Slice(p, tailLength);
                        whiteSpaceSpan.Fill(' ');
                    }

                    // Write the entire bar (progress + tail)
                    Error.Write(buf.Slice(0, pLength));

                    baseConsole.ForegroundColor = ForegroundColor;
                    Error.Write("] ");
                    // Write percentage
                    Write(OutputPipe.Error, $"{percentage,5:##.##}");
                    GoToLine(currentLine);
                } finally {
                    // Ensure colors and buffer are reset even if an exception occurs mid-render
                    ResetColors();
                }
            }
        }
    }
}