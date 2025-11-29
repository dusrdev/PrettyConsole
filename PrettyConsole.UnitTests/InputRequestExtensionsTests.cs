namespace PrettyConsole.UnitTests;

public class InputRequestExtensionsTests {
    [Test]
    public async Task RequestAnyInput_WritesPrompt_AndInvokesReadKey() {
        Out = Utilities.GetWriter(out var writer);
        bool invoked = false;

        try {
            InputRequestExtensions.ConfigureReadKey(() => {
                invoked = true;
                return new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false);
            });

            Console.RequestAnyInput($"Press something:");

            await Assert.That(writer.ToString()).Contains("Press something:");
            await Assert.That(invoked).IsTrue();
        } finally {
            InputRequestExtensions.ConfigureReadKey(null);
        }
    }
}
