using System.Diagnostics;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Represents an indeterminate progress bar that visually indicates the progress of a time-consuming task.
    /// </summary>
    /// <remarks>
    /// <para>
    /// After the time consuming task is completed, the progress bar is removed from the console. and the next output will take its place.
    /// </para>
    /// <para>
    /// The cancellation token parameter on the RunAsync methods is to cancel the progress bar (not necessarily the task) and end it any time.
    /// </para>
    /// </remarks>
    public class IndeterminateProgressBar {
        // Constant pattern containing the characters needed for the indeterminate progress bar
        private const string Twirl = "-\\|/";

        // A whitespace the length of 10 spaces
        private const string ExtraBuffer = "          ";

        /// <summary>
        /// Gets or sets the foreground color of the progress bar.
        /// </summary>
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Gray;

        /// <summary>
        /// Gets or sets a value indicating whether to display the elapsed time in the progress bar.
        /// </summary>
        public bool DisplayElapsedTime { get; set; } = true;

        /// <summary>
        /// Gets or sets the update rate (in ms) of the indeterminate progress bar.
        /// </summary>
        public int UpdateRate { get; set; } = 50;

        private readonly char[] _emptyLine;

        /// <summary>
        /// Represents an indeterminate progress bar that continuously animates without a specific progress value.
        /// </summary>
        public IndeterminateProgressBar() {
            _emptyLine = GC.AllocateUninitializedArray<char>(ogConsole.BufferWidth);
            _emptyLine.AsSpan().Fill(' ');
        }

        /// <summary>
        /// Runs the indeterminate progress bar while the specified task is running.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="token"></param>
        /// <returns>The output of the running task</returns>
        public async Task<T> RunAsync<T>(Task<T> task, CancellationToken token = default) {
            await RunAsync(task, token);

            return task.IsCompleted ? task.Result : await task;
        }

        /// <summary>
        /// Runs the indeterminate progress bar while the specified task is running.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task RunAsync(Task task, CancellationToken token = default) {
            try {
                if (task.Status is not TaskStatus.Running) {
                    task.Start();
                }
            } catch {
                //ignore
            }

            ResetColors();
            var originalColor = ogConsole.ForegroundColor;
            var startTime = Stopwatch.GetTimestamp();
            var lineNum = ogConsole.CursorTop;

            while (!task.IsCompleted) {
                // Await until the TaskAwaiter informs of completion
                foreach (char c in Twirl) {
                    // Cycle through the characters of twirl
                    ogConsole.ForegroundColor = ForegroundColor;
                    ogConsole.Write(c);
                    ogConsole.ForegroundColor = originalColor;
                    if (DisplayElapsedTime) {
                        var elapsed = Stopwatch.GetElapsedTime(startTime);
                        ogConsole.Write(" [Elapsed: ");
                        ogConsole.Write(elapsed.ToFriendlyString());
                        ogConsole.Write(']');
                    }

                    ogConsole.Write(ExtraBuffer);

                    ogConsole.SetCursorPosition(0, lineNum);
                    await Task.Delay(UpdateRate, token); // The update rate
                    ogConsole.Write(_emptyLine);
                    ogConsole.SetCursorPosition(0, lineNum);
                    if (token.IsCancellationRequested) {
                        return;
                    }
                }
            }

            ResetColors();
        }
    }
}