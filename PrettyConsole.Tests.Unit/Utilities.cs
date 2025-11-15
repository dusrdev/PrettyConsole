using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace PrettyConsole.Tests.Unit;

public static partial class Utilities {
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

    public static string StripAnsiSequences(string value) {
        if (string.IsNullOrEmpty(value)) {
            return string.Empty;
        }

        return AnsiSequenceRegex().Replace(value, string.Empty);
    }

    public static void SkipIfNoInteractiveConsole() {
        const string reason = "Interactive console APIs are not available in this environment.";

        if (Console.IsOutputRedirected) {
            Assert.Skip(reason);
        }

        try {
            _ = Console.CursorTop;
        } catch (IOException) {
            Assert.Skip(reason);
        } catch (PlatformNotSupportedException) {
            Assert.Skip(reason);
        }
    }

    [GeneratedRegex("\\u001b\\[[0-9;]*m", RegexOptions.Compiled)]
    private static partial Regex AnsiSequenceRegex();
}