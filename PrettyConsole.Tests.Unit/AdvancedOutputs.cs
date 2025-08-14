namespace PrettyConsole.Tests.Unit;

public class AdvancedOutputs {
    [Fact]
    public async Task TypeWrite_Regular() {
        Out = Utilities.GetWriter(out var stringWriter);
        await TypeWrite("Hello world!" * Color.Green, 10);
        Assert.Contains("Hello world!", stringWriter.ToString());
    }

    [Fact]
    public async Task TypeWriteLine_Regular() {
        Out = Utilities.GetWriter(out var stringWriter);
        await TypeWriteLine("Hello world!" * Color.Green, 10);
        Assert.Contains("Hello world!" + Environment.NewLine, stringWriter.ToString());
    }
}