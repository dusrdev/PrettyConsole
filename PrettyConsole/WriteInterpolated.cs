using System.Runtime.CompilerServices;

namespace PrettyConsole;

public static partial class Console {
#pragma warning disable IDE0060 // Remove unused parameter
    /// <summary>
    /// Writes interpolated content using <see cref="PrettyConsoleInterpolatedStringHandler"/>.
    /// </summary>
    /// <param name="pipe">Destination pipe. Defaults to <see cref="OutputPipe.Out"/>.</param>
    /// <param name="handler">Interpolated string handler that streams the content.</param>
    public static void WriteInterpolated(OutputPipe pipe, [InterpolatedStringHandlerArgument(nameof(pipe))] PrettyConsoleInterpolatedStringHandler handler = default) {
    }

    /// <summary>
    /// Writes interpolated content using <see cref="PrettyConsoleInterpolatedStringHandler"/>.
    /// </summary>
    /// <param name="pipe">Destination pipe. Defaults to <see cref="OutputPipe.Out"/>.</param>
    /// <param name="handler">Interpolated string handler that streams the content.</param>

    public static void WriteLineInterpolated(OutputPipe pipe, [InterpolatedStringHandlerArgument(nameof(pipe))] PrettyConsoleInterpolatedStringHandler handler = default) {
        NewLine(pipe);
    }
#pragma warning restore IDE0060 // Remove unused parameter

}
