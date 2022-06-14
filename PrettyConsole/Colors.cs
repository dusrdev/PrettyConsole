using System;

namespace PrettyConsole {
    public class Colors {
        public ConsoleColor Primary { get; set; } = ConsoleColor.White;
        public ConsoleColor Default { get; set; } = ConsoleColor.Gray;
        public ConsoleColor Input { get; set; } = ConsoleColor.Gray;
        public ConsoleColor Success { get; set; } = ConsoleColor.Green;
        public ConsoleColor Error { get; set; } = ConsoleColor.Red;
        public ConsoleColor Highlight { get; set; } = ConsoleColor.Blue;
    }
}
