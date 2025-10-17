using System.Runtime.CompilerServices;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Writes interpolated content using <see cref="PrettyConsoleInterpolatedStringHandler"/>.
    /// </summary>
    /// <param name="pipe">Destination pipe. Defaults to <see cref="OutputPipe.Out"/>.</param>
    /// <param name="handler">Interpolated string handler that streams the content.</param>
    public static void WriteInterpolated(OutputPipe pipe, [InterpolatedStringHandlerArgument(nameof(pipe))] PrettyConsoleInterpolatedStringHandler handler = default) {
        _ = pipe;
        _ = handler;
    }

    /// <summary>
    /// Writes interpolated content using <see cref="PrettyConsoleInterpolatedStringHandler"/>.
    /// </summary>
    /// <param name="pipe">Destination pipe. Defaults to <see cref="OutputPipe.Out"/>.</param>
    /// <param name="handler">Interpolated string handler that streams the content.</param>
    public static void WriteLineInterpolated(OutputPipe pipe, [InterpolatedStringHandlerArgument(nameof(pipe))] PrettyConsoleInterpolatedStringHandler handler = default) {
        _ = pipe;
        _ = handler;
        NewLine(pipe);
    }
}
