using System.Collections.ObjectModel;
using System.Diagnostics;

namespace PrettyConsole;

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

    /// <summary>
    /// Gets or sets the foreground color of the progress bar.
    /// </summary>
    public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.DefaultForeground;

    /// <summary>
    /// Gets or sets a value indicating whether to display the elapsed time in the progress bar.
    /// </summary>
    public bool DisplayElapsedTime { get; set; } = true;

    /// <summary>
    /// Gets or sets the update rate (in ms) of the indeterminate progress bar.
    /// </summary>
    /// <remarks>Default = 200</remarks>
    public int UpdateRate { get; set; } = 200;

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

        Console.ResetColor();
        ConsoleColor originalColor = Console.ForegroundColor;
        long startTime = Stopwatch.GetTimestamp();
        long updateRateAsTicks = TimeSpan.FromMilliseconds(UpdateRate).Ticks;

        // Maintain a stable cadence that accounts for render time
        long nextTick = startTime;
        int seqIndex = 0;

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);

        // Cancel the delay token as soon as the bound task completes
        _ = task.ContinueWith(static (t, state) => ((CancellationTokenSource)state!).Cancel(), linkedCts,
            CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

        while (!task.IsCompleted && !token.IsCancellationRequested) {
            try {
                Console.ForegroundColor = ForegroundColor;
                PrettyConsoleExtensions.Error.Write(AnimationSequence[seqIndex]);
            } finally {
                Console.ForegroundColor = originalColor;
            }

            if (header.Length > 0) {
                Console.WriteInterpolated(OutputPipe.Error, $" {header}");
            }

            if (DisplayElapsedTime) {
                var elapsed = Stopwatch.GetElapsedTime(startTime);
                Console.WriteInterpolated(OutputPipe.Error, $" [Elapsed: {elapsed:hr}]");
            }

            // Compute sleep to maintain UpdateRate between frame starts
            var now = Stopwatch.GetTimestamp();
            nextTick += updateRateAsTicks;
            var remaining = nextTick - now;

            if (remaining <= 0) {
                // If we are late by >= one period, snap schedule to now to avoid burst catch-up
                if (-remaining >= updateRateAsTicks) {
                    nextTick = now;
                }
            } else {
                try {
                    // Coarse delay for most of the remainder
                    var remainingMs = (int)TimeSpan.FromTicks(remaining).TotalMilliseconds;
                    if (remainingMs > 1) {
                        await Task.Delay(remainingMs - 1, linkedCts.Token).ConfigureAwait(false);
                    }
                    // Fine spin for the last ~1ms to improve smoothness
                    var sw = new SpinWait();
                    while (!linkedCts.IsCancellationRequested && Stopwatch.GetTimestamp() < nextTick) {
                        sw.SpinOnce();
                    }
                } catch (OperationCanceledException) {
                    // Either external cancellation or task completed
                }
            }

            // Always clear once per frame
            Console.ClearNextLines(1, OutputPipe.Error);

            if (token.IsCancellationRequested || task.IsCompleted) {
                break;
            }

            // Advance animation sequence index without allocations
            seqIndex++;
            if (seqIndex == AnimationSequence.Count) {
                seqIndex = 0;
            }
        }

        Console.ResetColor();
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
        /// A braille animation sequence
        /// </summary>
        public static readonly ReadOnlyCollection<string> Braille
            = new(["⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏"]);

        /// <summary>
        /// A running person animation sequence
        /// </summary>
        public static readonly ReadOnlyCollection<string> RunningPerson
            = new(["🧎‍➡️", "🧍", "🚶‍➡️", "🏃‍➡️", " "]);

        /// <summary>
        /// A sad smiley animation sequence ("what's taking so long??")
        /// </summary>
        public static readonly ReadOnlyCollection<string> SadSmiley
            = new(["😞", "😣", "😖", "😫", "😩", " "]);

        /// <summary>
        /// A loading-bar animation sequence
        /// </summary>
        public static readonly ReadOnlyCollection<string> LoadingBar
            = new(["[    ]", "[=   ]", "[==  ]", "[=== ]", "[====]", "[ ===]", "[  ==]", "[   =]", "[    ]"]);

        /// <summary>
        /// An ASCII ping-pong animation sequence
        /// </summary>
        public static readonly ReadOnlyCollection<string> PingPong
            = new([
                "|•    |",
                    "| •   |",
                    "|  •  |",
                    "|   • |",
                    "|    •|",
                    "|   • |",
                    "|  •  |",
                    "| •   |",
            ]);
    }
}