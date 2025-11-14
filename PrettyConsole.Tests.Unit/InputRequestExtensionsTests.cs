namespace PrettyConsole.Tests.Unit;

public class InputRequestExtensionsTests {
    [Fact]
    public void RequestAnyInput_WritesPrompt_AndInvokesReadKey() {
        Out = Utilities.GetWriter(out var writer);
        bool invoked = false;

        try {
            InputRequestExtensions.ConfigureReadKey(() => {
                invoked = true;
                return new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false);
            });

            Console.RequestAnyInput($"Press something:");

            Assert.Contains("Press something:", writer.ToString());
            Assert.True(invoked);
        } finally {
			InputRequestExtensions.ConfigureReadKey(null);
		}
    }
}
