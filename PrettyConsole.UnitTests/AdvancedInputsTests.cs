namespace PrettyConsole.UnitTests;

public class AdvancedInputsTests {
    [Test]
    public async Task Confirm_Case_Y_Interpolated() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("y");
        In = reader;
        var res = Console.Confirm($"Enter y:");
        await Assert.That(stringWriter.ToString()).Contains("Enter y:");
        await Assert.That(res).IsTrue();
    }

    [Test]
    public async Task Confirm_Case_Yes_Interpolated() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("yes");
        In = reader;
        var res = Console.Confirm($"Enter yes:");
        await Assert.That(stringWriter.ToString()).Contains("Enter yes");
        await Assert.That(res).IsTrue();
    }

    [Test]
    public async Task Confirm_Case_Empty_Interpolated() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("");
        In = reader;
        var res = Console.Confirm($"Enter yes:");
        await Assert.That(stringWriter.ToString()).Contains("Enter yes");
        await Assert.That(res).IsTrue();
    }

    [Test]
    public async Task Confirm_Case_No_Interpolated() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("no");
        In = reader;
        var res = Console.Confirm($"Enter no:");
        await Assert.That(stringWriter.ToString()).Contains("Enter no");
        await Assert.That(res).IsFalse();
    }

    [Test]
    public async Task Confirm_CustomTrueValues_WithInterpolatedPrompt() {
        Out = Utilities.GetWriter(out var stringWriter);
        var reader = Utilities.GetReader("ok");
        In = reader;

        var res = Console.Confirm(["ok", "okay"], $"Proceed?", false);

        await Assert.That(stringWriter.ToStringAndFlush()).IsEqualTo("Proceed?");
        await Assert.That(res).IsTrue();
    }
}