using PrettyConsole.Models;

using static PrettyConsole.Console;

namespace PrettyConsole.Tests;

public interface IPrettyConsoleTest {
    abstract string FeatureName { get; }

    abstract void Implementation();

    public void Render() {
        WriteLine("Test: ", FeatureName & Color.Black | Color.Cyan);
        NewLine();
        Implementation();
    }
}