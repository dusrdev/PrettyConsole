using System.Globalization;

namespace PrettyConsole.UnitTests;

public class PrettyConsoleInterpolatedStringHandlerTests {
    private readonly StringWriter _writer;

    public PrettyConsoleInterpolatedStringHandlerTests() {
        Out = Utilities.GetWriter(out _writer);
    }

    [Test]
    [Arguments(0, 0, 0, "0h 0m 0s")]
    [Arguments(0, 1, 2, "0h 1m 2s")]
    [Arguments(5, 59, 59, "5h 59m 59s")]
    [Arguments(48, 30, 5, "48h 30m 5s")]
    [Arguments(1234, 0, 1, "1234h 0m 1s")]
    [Arguments(256204778, 48, 5, "256204778h 48m 5s")]
    public async Task AppendFormatted_TimeSpanDuration_WritesExpected(int hours, int minutes, int seconds, string expected) {
        var timeSpan = TimeSpan.FromHours(hours)
            .Add(TimeSpan.FromMinutes(minutes))
            .Add(TimeSpan.FromSeconds(seconds));

        Console.WriteInterpolated($"Elapsed {timeSpan:duration}");

        await Assert.That(_writer.ToStringAndFlush()).IsEqualTo($"Elapsed {expected}");
    }

    [Test]
    [Arguments(0d)]
    [Arguments(512d)]
    [Arguments(1024d)]
    [Arguments(15360d)]
    [Arguments(42_949_672_960d)]
    [Arguments(1.1258999068426228e105)]
    [Arguments(1.1258999068426251e105)]
    [Arguments(double.MaxValue)]
    public async Task AppendFormatted_DoubleBytes_WritesExpected(double value) {
        Console.WriteInterpolated($"Size {value:bytes}");

        await Assert.That(_writer.ToStringAndFlush()).IsEqualTo($"Size {FormatBytes(value)}");
    }

    [Test]
    public async Task CharsWritten_IgnoresAnsiColorAndMarkupSequences() {
        int chars = Console.WriteInterpolated($"{ConsoleColor.Red}{Markup.Bold}Hi{Markup.Reset}");

        var written = Utilities.StripAnsiSequences(_writer.ToStringAndFlush());

        await Assert.That(written).IsEqualTo("Hi");
        await Assert.That(chars).IsEqualTo(2);
    }

    [Test]
    [Arguments(5, "   OK")]
    [Arguments(-5, "OK   ")]
    public async Task Alignment_UsesVisibleLengthWhenMarkupPresent(int alignment, string expected) {
        int chars = alignment > 0
            ? Console.WriteInterpolated($"{Markup.Bold}{"OK",5}{Markup.Reset}")
            : Console.WriteInterpolated($"{Markup.Bold}{"OK",-5}{Markup.Reset}");

        var written = Utilities.StripAnsiSequences(_writer.ToStringAndFlush());

        await Assert.That(written).IsEqualTo(expected);
        await Assert.That(chars).IsEqualTo(expected.Length);
    }

    [Test]
    public async Task WriteLineInterpolated_ReturnsCharsWithoutNewline() {
        int chars = Console.WriteLineInterpolated($"Hi");

        var written = Utilities.StripAnsiSequences(_writer.ToStringAndFlush());

        await Assert.That(written).IsEqualTo("Hi" + Environment.NewLine);
        await Assert.That(chars).IsEqualTo(2);
    }

    [Test]
    public async Task WriteInterpolated_MixedPrimitivesAndFormats_ReturnsVisibleCount() {
        var duration = TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(7);

        int chars = Console.WriteInterpolated($"Id:{123} Ok:{true} Pi:{3.14159:F2} Char:{'X'} Elapsed:{duration:duration}");

        var expected = $"Id:123 Ok:True Pi:3.14 Char:X Elapsed:0h 5m 7s";
        await Assert.That(_writer.ToStringAndFlush()).IsEqualTo(expected);
        await Assert.That(chars).IsEqualTo(expected.Length);
    }

    [Test]
    public async Task WriteInterpolated_ColorTuple_DoesNotAffectVisibleCount() {
        int chars = Console.WriteInterpolated($"{(ConsoleColor.Red, ConsoleColor.White)}ERR{ConsoleColor.Default} done");

        var written = Utilities.StripAnsiSequences(_writer.ToStringAndFlush());

        await Assert.That(written).IsEqualTo("ERR done");
        await Assert.That(chars).IsEqualTo("ERR done".Length);
    }

    [Test]
    [Arguments(6)]
    [Arguments(-6)]
    public async Task WriteInterpolated_ColorTupleAlignment_UsesWidthOnly(int alignment) {
        int chars = alignment > 0
            ? Console.WriteInterpolated($"{(ConsoleColor.Blue, ConsoleColor.White),6}")
            : Console.WriteInterpolated($"{(ConsoleColor.Blue, ConsoleColor.White),-6}");

        var written = Utilities.StripAnsiSequences(_writer.ToStringAndFlush());
        var expected = new string(' ', 6);

        await Assert.That(written).IsEqualTo(expected);
        await Assert.That(chars).IsEqualTo(6);
    }

    [Test]
    public async Task WriteLineInterpolated_WithColorsAndPrimitives_CountExcludesNewline() {
        var duration = TimeSpan.FromSeconds(42);
        var writer = new StringWriter();
        Error = writer;

        int chars = Console.WriteLineInterpolated(OutputPipe.Error, $"{ConsoleColor.Yellow}[{duration:duration}] {Markup.Bold}done{Markup.Reset}");

        var stripped = Utilities.StripAnsiSequences(writer.ToString());
        var expected = $"[0h 0m 42s] done{Environment.NewLine}";

        await Assert.That(stripped).IsEqualTo(expected);
        await Assert.That(chars).IsEqualTo("[0h 0m 42s] done".Length);
    }

    private static string FormatBytes(double value) {
        const double formatBytesKb = 1024d;
        var suffix = 0;
        var num = value;

        while (suffix < FileSizeSuffix.Length - 1 && num >= formatBytesKb) {
            num /= formatBytesKb;
            suffix++;
        }

        return string.Format(CultureInfo.CurrentCulture, "{0:#,##0.##} {1}", num, FileSizeSuffix[suffix]);
    }

    private static ReadOnlySpan<string> FileSizeSuffix => new[] { "B", "KB", "MB", "GB", "TB", "PB" };
}
