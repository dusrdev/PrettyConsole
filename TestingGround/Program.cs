using static PrettyConsole.Console;

namespace TestingGround {
    internal class Program {
        static void Main(string[] args) {
            List<string> choices = new List<string>() {
                "One", "Two", "Three"
            };

            var c = Selection("Select the option you want", choices);

            WriteLine(c, Color.Highlight);
        }
    }
}