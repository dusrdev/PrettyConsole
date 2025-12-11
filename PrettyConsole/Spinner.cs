using System.Collections.ObjectModel;
using System.Diagnostics;

namespace PrettyConsole;

/// <summary>
/// Represents a spinner that visually indicates the progress of a time-consuming task.
/// </summary>
/// <remarks>
/// <para>
/// After the time-consuming task is completed, the spinner output is not cleared, use <see cref="RenderingExtensions.ClearNextLines(int, OutputPipe)"/> or <see cref="RenderingExtensions.SkipLines(int)"/> to handle the final output.
/// </para>
/// <para>
/// The cancellation token parameter on the RunAsync methods cancels the spinner itself (not necessarily the task) and ends it at any time.
/// </para>
/// </remarks>
public class Spinner {
    /// <summary>
    /// Contains the characters that will be iterated through while running.
    /// </summary>
    /// <remarks>Choose from the defaults in <see cref="Patterns"/></remarks>
    public ReadOnlyCollection<string> Pattern { get; init; } = Patterns.Twirl;

    /// <summary>
    /// Gets or sets the foreground color of the spinner.
    /// </summary>
    public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.DefaultForeground;

    /// <summary>
    /// Gets or sets a value indicating whether to display the elapsed time next to the spinner.
    /// </summary>
    public bool DisplayElapsedTime { get; init; } = true;

    /// <summary>
    /// Gets or sets the update rate (in ms) of the spinner frames.
    /// </summary>
    /// <remarks>Default = 200</remarks>
    public int UpdateRate { get; init; } = 200;

    /// <summary>
    /// Runs the spinner while the specified task is running.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="token"></param>
    /// <returns>The output of the running task</returns>
    public async Task<T> RunAsync<T>(Task<T> task, CancellationToken token = default) {
        await RunAsyncNonGeneric(task, null, token);

        return task.IsCompleted ? task.Result : await task;
    }

    /// <summary>
    /// Runs the spinner while the specified task is running.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="header"></param>
    /// <param name="token"></param>
    public async Task<T> RunAsync<T>(Task<T> task, string header, CancellationToken token = default) {
        await RunAsyncNonGeneric(task, (builder, out handler) => handler = builder.Build(OutputPipe.Error, $"{header}"), token);

        return task.IsCompleted ? task.Result : await task;
    }

    /// <summary>
    /// Runs the spinner while the specified task is running, using a dynamic header factory.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="headerFactory">Factory invoked every frame to render a header with <see cref="PrettyConsoleInterpolatedStringHandler"/>.</param>
    /// <param name="token"></param>
    /// <returns>The output of the running task.</returns>
    public async Task<T> RunAsync<T>(Task<T> task, PrettyConsoleInterpolatedStringHandlerFactory? headerFactory, CancellationToken token = default) {
        await RunAsyncNonGeneric(task, headerFactory, token);

        return task.IsCompleted ? task.Result : await task;
    }

    /// <summary>
    /// Runs the spinner while the specified task is running.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task RunAsync(Task task, CancellationToken token = default) => RunAsyncNonGeneric(task, null, token);

    /// <summary>
    /// Runs the spinner while the specified task is running.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="header"></param>
    /// <param name="token"></param>
    public Task RunAsync(Task task, string header, CancellationToken token = default) {
        return RunAsyncNonGeneric(task, (builder, out handler) => handler = builder.Build(OutputPipe.Error, $"{header}"), token);
    }

    /// <summary>
    /// Runs the spinner while the specified task is running, using a dynamic header factory.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="headerFactory">Factory invoked every frame to render a header with <see cref="PrettyConsoleInterpolatedStringHandler"/>.</param>
    /// <param name="token"></param>
    public Task RunAsync(Task task, PrettyConsoleInterpolatedStringHandlerFactory? headerFactory, CancellationToken token = default) => RunAsyncNonGeneric(task, headerFactory, token);

    /// <summary>
    /// Runs the spinner while the specified task is running, using a dynamic header factory.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="headerFactory">Factory invoked every frame to render a header with <see cref="PrettyConsoleInterpolatedStringHandler"/>.</param>
    /// <param name="token"></param>
    private async Task RunAsyncNonGeneric(Task task, PrettyConsoleInterpolatedStringHandlerFactory? headerFactory, CancellationToken token) {
        try {
            if (task.Status is not TaskStatus.Running) {
                task.Start();
            }
        } catch {
            //ignore
        }

        long startTime = Stopwatch.GetTimestamp();
        long updateRateAsTicks = TimeSpan.FromMilliseconds(UpdateRate).Ticks;

        // Maintain a stable cadence that accounts for render time
        long nextTick = startTime;
        int seqIndex = 0;

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);

        // Cancel the delay token as soon as the bound task completes
        _ = task.ContinueWith(static (_, state) => ((CancellationTokenSource)state!).Cancel(), linkedCts,
            CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

        while (!task.IsCompleted && !token.IsCancellationRequested) {
            Console.ClearNextLines(1, OutputPipe.Error); // Clear at start to prevent auto-delete after last write

            Console.WriteInterpolated(OutputPipe.Error, $"{ForegroundColor}{Pattern[seqIndex]}{ConsoleColor.DefaultForeground}");

            if (headerFactory is not null) {
                ConsoleContext.Error.WriteWhiteSpaces(1);
                headerFactory.Invoke(PrettyConsoleInterpolatedStringHandlerBuilder.Singleton, out var handler);
                handler.Flush();
            }

            if (DisplayElapsedTime) {
                var elapsed = Stopwatch.GetElapsedTime(startTime);
                Console.WriteInterpolated(OutputPipe.Error, $" [Elapsed: {elapsed:duration}]");
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

            if (token.IsCancellationRequested || task.IsCompleted) {
                break;
            }

            // Advance animation sequence index without allocations
            seqIndex++;
            if (seqIndex == Pattern.Count) {
                seqIndex = 0;
            }
        }
    }

    /// <summary>
    /// Provides constant animation sequences that can be used for <see cref="Pattern"/>
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