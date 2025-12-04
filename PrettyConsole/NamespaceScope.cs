namespace PrettyConsole;

/// <summary>
/// A factory function that returns a reference to a PrettyConsoleInterpolatedStringHandler
/// </summary>
/// <param name="builder"></param>
/// <param name="handler"></param>
/// <returns></returns>
public delegate void PrettyConsoleInterpolatedStringHandlerFactory(PrettyConsoleInterpolatedStringHandlerBuilder builder, out PrettyConsoleInterpolatedStringHandler handler);