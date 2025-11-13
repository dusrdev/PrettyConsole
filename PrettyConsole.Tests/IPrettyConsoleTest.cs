using static System.ConsoleColor;

namespace PrettyConsole.Tests;

public interface IPrettyConsoleTest {
    string FeatureName { get; }

    ValueTask Implementation();

    public async ValueTask Render() {
        Console.WriteLineInterpolated($"Test: {Black / White}{FeatureName}");
        Console.NewLine();
        await Implementation();
        Console.NewLine();
        Console.RequestAnyInput($"{Green}Press any key to continue to next feature...");
        Console.NewLine();
    }
}