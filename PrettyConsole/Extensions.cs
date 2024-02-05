using System;
using System.Collections.Generic;
using System.Text;

namespace PrettyConsole;

internal static class Extensions {
    // Returns a string with <paramref name="length"/> spaces
    internal static string GetWhiteSpaces(int length) {
        return new string(' ', length);
    }

    // A faster and more memory efficient implementation of string.split
    internal static IEnumerable<string> Split(string str, char separator) {
        StringBuilder builder = new();
        foreach (var c in str) {
            if (c.Equals(separator)) {
                if (builder.Length <= 0) {
                    continue;
                }

                yield return builder.ToString().Trim();
                builder.Clear();
                continue;
            }

            builder.Append(c);
        }
        var rest = builder.ToString().Trim();
        if (!string.IsNullOrWhiteSpace(rest)) {
            yield return rest;
        }
    }

    // Returns a user friendly representation
    internal static string ToFriendlyString(this TimeSpan elapsed) {
        if (elapsed.TotalSeconds < 60) {
            return string.Create(null, stackalloc char[10], $"{elapsed.TotalSeconds:0.##}s");
        }
        if (elapsed.Minutes < 60) {
            return string.Create(null, stackalloc char[10], $"{elapsed.TotalMinutes:0.##}m");
        }
        return string.Create(null, stackalloc char[10], $"{elapsed.TotalHours:0.###}h");
    }
}
