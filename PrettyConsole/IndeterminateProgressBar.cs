using System.Diagnostics;

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
    /// <param name="displayElapsedTime">Display elapsed time</param>
    /// <param name="updateRate">Rate at which the progress bar refreshes in milliseconds</param>
    /// <param name="token">So you can cancel the progress bar and end it any time</param>
    /// <remarks>
    /// <para>The cancellation token parameter is to be used if you want to cancel the progress bar and end it any time.</para>
    /// <para>It can also be used when you to display it while non-task actions are running, simply set the task to Task.Delay(-1) and cancel with the token when you want to</para>
    /// </remarks>
    public static async Task<T> IndeterminateProgressBar<T>(Task<T> task, ConsoleColor color, bool displayElapsedTime,
        int updateRate = 50, CancellationToken token = default) {
        await IndeterminateProgressBar(task, color, displayElapsedTime, updateRate, token);

        return task.IsCompleted ? task.Result : await task;
    }

    /// <summary>
    /// A simple twirl style indeterminate progress bar to signal the user that the app is not stuck but rather is performing a time consuming task.
    /// <para>
    /// The output is cleared so the next line will be written on the same line as the progress bar was.
    /// </para>
    /// </summary>
    /// <param name="task">The task you want to await on, it will not be modified, only the state is observed</param>
    /// <param name="color">The color in which to display the progress bar</param>
    /// <param name="displayElapsedTime">Display elapsed time</param>
    /// <param name="updateRate">Rate at which the progress bar refreshes in milliseconds</param>
    /// <param name="token">So you can cancel the progress bar and end it any time</param>
    /// <remarks>
    /// <para>The cancellation token parameter is to be used if you want to cancel the progress bar and end it any time.</para>
    /// <para>It can also be used when you to display it while non-task actions are running, simply set the task to Task.Delay(-1) and cancel with the token when you want to</para>
    /// </remarks>
    public static async Task IndeterminateProgressBar(Task task, ConsoleColor color, bool displayElapsedTime,
        int updateRate = 50, CancellationToken token = default) {
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
                ogConsole.ForegroundColor = color;
                ogConsole.Write(c);
                ogConsole.ForegroundColor = originalColor;
                if (displayElapsedTime) {
                    var elapsed = Stopwatch.GetElapsedTime(startTime);
                    ogConsole.Write(" [Elapsed: ");
                    ogConsole.Write(elapsed.ToFriendlyString());
                    ogConsole.Write(']');
                }

                ogConsole.Write(ExtraBuffer);

                ogConsole.SetCursorPosition(0, lineNum);
                await Task.Delay(updateRate, token); // The update rate
                ogConsole.Write(EmptyLine);
                ogConsole.SetCursorPosition(0, lineNum);
                if (token.IsCancellationRequested) {
                    return;
                }
            }
        }

        ResetColors();
    }
}