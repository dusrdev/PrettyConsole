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
        await Assert.That(sequence.Value).IsEqualTo("\e[39m");
        await Assert.That(sequence).IsSameReferenceAs(Color.DefaultForeground);
    }

    [Test]
    public async Task AnsiColors_DefaultBackground_UsesResetSequence() {
        var sequence = AnsiColors.Background((ConsoleColor)(-1));
        await Assert.That(sequence.Value).IsEqualTo("\e[49m");
        await Assert.That(sequence).IsSameReferenceAs(Color.DefaultBackground);
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
        await Assert.That(sequence.Value).IsEqualTo(expectedSequence);
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
        await Assert.That(sequence.Value).IsEqualTo(expectedSequence);
    }

    [Test]
    public async Task Color_BuiltInTokens_ExposeExpectedSequences() {
        await Assert.That(Color.Default.Value).IsEqualTo("\e[39m\e[49m");
        await Assert.That(Color.DefaultForeground.Value).IsEqualTo("\e[39m");
        await Assert.That(Color.DefaultBackground.Value).IsEqualTo("\e[49m");
        await Assert.That(Color.Green.Value).IsEqualTo("\e[92m");
        await Assert.That(Color.GreenBackground.Value).IsEqualTo("\e[102m");
    }

    [Test]
    public async Task Color_BuiltInTokens_AliasAnsiColorsCache() {
        await Assert.That(Color.Green).IsSameReferenceAs(AnsiColors.Foreground(Green));
        await Assert.That(Color.GreenBackground).IsSameReferenceAs(AnsiColors.Background(Green));
    }

    [Test]
    public async Task ConsoleColor_ImplicitlyConvertsToForegroundAnsiToken() {
        AnsiToken token = ConsoleColor.Green;
        await Assert.That(token).IsSameReferenceAs(Color.Green);
    }

    [Test]
    public async Task AnsiColors_IndexOrder_MatchesConsoleColorEnumOrder() {
        AnsiToken[] expectedForeground = [
            Color.Black,
            Color.DarkBlue,
            Color.DarkGreen,
            Color.DarkCyan,
            Color.DarkRed,
            Color.DarkMagenta,
            Color.DarkYellow,
            Color.Gray,
            Color.DarkGray,
            Color.Blue,
            Color.Green,
            Color.Cyan,
            Color.Red,
            Color.Magenta,
            Color.Yellow,
            Color.White
        ];

        AnsiToken[] expectedBackground = [
            Color.BlackBackground,
            Color.DarkBlueBackground,
            Color.DarkGreenBackground,
            Color.DarkCyanBackground,
            Color.DarkRedBackground,
            Color.DarkMagentaBackground,
            Color.DarkYellowBackground,
            Color.GrayBackground,
            Color.DarkGrayBackground,
            Color.BlueBackground,
            Color.GreenBackground,
            Color.CyanBackground,
            Color.RedBackground,
            Color.MagentaBackground,
            Color.YellowBackground,
            Color.WhiteBackground
        ];

        foreach (var color in Enum.GetValues<ConsoleColor>()) {
            int index = (int)color;
            await Assert.That(AnsiColors.Foreground(color)).IsSameReferenceAs(expectedForeground[index]);
            await Assert.That(AnsiColors.Background(color)).IsSameReferenceAs(expectedBackground[index]);
        }
    }
}
