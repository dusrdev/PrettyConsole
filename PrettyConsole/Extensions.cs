using System;
using System.Collections.Generic;
using System.Text;

namespace PrettyConsole {
    internal static class Extensions {
        // Returns the length of the longest string in the collection
        public static int MaxStringLength(IEnumerable<string> col) {
            int max = 0;
            foreach (var value in col) {
                if (max < value.Length) {
                    max = value.Length;
                }
            }
            return max;
        }

        // Returns a string completed with spaces to fill a certain length
        public static string SuffixWithSpaces(ReadOnlySpan<char> str, int maxLength) {
            if (IsEmptyOrWhiteSpace(str) || str.Length > maxLength) {
                return new string(' ', maxLength);
            }
            return $"{str}{new string(' ', maxLength - str.Length)}";
        }

        // Checks whether the ReadOnlySpan is empty or only contains whitespace characters
        public static bool IsEmptyOrWhiteSpace(ReadOnlySpan<char> str) => str.IsEmpty || str.IsWhiteSpace();

        // A faster and more memory efficient implementation of string.split
        public static IEnumerable<string> SplitAsSpan(ReadOnlySpan<char> str, char seperator) {
            StringBuilder builder = new();
            List<string> output = new();
            foreach (var c in str) {
                if (c.Equals(seperator)) {
                    if (builder.Length > 0) {
                        output.Add(builder.ToString().Trim());
                        builder.Clear();
                    }
                } else {
                    builder.Append(c);
                }
            }
            if (builder.Length > 0) {
                output.Add(builder.ToString().Trim());
            }
            return output;
        }

        // Returns a user friendly representation
        public static string ToFriendlyString(this TimeSpan elapsed) {
            string time;
            if (elapsed.TotalSeconds < 60) {
                time = $"{elapsed.TotalSeconds:0.##}s";
            } else if (elapsed.Minutes < 60) {
                time = $"{elapsed.TotalMinutes:0.##}m";
            } else {
                time = $"{elapsed.TotalHours:0.###}h";
            }
            return time;
        }
    }
}
