namespace PrettyConsole.UnitTests;

public class MarkupTests {
    [Test]
    [Arguments(Markup.Reset, "\e[0m")]
    [Arguments(Markup.Underline, "\e[4m")]
    [Arguments(Markup.ResetUnderline, "\e[24m")]
    [Arguments(Markup.Bold, "\e[1m")]
    [Arguments(Markup.ResetBold, "\e[22m")]
    [Arguments(Markup.Italic, "\e[3m")]
    [Arguments(Markup.ResetItalic, "\e[23m")]
    [Arguments(Markup.Strikethrough, "\e[9m")]
    [Arguments(Markup.ResetStrikethrough, "\e[29m")]
    public async Task Markup_Constants(string actual, string expected) {
        await Assert.That(actual).IsEqualTo(expected).IgnoringCase();
    }
}
