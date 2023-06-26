using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// A simple twirl style indeterminate progress bar to signal the user that the app is not stuck but rather is performing a time consuming task.
    /// <para>
    /// The output is cleared so the next line will be written on the same line as the progress bar was.
    /// </para>
    /// </summary>
    /// <param name="task">The task you want to await on, it will not be modified, only the state is observed</param>
    /// <param name="color">The color in which to display the progress bar</param>
    /// <param name="title">Message to display alongside the progress bar</param>
    /// <param name="displayElapsedTime">Display elapsed time</param>
    /// <param name="updateRate">Rate at which the progress bar refreshes in milliseconds</param>
    /// <param name="token">So you can cancel the progress bar and end it any time</param>
    /// <remarks>
    /// <para>The cancellation token parameter is to be used if you want to cancel the progress bar and end it any time.</para>
    /// <para>It can also be used when you to display it while non-task actions are running, simply set the task to Task.Delay(-1) and cancel with the token when you want to</para>
    /// </remarks>
    [Pure]
    public static async Task<T> IndeterminateProgressBar<T>(Task<T> task, ConsoleColor color, string title, bool displayElapsedTime, int updateRate = 50, CancellationToken token = default) {
        try {
            if (task.Status is not TaskStatus.Running) {
                task.Start();
            }
        } catch {
            //ignore
        }

        NewLine(); // Break line before starting
        ogConsole.ResetColor();
        ogConsole.ForegroundColor = color;
        var startTime = Stopwatch.GetTimestamp();
        var lineNum = ogConsole.CursorTop;

        title = string.IsNullOrWhiteSpace(title) ? string.Empty : string.Concat(title, " ");

        while (!task.IsCompleted) { // Await until the TaskAwaiter informs of completion
            foreach (char c in Twirl) { // Cycle through the characters of twirl
                if (displayElapsedTime) {
                    var elapsed = Stopwatch.GetElapsedTime(startTime);
                    ogConsole.Write($"{title}{c} [Elapsed: {elapsed.ToFriendlyString()}]{ExtraBuffer}"); // Remove last character and re-write
                } else {
                    ogConsole.Write($"{title}{c}{ExtraBuffer}"); // Remove last character and re-write
                }
                ogConsole.SetCursorPosition(0, lineNum);
                await Task.Delay(updateRate, token); // The update rate
                ogConsole.Write(EmptyLine);
                ogConsole.SetCursorPosition(0, lineNum);
                if (token.IsCancellationRequested) {
                    ogConsole.Write(EmptyLine);
                    ogConsole.SetCursorPosition(0, lineNum);
                    return await task;
                }
            }
        }

        NewLine(); // Break line after completion

        ogConsole.ResetColor();

        return await task;
    }

    /// <summary>
    /// A simple twirl style indeterminate progress bar to signal the user that the app is not stuck but rather is performing a time consuming task.
    /// <para>
    /// The output is cleared so the next line will be written on the same line as the progress bar was.
    /// </para>
    /// </summary>
    /// <param name="task">The task you want to await on, it will not be modified, only the state is observed</param>
    /// <param name="color">The color in which to display the progress bar</param>
    /// <param name="title">Message to display alongside the progress bar</param>
    /// <param name="displayElapsedTime">Display elapsed time</param>
    /// <param name="updateRate">Rate at which the progress bar refreshes in milliseconds</param>
    /// <param name="token">So you can cancel the progress bar and end it any time</param>
    /// <remarks>
    /// <para>The cancellation token parameter is to be used if you want to cancel the progress bar and end it any time.</para>
    /// <para>It can also be used when you to display it while non-task actions are running, simply set the task to Task.Delay(-1) and cancel with the token when you want to</para>
    /// </remarks>
    public static async Task IndeterminateProgressBar(Task task, ConsoleColor color, string title, bool displayElapsedTime, int updateRate = 50, CancellationToken token = default) {
        var wrapped = Task.Run(async () => {
            await task;
            return 0;
        }, token);
        _ = await IndeterminateProgressBar(wrapped, color, title, displayElapsedTime, updateRate, token);
    }
}