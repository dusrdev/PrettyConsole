namespace PrettyConsole.Tests.Unit;

public class Write {
    [Fact]
    public void Write_ColoredOutput_Single() {
        Out = Utilities.SetWriter(out var writer);
        Write("Hello world!" * Color.Green);
        writer.ToString().Should().Be("Hello world!");
    }
}