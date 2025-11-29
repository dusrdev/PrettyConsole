namespace PrettyConsole.Tests.Unit;

public class MarkupTests {
    [Fact]
    public void Markup_Enabled_MatchesConsoleRedirectionState() {
        Assert.Equal("\e[4m", Markup.Underline);
        Assert.Equal("\e[0m", Markup.Reset);
    }
}