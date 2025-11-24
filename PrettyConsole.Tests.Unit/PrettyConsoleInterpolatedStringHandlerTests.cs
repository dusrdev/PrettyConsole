using System.Globalization;

namespace PrettyConsole.Tests.Unit;

public class PrettyConsoleInterpolatedStringHandlerTests {
    private readonly StringWriter _writer;

    public PrettyConsoleInterpolatedStringHandlerTests() {
        Out = Utilities.GetWriter(out _writer);
    }

    [Theory]
    [InlineData(0, 0, 0, "0h 0m 0s")]
    [InlineData(0, 1, 2, "0h 1m 2s")]
    [InlineData(5, 59, 59, "5h 59m 59s")]
    [InlineData(48, 30, 5, "48h 30m 5s")]
    [InlineData(1234, 0, 1, "1234h 0m 1s")]
    [InlineData(256204778, 48, 5, "256204778h 48m 5s")] // TimeSpan.MaxValue
    public void AppendFormatted_TimeSpanDuration_WritesExpected(int hours, int minutes, int seconds, string expected) {
        var timeSpan = TimeSpan.FromHours(hours)
            .Add(TimeSpan.FromMinutes(minutes))
            .Add(TimeSpan.FromSeconds(seconds));

        Console.WriteInterpolated($"Elapsed {timeSpan:duration}");

        Assert.Equal($"Elapsed {expected}", _writer.ToStringAndFlush());
    }

    [Theory]
    [InlineData(0d)]
    [InlineData(512d)]
    [InlineData(1024d)]
    [InlineData(15360d)]
    [InlineData(42_949_672_960d)]
    [InlineData(1.1258999068426228e105)] // scales to just under 1e90 PB => uses default stack buffer
    [InlineData(1.1258999068426251e105)] // scales to just over 1e90 PB => uses large stack buffer
    [InlineData(double.MaxValue)]
    public void AppendFormatted_DoubleBytes_WritesExpected(double value) {
        Console.WriteInterpolated($"Size {value:bytes}");

        Assert.Equal($"Size {FormatBytes(value)}", _writer.ToStringAndFlush());
    }

    [Fact]
    public void CharsWritten_IgnoresAnsiColorAndMarkupSequences() {
        int chars = Console.WriteInterpolated($"{ConsoleColor.Red}{Markup.Bold}Hi{Markup.Reset}");

        var written = Utilities.StripAnsiSequences(_writer.ToStringAndFlush());

        Assert.Equal("Hi", written);
        Assert.Equal(2, chars);
    }

    [Theory]
    [InlineData(5, "   OK")]
    [InlineData(-5, "OK   ")]
    public void Alignment_UsesVisibleLengthWhenMarkupPresent(int alignment, string expected) {
        int chars = alignment > 0
            ? Console.WriteInterpolated($"{Markup.Bold}{"OK",5}{Markup.Reset}")
            : Console.WriteInterpolated($"{Markup.Bold}{"OK",-5}{Markup.Reset}");

        var written = Utilities.StripAnsiSequences(_writer.ToStringAndFlush());

        Assert.Equal(expected, written);
        Assert.Equal(expected.Length, chars);
    }

    [Fact]
    public void WriteLineInterpolated_ReturnsCharsWithoutNewline() {
        int chars = Console.WriteLineInterpolated($"Hi");

        var written = Utilities.StripAnsiSequences(_writer.ToStringAndFlush());

        Assert.Equal("Hi" + Environment.NewLine, written);
        Assert.Equal(2, chars);
    }

    [Fact]
    public void WriteInterpolated_MixedPrimitivesAndFormats_ReturnsVisibleCount() {
        var duration = TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(7);

        int chars = Console.WriteInterpolated($"Id:{123} Ok:{true} Pi:{3.14159:F2} Char:{'X'} Elapsed:{duration:duration}");

        var expected = $"Id:123 Ok:True Pi:3.14 Char:X Elapsed:0h 5m 7s";
        Assert.Equal(expected, _writer.ToStringAndFlush());
        Assert.Equal(expected.Length, chars);
    }

    [Fact]
    public void WriteInterpolated_ColorTuple_DoesNotAffectVisibleCount() {
        int chars = Console.WriteInterpolated($"{(ConsoleColor.Red, ConsoleColor.White)}ERR{ConsoleColor.Default} done");

        var written = Utilities.StripAnsiSequences(_writer.ToStringAndFlush());

        Assert.Equal("ERR done", written);
        Assert.Equal("ERR done".Length, chars);
    }

    [Theory]
    [InlineData(6)]
    [InlineData(-6)]
    public void WriteInterpolated_ColorTupleAlignment_UsesWidthOnly(int alignment) {
        int chars = alignment > 0
            ? Console.WriteInterpolated($"{(ConsoleColor.Blue, ConsoleColor.White),6}")
            : Console.WriteInterpolated($"{(ConsoleColor.Blue, ConsoleColor.White),-6}");

        var written = Utilities.StripAnsiSequences(_writer.ToStringAndFlush());
        var expected = new string(' ', 6);

        Assert.Equal(expected, written);
        Assert.Equal(6, chars);
    }

    [Fact]
    public void WriteLineInterpolated_WithColorsAndPrimitives_CountExcludesNewline() {
        var duration = TimeSpan.FromSeconds(42);
        var writer = new StringWriter();
        Error = writer;

        int chars = Console.WriteLineInterpolated(OutputPipe.Error, $"{ConsoleColor.Yellow}[{duration:duration}] {Markup.Bold}done{Markup.Reset}");

        var stripped = Utilities.StripAnsiSequences(writer.ToString());
        var expected = $"[0h 0m 42s] done{Environment.NewLine}";

        Assert.Equal(expected, stripped);
        Assert.Equal("[0h 0m 42s] done".Length, chars);
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