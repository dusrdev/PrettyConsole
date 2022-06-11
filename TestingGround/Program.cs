using static PrettyConsole.Console;

namespace TestingGround {
    internal class Program {
        static void Main() {
            SetColors(m => {
                m.Highlight = ConsoleColor.Green;
                m.Input = ConsoleColor.Magenta;
            });
        }
    }
}