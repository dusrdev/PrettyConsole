namespace PrettyConsole.Tests.Unit;

public class AdvancedOutputs {
    [Fact]
    public async Task TypeWrite_Regular() {
        Out = Utilities.GetWriter(out var stringWriter);
        await TypeWrite("Hello world!" * Color.Green, 10);
        stringWriter.ToString().Should().Contain("Hello world!");
    }

    [Fact]
    public async Task TypeWriteLine_Regular() {
        Out = Utilities.GetWriter(out var stringWriter);
        await TypeWriteLine("Hello world!" * Color.Green, 10);
        stringWriter.ToString().Should().Contain("Hello world!" + Environment.NewLine);
    }
}