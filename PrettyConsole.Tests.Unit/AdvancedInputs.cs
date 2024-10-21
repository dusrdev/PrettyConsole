namespace PrettyConsole.Tests.Unit;

public class AdvancedInputs {
    [Fact]
    public void Confirm_Case_Y() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("y");
        In = reader;
        var res = Confirm(["Enter y" * Color.White]);
        stringWriter.ToString().Should().Contain("Enter y");
        res.Should().BeTrue();
    }

    [Fact]
    public void Confirm_Case_Yes() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("yes");
        In = reader;
        var res = Confirm(["Enter yes" * Color.White]);
        stringWriter.ToString().Should().Contain("Enter yes");
        res.Should().BeTrue();
    }

    [Fact]
    public void Confirm_Case_Empty() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("");
        In = reader;
        var res = Confirm(["Enter yes" * Color.White]);
        stringWriter.ToString().Should().Contain("Enter yes");
        res.Should().BeTrue();
    }

    [Fact]
    public void Confirm_Case_No() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("no");
        In = reader;
        var res = Confirm(["Enter no" * Color.White]);
        stringWriter.ToString().Should().Contain("Enter no");
        res.Should().BeFalse();
    }
}