using System.Collections.Generic;

namespace PrettyConsole.Helpers {
    internal static class General {
        public static int MaxStringLength(IEnumerable<string> col) {
            int max = 0;
            foreach (string value in col) {
                if (max < value.Length) {
                    max = value.Length;
                }
            }
            return max;
        }

        public static string SuffixWithSpaces(string str, int maxLength) {
            if (string.IsNullOrWhiteSpace(str) || str.Length > maxLength) {
                return new string(' ', maxLength);
            }
            return $"{str}{new string(' ', maxLength - str.Length)}";
        }
    }
}
