using System.Runtime.CompilerServices;

namespace PrettyConsole;

public static partial class Console {
#pragma warning disable IDE0060 // Remove unused parameter
    /// <summary>
    /// Writes interpolated content using <see cref="PrettyConsoleInterpolatedStringHandler"/> to <see cref="OutputPipe.Out"/>.
    /// </summary>
    /// <param name="handler">Interpolated string handler that streams the content.</param>
    public static void Write([InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) {
        ResetColors();
	}

    /// <summary>
    /// Writes interpolated content using <see cref="PrettyConsoleInterpolatedStringHandler"/>.
    /// </summary>
    /// <param name="pipe">Destination pipe. Defaults to <see cref="OutputPipe.Out"/>.</param>
    /// <param name="handler">Interpolated string handler that streams the content.</param>
    public static void Write(OutputPipe pipe, [InterpolatedStringHandlerArgument(nameof(pipe))] PrettyConsoleInterpolatedStringHandler handler = default) {
        ResetColors();
    }

    /// <summary>
    /// Writes interpolated content using <see cref="PrettyConsoleInterpolatedStringHandler"/> to <see cref="OutputPipe.Out"/>.
    /// </summary>
    /// <param name="handler">Interpolated string handler that streams the content.</param>
    public static void WriteLine([InterpolatedStringHandlerArgument] PrettyConsoleInterpolatedStringHandler handler = default) {
        ResetColors();
        NewLine(OutputPipe.Out);
    }

    /// <summary>
    /// Writes interpolated content using <see cref="PrettyConsoleInterpolatedStringHandler"/>.
    /// </summary>
    /// <param name="pipe">Destination pipe. Defaults to <see cref="OutputPipe.Out"/>.</param>
    /// <param name="handler">Interpolated string handler that streams the content.</param>
    public static void WriteLine(OutputPipe pipe, [InterpolatedStringHandlerArgument(nameof(pipe))] PrettyConsoleInterpolatedStringHandler handler = default) {
        ResetColors();
        NewLine(pipe);
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
