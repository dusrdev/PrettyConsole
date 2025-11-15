namespace PrettyConsole.Tests.Unit;

public class MarkupTests {
    [Fact]
    public void Markup_Enabled_MatchesConsoleRedirectionState() {
        var expectedEnabled = !Console.IsOutputRedirected && !Console.IsErrorRedirected;
        Assert.Equal(expectedEnabled, Markup.Enabled);
        if (expectedEnabled) {
            Assert.Equal("\u001b[4m", Markup.Underline);
            Assert.Equal("\u001b[0m", Markup.Reset);
        } else {
            Assert.Equal(string.Empty, Markup.Underline);
            Assert.Equal(string.Empty, Markup.Reset);
        }
    }
}