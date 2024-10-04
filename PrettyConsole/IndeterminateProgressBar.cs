using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Represents an indeterminate progress bar that visually indicates the progress of a time-consuming task.
    /// </summary>
    /// <remarks>
    /// <para>
    /// After the time-consuming task is completed, the progress bar is removed from the console. and the next output will take its place.
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
        public ConsoleColor ForegroundColor { get; set; } = Color.DefaultForegroundColor;

        /// <summary>
        /// Gets or sets a value indicating whether to display the elapsed time in the progress bar.
        /// </summary>
        public bool DisplayElapsedTime { get; set; } = true;

        /// <summary>
        /// Gets or sets the update rate (in ms) of the indeterminate progress bar.
        /// </summary>
        public int UpdateRate { get; set; } = 50;

        private readonly string _emptyLine;

        /// <summary>
        /// Represents an indeterminate progress bar that continuously animates without a specific progress value.
        /// </summary>
        public IndeterminateProgressBar() {
            _emptyLine = new string(' ', baseConsole.BufferWidth);
        }

        /// <summary>
        /// Runs the indeterminate progress bar while the specified task is running.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="token"></param>
        /// <returns>The output of the running task</returns>
        public async Task<T> RunAsync<T>(Task<T> task, CancellationToken token = default) {
            await RunAsyncNonGeneric(task, token);

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
            var originalColor = baseConsole.ForegroundColor;
            var startTime = Stopwatch.GetTimestamp();
            var lineNum = baseConsole.CursorTop;

            using var memoryOwner = Utils.ObtainMemory(20);

            while (!task.IsCompleted) {
                // Await until the TaskAwaiter informs of completion
                foreach (var c in Twirl) {
                    // Cycle through the characters of twirl
                    baseConsole.ForegroundColor = ForegroundColor;
                    Error.Write(c);
                    baseConsole.ForegroundColor = originalColor;
                    if (DisplayElapsedTime) {
                        var elapsed = Stopwatch.GetElapsedTime(startTime);
                        Error.Write(" [Elapsed: ");
                        baseConsole.Error.Write(Utils.FormatElapsedTime(elapsed, memoryOwner.Memory.Span));
                        Error.Write(']');
                    }

                    Error.Write(ExtraBuffer);

                    baseConsole.SetCursorPosition(0, lineNum);
                    await Task.Delay(UpdateRate, token); // The update rate
                    Error.Write(_emptyLine);
                    baseConsole.SetCursorPosition(0, lineNum);
                    if (token.IsCancellationRequested) {
                        return;
                    }
                }
            }

            ResetColors();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private Task RunAsyncNonGeneric(Task task, CancellationToken token) => RunAsync(task, token);
    }
}