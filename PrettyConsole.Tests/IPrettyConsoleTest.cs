using static PrettyConsole.Console;

namespace PrettyConsole.Tests;

public interface IPrettyConsoleTest {
    abstract string FeatureName { get; }

    abstract void Implementation();

    public void Render() {
        WriteLine("Test: ", $" {FeatureName} " & Color.Black | Color.White);
        NewLine();
        Implementation();
        NewLine();
        RequestAnyInput("Press any key to continue to next feature..." & Color.Green);
        NewLine();
    }
}