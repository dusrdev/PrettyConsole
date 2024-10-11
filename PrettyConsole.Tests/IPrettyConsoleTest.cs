using static PrettyConsole.Console;

namespace PrettyConsole.Tests;

public interface IPrettyConsoleTest {
    string FeatureName { get; }

    void Implementation();

    public void Render() {
        WriteLine(["Test: ", FeatureName * Color.Black / Color.White]);
        NewLine();
        Implementation();
        NewLine();
        RequestAnyInput(["Press any key to continue to next feature..." * Color.Green]); 
        NewLine();
    }
}