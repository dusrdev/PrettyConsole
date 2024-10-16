using static PrettyConsole.Console;

namespace PrettyConsole.Tests;

public interface IPrettyConsoleTest {
    string FeatureName { get; }

    ValueTask Implementation();

    public async ValueTask Render() {
        WriteLine(["Test: ", FeatureName * Color.Black / Color.White]);
        NewLine();
        await Implementation();
        NewLine();
        RequestAnyInput(["Press any key to continue to next feature..." * Color.Green]);
        NewLine();
    }
}