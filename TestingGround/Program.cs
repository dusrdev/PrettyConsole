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

            var c = Selection("Select the option you want", choices);

            WriteLine(c, Color.Highlight);
        }
    }
}