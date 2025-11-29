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
}
