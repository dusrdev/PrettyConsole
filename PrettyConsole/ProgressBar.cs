using System.Runtime.CompilerServices;

using PrettyConsole.Models;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Outputs progress bar filled according to <paramref name="percent"/>
    /// <para>
    /// When called consecutively, it overrides the previous
    /// </para>
    /// </summary>
    /// <param name="percent"></param>
    public static void UpdateProgressBar(int percent) => UpdateProgressBar(percent, Color.Default);

    /// <summary>
    /// Outputs progress bar filled according to <paramref name="percent"/>
    /// <para>
    /// When called consecutively, it overrides the previous
    /// </para>
    /// </summary>
    /// <param name="percent"></param>
    /// <param name="color">The color you want the progress bar to be</param>
    public static void UpdateProgressBar(int percent, ConsoleColor color)
     => UpdateProgressBar(percent, color, color);

    /// <summary>
    /// Outputs progress bar filled according to <paramref name="percent"/>
    /// <para>
    /// When called consecutively, it overrides the previous
    /// </para>
    /// </summary>
    /// <param name="percent"></param>
    /// <param name="foreground">color of the bounds and percentage</param>
    /// <param name="progress">color of the progress bar fill</param>
    public static void UpdateProgressBar(int percent, ConsoleColor foreground, ConsoleColor progress)
    => UpdateProgressBar(new ProgressBarDisplay(percent, foreground, progress));

    private static readonly char[] ProgressBarBuffer = GC.AllocateUninitializedArray<char>(ProgressBarSize);

    /// <summary>
    /// Outputs progress bar filled according to <paramref name="display"/>
    /// <para>
    /// When called consecutively, it overrides the previous
    /// </para>
    /// <para>Make sure to print a new line after the last call, otherwise your outputs will override the progress bar</para>
    /// </summary>
    /// <remarks>
    /// <para>Test before usage in release, when updated too quickly, the progress bar may fail to override previous lines and will make a mess</para>
    /// <para>If that happens, consider restricting the updates yourself by wrapping the call</para>
    /// <para>If the progress bar is interrupted, you clear the used lines with ClearNextLines()</para>
    /// </remarks>
    /// <param name="display"></param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void UpdateProgressBar(ProgressBarDisplay display) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = display.Foreground;
            var currentLine = ogConsole.CursorTop;
            ogConsole.SetCursorPosition(0, currentLine);
            ogConsole.WriteLine(EmptyLine);
            ogConsole.WriteLine(EmptyLine);
            ogConsole.SetCursorPosition(0, currentLine);
            if (display.Header.Length is not 0) {
                ogConsole.Out.WriteLine(display.Header);
            }
            ogConsole.Out.Write("[");
            var p = (int)(ProgressBarSize * display.Percentage * 0.01);
            ogConsole.ForegroundColor = display.Progress;
            Span<char> span = ProgressBarBuffer;
            Span<char> full = span[..p];
            full.Fill(display.ProgressChar);
            Span<char> empty = span[p..];
            empty.Fill(' ');
            ogConsole.Out.Write(full);
            ogConsole.Out.Write(empty);
            ogConsole.ForegroundColor = display.Foreground;
            ogConsole.Out.Write($"] {display.Percentage,5:##0.##}%");
            ogConsole.SetCursorPosition(0, currentLine);
        } finally {
            ogConsole.ResetColor();
        }
    }
}