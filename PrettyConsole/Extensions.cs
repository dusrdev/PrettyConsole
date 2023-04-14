﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PrettyConsole;

internal static class Extensions {
    // Returns a string with <paramref name="length"/> spaces
    public static string GetWhiteSpaces(int length) {
        return new string(' ', length);
    }

    // Returns a string completed with spaces to fill a certain length
    public static string SuffixWithSpaces(ReadOnlySpan<char> str, int maxLength) {
        if (IsEmptyOrWhiteSpace(str) || str.Length > maxLength) {
            return new string(' ', maxLength);
        }
        return string.Concat(str, GetWhiteSpaces(maxLength - str.Length));
    }

    // Checks whether the ReadOnlySpan is empty or only contains whitespace characters
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmptyOrWhiteSpace(ReadOnlySpan<char> str) => str.IsEmpty || str.IsWhiteSpace();

    // A faster and more memory efficient implementation of string.split
    public static IEnumerable<string> Split(string str, char separator) {
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
