using System.Runtime.InteropServices;

namespace PrettyConsole.UnitTests;

[SkipWhenConsoleUnavailable]
public partial class ConsoleContextWindowsAnsiTests {
    [Test]
    public async Task IsAnsiSupported_MatchesConsoleModeVirtualTerminalFlag() {
        if (!OperatingSystem.IsWindows()) {
            return;
        }

        bool expected = TryGetVirtualTerminalSupport(out bool isSupported) && isSupported;

        await Assert.That(ConsoleContext.IsAnsiSupported).IsEqualTo(expected);
    }

    private static bool TryGetVirtualTerminalSupport(out bool isSupported) {
        nint outputHandle = GetStdHandle(StdOutputHandle);
        if (outputHandle == 0 || outputHandle == InvalidHandleValue) {
            isSupported = false;
            return false;
        }

        if (!GetConsoleMode(outputHandle, out uint consoleMode)) {
            isSupported = false;
            return false;
        }

        isSupported = (consoleMode & EnableVirtualTerminalProcessing) != 0;
        return true;
    }

    private const int StdOutputHandle = -11;
    private const uint EnableVirtualTerminalProcessing = 0x0004;
    private static readonly nint InvalidHandleValue = -1;

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial nint GetStdHandle(int nStdHandle);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);
}
