using System.Collections.ObjectModel;
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
        /// <summary>
        /// Contains the characters that will be iterated through while running
        /// </summary>
        /// <remarks>
        /// You can also choose from some defaults in <see cref="Patterns"/>
        /// </remarks>
        public ReadOnlyCollection<string> AnimationSequence { get; set; } = Patterns.Twirl;

        // A length of whitespace padding to the end
        private const int PaddingLength = 10;

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
        /// <remarks>Default = 200</remarks>
        public int UpdateRate { get; set; } = 200;

        private static readonly char[] TempBuffer = new char[20];

        /// <summary>
        /// Runs the indeterminate progress bar while the specified task is running.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="token"></param>
        /// <returns>The output of the running task</returns>
        public async Task<T> RunAsync<T>(Task<T> task, CancellationToken token = default) {
            return await RunAsync(task, string.Empty, token);
        }

        /// <summary>
        /// Runs the indeterminate progress bar while the specified task is running.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="header">The header which to display before the progress char</param>
        /// <param name="token"></param>
        /// <returns>The output of the running task</returns>
        public async Task<T> RunAsync<T>(Task<T> task, string header, CancellationToken token = default) {
            await RunAsyncNonGeneric(task, header, token);

            return task.IsCompleted ? task.Result : await task;
        }

        /// <summary>
        /// Runs the indeterminate progress bar while the specified task is running.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task RunAsync(Task task, CancellationToken token = default) {
            await RunAsync(task, string.Empty, token);
        }

        /// <summary>
        /// Runs the indeterminate progress bar while the specified task is running.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="header">The header which to display before the progress char</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task RunAsync(Task task, string header, CancellationToken token = default) {
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

            while (!task.IsCompleted && !token.IsCancellationRequested) {
                // Await until the TaskAwaiter informs of completion
                foreach (var c in AnimationSequence) {
                    if (header.Length > 0) {
                        Error.Write(header);
                        Error.Write(' ');
                    }

                    // Cycle through the characters of twirl
                    baseConsole.ForegroundColor = ForegroundColor;
                    Error.Write(c);
                    baseConsole.ForegroundColor = originalColor;
                    if (DisplayElapsedTime) {
                        var elapsed = Stopwatch.GetElapsedTime(startTime);
                        Error.Write(" [Elapsed: ");
                        Error.Write(Utils.FormatTimeSpan(elapsed, TempBuffer));
                        Error.Write(']');
                    }

                    Error.WriteWhiteSpaces(PaddingLength);
                    await Task.Delay(UpdateRate, token); // The update rate
                    ClearNextLines(1, OutputPipe.Error);
                    if (token.IsCancellationRequested) {
                        return;
                    }
                }
            }

            ResetColors();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private Task RunAsyncNonGeneric(Task task, string header, CancellationToken token) => RunAsync(task, header, token);

        /// <summary>
        /// Provides constant animation sequences that can be used for <see cref="AnimationSequence"/>
        /// </summary>
        public static class Patterns {
            /// <summary>
            /// A twirl animation sequence
            /// </summary>
            public static readonly ReadOnlyCollection<string> Twirl
                = new(["|", "/", "-", "\\"]);

            /// <summary>
            /// A bounce animation sequence
            /// </summary>
            public static readonly ReadOnlyCollection<string> Bounce
                = new(["<", ">", "=", "=", "<", ">"]);

            /// <summary>
            /// A dots animation sequence
            /// </summary>
            public static readonly ReadOnlyCollection<string> Dots
                = new([".", "o", "O", "°", "O", "o", "."]);

            /// <summary>
            /// A braille animation sequence
            /// </summary>
            public static readonly ReadOnlyCollection<string> Braille
                = new(["⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏"]);

            /// <summary>
            /// An arrow animation sequence
            /// </summary>
            public static readonly ReadOnlyCollection<string> Arrow
                = new(["-", "~", ">"]);

            /// <summary>
            /// A brackets animation sequence
            /// </summary>
            public static readonly ReadOnlyCollection<string> Brackets
                = new(["<", "(", "[", "{", "}", "]", ")", ">"]);

            /// <summary>
            /// A running person animation sequence
            /// </summary>
            public static readonly ReadOnlyCollection<string> RunningPerson
                = new(["🧍", "🚶‍➡️", "🏃‍➡️"]);

            /// <summary>
            /// A sad smiley animation sequence
            /// </summary>
            public static readonly ReadOnlyCollection<string> SadSmiley
                = new(["😞", "😣", "😖", "😫", "😩"]);
        }
    }
}