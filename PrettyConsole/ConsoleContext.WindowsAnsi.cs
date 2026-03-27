using System.Runtime.InteropServices;

namespace PrettyConsole;

public static partial class ConsoleContext {
#if WINDOWS
    /// <summary>
    /// Holds a value that checks whether ANSI is supported in the current console (WINDOWS ONLY)
    /// </summary>
    public static readonly bool IsAnsiSupported = GetIsAnsiSupported();

    private static bool GetIsAnsiSupported() {
        nint outputHandle = GetStdHandle(StdOutputHandle);
        if (outputHandle == 0 || outputHandle == InvalidHandleValue) {
            return false;
        }

        return GetConsoleMode(outputHandle, out uint consoleMode)
            && (consoleMode & EnableVirtualTerminalProcessing) != 0;
    }
    private const int StdOutputHandle = -11;
    private const uint EnableVirtualTerminalProcessing = 0x0004;
    private static readonly nint InvalidHandleValue = -1;

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial nint GetStdHandle(int nStdHandle);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);
#endif
}
