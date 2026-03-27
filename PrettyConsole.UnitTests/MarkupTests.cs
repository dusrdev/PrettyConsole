namespace PrettyConsole.UnitTests;

public class MarkupTests {
    [Test]
    [Arguments("\e[0m")]
    [Arguments("\e[4m")]
    [Arguments("\e[24m")]
    [Arguments("\e[1m")]
    [Arguments("\e[22m")]
    [Arguments("\e[3m")]
    [Arguments("\e[23m")]
    [Arguments("\e[9m")]
    [Arguments("\e[29m")]
    public async Task Markup_BuiltInTokens_ExposeExpectedSequences(string expected) {
        AnsiToken actual = expected switch {
            "\e[0m" => Markup.Reset,
            "\e[4m" => Markup.Underline,
            "\e[24m" => Markup.ResetUnderline,
            "\e[1m" => Markup.Bold,
            "\e[22m" => Markup.ResetBold,
            "\e[3m" => Markup.Italic,
            "\e[23m" => Markup.ResetItalic,
            "\e[9m" => Markup.Strikethrough,
            "\e[29m" => Markup.ResetStrikethrough,
            _ => throw new InvalidOperationException("Unexpected markup token test case.")
        };

        await Assert.That(actual.Value).IsEqualTo(expected).IgnoringCase();
    }

    [Test]
    public async Task AnsiToken_CustomValue_UsesProvidedSequence() {
        var token = new AnsiToken("\e[5m");

        await Assert.That(token.Value).IsEqualTo("\e[5m");
    }
}
