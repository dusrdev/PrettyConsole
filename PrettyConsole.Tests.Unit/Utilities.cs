using System.Globalization;
using System.Text;

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
}