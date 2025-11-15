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