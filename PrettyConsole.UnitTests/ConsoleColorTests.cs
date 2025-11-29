namespace PrettyConsole.UnitTests;

public class ConsoleColorTests {
    [Test]
    public async Task ConsoleColor_DivideOperator() {
        var (fg, bg) = Red / Blue;
        await Assert.That(fg).IsEqualTo(Red);
        await Assert.That(bg).IsEqualTo(Blue);
    }

    [Test]
    public async Task ConsoleColor_DivideTupleOperator() {
        var (fg, bg) = Red / (Blue / Green);
        await Assert.That(fg).IsEqualTo(Red);
        await Assert.That(bg).IsEqualTo(Green);
    }

    [Test]
    public async Task ConsoleColor_DefaultColors() {
        var (fg, bg) = ConsoleColor.Default;
        await Assert.That(fg).IsEqualTo(ConsoleColor.DefaultForeground);
        await Assert.That(bg).IsEqualTo(ConsoleColor.DefaultBackground);
    }

    [Test]
    public async Task AnsiColors_DefaultForeground_UsesResetSequence() {
        var sequence = AnsiColors.Foreground((ConsoleColor)(-1));
        await Assert.That(sequence).IsEqualTo("\e[39m");
    }

    [Test]
    public async Task AnsiColors_DefaultBackground_UsesResetSequence() {
        var sequence = AnsiColors.Background((ConsoleColor)(-1));
        await Assert.That(sequence).IsEqualTo("\e[49m");
    }

    [Test]
    [Arguments(Black, "\e[30m")]
    [Arguments(DarkBlue, "\e[34m")]
    [Arguments(DarkGreen, "\e[32m")]
    [Arguments(DarkCyan, "\e[36m")]
    [Arguments(DarkRed, "\e[31m")]
    [Arguments(DarkMagenta, "\e[35m")]
    [Arguments(DarkYellow, "\e[33m")]
    [Arguments(Gray, "\e[37m")]
    [Arguments(DarkGray, "\e[90m")]
    [Arguments(Blue, "\e[94m")]
    [Arguments(Green, "\e[92m")]
    [Arguments(Cyan, "\e[96m")]
    [Arguments(Red, "\e[91m")]
    [Arguments(Magenta, "\e[95m")]
    [Arguments(Yellow, "\e[93m")]
    [Arguments(White, "\e[97m")]
    public async Task AnsiColors_ForegroundSequences(ConsoleColor color, string expectedSequence) {
        var sequence = AnsiColors.Foreground(color);
        await Assert.That(sequence).IsEqualTo(expectedSequence);
    }

    [Test]
    [Arguments(Black, "\e[40m")]
    [Arguments(DarkBlue, "\e[44m")]
    [Arguments(DarkGreen, "\e[42m")]
    [Arguments(DarkCyan, "\e[46m")]
    [Arguments(DarkRed, "\e[41m")]
    [Arguments(DarkMagenta, "\e[45m")]
    [Arguments(DarkYellow, "\e[43m")]
    [Arguments(Gray, "\e[47m")]
    [Arguments(DarkGray, "\e[100m")]
    [Arguments(Blue, "\e[104m")]
    [Arguments(Green, "\e[102m")]
    [Arguments(Cyan, "\e[106m")]
    [Arguments(Red, "\e[101m")]
    [Arguments(Magenta, "\e[105m")]
    [Arguments(Yellow, "\e[103m")]
    [Arguments(White, "\e[107m")]
    public async Task AnsiColors_BackgroundSequences(ConsoleColor color, string expectedSequence) {
        var sequence = AnsiColors.Background(color);
        await Assert.That(sequence).IsEqualTo(expectedSequence);
    }

    [Test]
    public async Task AnsiColors_InternalBuilders_MatchPublicAccessors() {
        foreach (var color in Enum.GetValues<ConsoleColor>()) {
            var fgBuilt = AnsiColors.BuildForegroundSequence(color);
            var bgBuilt = AnsiColors.BuildBackgroundSequence(color);

            await Assert.That(AnsiColors.Foreground(color)).IsEqualTo(fgBuilt);
            await Assert.That(AnsiColors.Background(color)).IsEqualTo(bgBuilt);
        }
    }
}
