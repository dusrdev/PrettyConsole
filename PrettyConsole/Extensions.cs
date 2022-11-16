using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PrettyConsole {
    internal static class Extensions {
        // Returns a string completed with spaces to fill a certain length
        public static string SuffixWithSpaces(ReadOnlySpan<char> str, int maxLength) {
            if (IsEmptyOrWhiteSpace(str) || str.Length > maxLength) {
                return new string(' ', maxLength);
            }
            return $"{str}{new string(' ', maxLength - str.Length)}";
        }

        // Checks whether the ReadOnlySpan is empty or only contains whitespace characters
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmptyOrWhiteSpace(ReadOnlySpan<char> str) => str.IsEmpty || str.IsWhiteSpace();

        // A faster and more memory efficient implementation of string.split
        public static IEnumerable<string> Split(string str, char seperator) {
            StringBuilder builder = new();
            foreach (var c in str) {
                if (c.Equals(seperator)) {
                    if (builder.Length <= 0) {
                        continue;
                    }

                    yield return builder.ToString().Trim();
                    builder.Clear();
                    continue;
                }

                builder.Append(c);
            }
            if (builder.Length > 0) {
                yield return builder.ToString().Trim();
            }
            yield return string.Empty;
        }

        // Returns a user friendly representation
        public static string ToFriendlyString(this TimeSpan elapsed) {
            if (elapsed.TotalSeconds < 60) {
                return $"{elapsed.TotalSeconds:0.##}s";
            }
            if (elapsed.Minutes < 60) {
                return $"{elapsed.TotalMinutes:0.##}m";
            }
            return $"{elapsed.TotalHours:0.###}h";
        }
    }
}
