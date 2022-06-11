using static PrettyConsole.Console;

namespace TestingGround {
    internal class Program {
        static void Main() {
            SetColors(m => {
                m.Highlight = ConsoleColor.Green;
                m.Input = ConsoleColor.Magenta;
            });

            List<string> choices = new() {
                "One", "Two", "Three"
            };

            Dictionary<string, List<string>> tree = new() {
                { "first", new() { "one", "two" } },
                { "second", new() { "three", "four" } },
            };

            var results = TreeMenu("Select the option you want", tree);

            WriteLine($"{results.option} - {results.subOption}");
        }
    }
}