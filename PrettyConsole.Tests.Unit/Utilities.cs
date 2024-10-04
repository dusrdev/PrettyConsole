namespace PrettyConsole.Tests.Unit;

public static class Utilities {
	public static TextWriter SetWriter(out StringWriter writer) {
		writer = new StringWriter();
		return writer;
	}
}