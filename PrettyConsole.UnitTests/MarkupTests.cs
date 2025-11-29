namespace PrettyConsole.UnitTests;

public class MarkupTests {
    [Test]
    public async Task Markup_Enabled_MatchesConsoleRedirectionState() {
        await Assert.That(Markup.Underline.Equals("\e[4m", StringComparison.Ordinal)).IsTrue();
        await Assert.That(Markup.Reset.Equals("\e[0m", StringComparison.Ordinal)).IsTrue();
    }
}
