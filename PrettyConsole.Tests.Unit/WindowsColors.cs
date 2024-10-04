using static PrettyConsole.Color;

namespace PrettyConsole.Tests.Unit;

#if OS_WINDOWS
public class WindowsColors {
    [Fact]
    public void SetColorsToDefaults() {
        Kernel32.CONSOLE_SCREEN_BUFFER_INFO csbi = Kernel32.GetBufferInfo(false, out bool succeeded);

        var foreground = ColorAttributeToConsoleColor((Kernel32.Color)csbi.wAttributes & Kernel32.Color.ForegroundMask);
        var background = ColorAttributeToConsoleColor((Kernel32.Color)csbi.wAttributes & Kernel32.Color.
            BackgroundMask);

        succeeded.Should().BeTrue();
        foreground.Should().Be(ConsoleColor.Gray);
        background.Should().Be(ConsoleColor.Black);
    }
}
#endif