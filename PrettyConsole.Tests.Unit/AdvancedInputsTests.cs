namespace PrettyConsole.Tests.Unit;

public class AdvancedInputsTests {
    [Fact]
    public void Confirm_Case_Y_Interpolated() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("y");
        In = reader;
        var res = Console.Confirm($"Enter y:");
        Assert.Contains("Enter y:", stringWriter.ToString());
        Assert.True(res);
    }

    [Fact]
    public void Confirm_Case_Yes_Interpolated() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("yes");
        In = reader;
        var res = Console.Confirm($"Enter yes:");
        Assert.Contains("Enter yes", stringWriter.ToString());
        Assert.True(res);
    }

    [Fact]
    public void Confirm_Case_Empty_Interpolated() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("");
        In = reader;
        var res = Console.Confirm($"Enter yes:");
        Assert.Contains("Enter yes", stringWriter.ToString());
        Assert.True(res);
    }

    [Fact]
    public void Confirm_Case_No_Interpolated() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("no");
        In = reader;
        var res = Console.Confirm($"Enter no:");
        Assert.Contains("Enter no", stringWriter.ToString());
        Assert.False(res);
    }

    [Fact]
    public void Confirm_CustomTrueValues_WithInterpolatedPrompt() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("ok");
        In = reader;

        var res = Console.Confirm(["ok", "okay"], false, $"Proceed?");

        Assert.Equal("Proceed?", stringWriter.ToStringAndFlush());
        Assert.True(res);
    }
}