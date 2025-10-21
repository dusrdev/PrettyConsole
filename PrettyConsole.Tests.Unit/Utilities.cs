using System;
using System.Globalization;
using System.Text;

using Xunit;

namespace PrettyConsole.Tests.Unit;

public static class Utilities {
    public static StringReader GetReader(string str) => new(str);

    public static TextWriter GetWriter(out StringWriter writer) {
        writer = new StringWriter(new StringBuilder(), CultureInfo.CurrentCulture);
        return writer;
    }

    public static string ToStringAndFlush(this StringWriter writer) {
        var result = writer.ToString();
        writer.GetStringBuilder().Clear();
        return result;
    }

    public static string WithNewLine(this string str) => string.Concat(str, Environment.NewLine);

    public static void SkipIfNoInteractiveConsole() {
        const string reason = "Interactive console APIs are not available in this environment.";

        if (System.Console.IsOutputRedirected) {
            Assert.Skip(reason);
        }

        try {
            _ = System.Console.CursorTop;
        } catch (System.IO.IOException) {
            Assert.Skip(reason);
        } catch (PlatformNotSupportedException) {
            Assert.Skip(reason);
        }
    }
}