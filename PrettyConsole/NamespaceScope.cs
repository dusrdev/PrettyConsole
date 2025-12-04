namespace PrettyConsole;

/// <summary>
/// An action that set a local reference of <paramref name="value"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <returns></returns>
public delegate void RefAction<T>(out T value) where T : allows ref struct;