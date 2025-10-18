namespace PrettyConsole.Tests.Unit;

public class AdvancedInputs {
    [Fact]
    public void Confirm_Case_Y() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("y");
        In = reader;
        var res = Confirm(["Enter y" * Color.White]);
        Assert.Contains("Enter y", stringWriter.ToString());
        Assert.True(res);
    }

    [Fact]
    public void Confirm_Case_Yes() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("yes");
        In = reader;
        var res = Confirm(["Enter yes" * Color.White]);
        Assert.Contains("Enter yes", stringWriter.ToString());
        Assert.True(res);
    }

    [Fact]
    public void Confirm_Case_Empty() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("");
        In = reader;
        var res = Confirm(["Enter yes" * Color.White]);
        Assert.Contains("Enter yes", stringWriter.ToString());
        Assert.True(res);
    }

    [Fact]
    public void Confirm_Case_No() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("no");
        In = reader;
        var res = Confirm(["Enter no" * Color.White]);
        Assert.Contains("Enter no", stringWriter.ToString());
        Assert.False(res);
    }

    [Fact]
    public void Confirm_Case_Y_Interpolated() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("y");
        In = reader;
        var res = Confirm($"Enter y:");
        Assert.Contains("Enter y:", stringWriter.ToString());
        Assert.True(res);
    }

    [Fact]
    public void Confirm_Case_Yes_Interpolated() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("yes");
        In = reader;
        var res = Confirm($"Enter yes:");
        Assert.Contains("Enter yes", stringWriter.ToString());
        Assert.True(res);
    }

    [Fact]
    public void Confirm_Case_Empty_Interpolated() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("");
        In = reader;
        var res = Confirm($"Enter yes:");
        Assert.Contains("Enter yes", stringWriter.ToString());
        Assert.True(res);
    }

    [Fact]
    public void Confirm_Case_No_Interpolated() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("no");
        In = reader;
        var res = Confirm($"Enter no:");
        Assert.Contains("Enter no", stringWriter.ToString());
        Assert.False(res);
    }
}