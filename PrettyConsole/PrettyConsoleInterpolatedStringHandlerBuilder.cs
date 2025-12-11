namespace PrettyConsole;

#pragma warning disable CA1822 // Mark members as static
/// <summary>
/// Provides an API to build a string handler.
/// </summary>
public sealed class PrettyConsoleInterpolatedStringHandlerBuilder {
    /// <summary>
    /// A singleton instance of <see cref="PrettyConsoleInterpolatedStringHandlerBuilder"/>.
    /// </summary>
    /// <remarks>This instance is stateless and thread-safe.</remarks>
    public static readonly PrettyConsoleInterpolatedStringHandlerBuilder Singleton = new();

    /// <summary>
    /// Builds a <see cref="PrettyConsoleInterpolatedStringHandler"/> and returns its reference.
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    public ref PrettyConsoleInterpolatedStringHandler Build([InterpolatedStringHandlerArgument] ref PrettyConsoleInterpolatedStringHandler handler) => ref handler;

    /// <summary>
    /// Builds a <see cref="PrettyConsoleInterpolatedStringHandler"/> and returns its reference.
    /// </summary>
    /// <param name="pipe"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public ref PrettyConsoleInterpolatedStringHandler Build(OutputPipe pipe, [InterpolatedStringHandlerArgument(nameof(pipe))] ref PrettyConsoleInterpolatedStringHandler handler) => ref handler;
}
#pragma warning restore CA1822 // Mark members as static
