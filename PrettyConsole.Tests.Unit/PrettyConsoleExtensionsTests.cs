namespace PrettyConsole.Tests.Unit;

public class PrettyConsoleExtensionsTests {
    [Fact]
    public void WriteWhiteSpaces_WritesRequestedLength() {
        var writer = new StringWriter();

        writer.WriteWhiteSpaces(10);
        Assert.Equal(new string(' ', 10), writer.ToString());

        writer.GetStringBuilder().Clear();
        writer.WriteWhiteSpaces(300);
        Assert.Equal(300, writer.ToString().Length);
        Assert.True(writer.ToString().All(c => c == ' '));
    }
}