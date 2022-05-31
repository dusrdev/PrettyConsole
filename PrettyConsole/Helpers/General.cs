using System.Collections.Generic;

namespace PrettyConsole.Helpers {
    internal static class General {
        /// <summary>
        /// Returns the length of the longest string in the collection
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static int MaxStringLength(IEnumerable<string> col) {
            int max = 0;
            foreach (string value in col) {
                if (max < value.Length) {
                    max = value.Length;
                }
            }
            return max;
        }

        /// <summary>
        /// Returns a string completed with spaces to fill a certain length
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string SuffixWithSpaces(string str, int maxLength) {
            if (string.IsNullOrWhiteSpace(str) || str.Length > maxLength) {
                return new string(' ', maxLength);
            }
            return $"{str}{new string(' ', maxLength - str.Length)}";
        }
    }
}
