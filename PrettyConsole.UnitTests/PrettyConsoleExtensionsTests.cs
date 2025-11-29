namespace PrettyConsole.UnitTests;

public class PrettyConsoleExtensionsTests {
    [Test]
    public async Task WriteWhiteSpaces_WritesRequestedLength() {
        var writer = new StringWriter();

        writer.WriteWhiteSpaces(10);
        await Assert.That(writer.ToString()).IsEqualTo(new string(' ', 10));

        writer.GetStringBuilder().Clear();
        writer.WriteWhiteSpaces(300);
        await Assert.That(writer.ToString().Length).IsEqualTo(300);
        await Assert.That(writer.ToString().All(c => c == ' ')).IsTrue();
    }
}
