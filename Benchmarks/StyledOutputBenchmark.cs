using BenchmarkDotNet.Attributes;

using PrettyConsole;

using Spectre.Console;

using static System.ConsoleColor;

namespace Benchmarks;

/// <summary>
/// Runs a benchmark printing the following output:
/// Hello {Green}John{ResetColor}, status = {Cyan}{Percentage}{Reset}%, Elapsed = {Yellow}{Elapsed:c}{Reset}
/// </summary>
[Config(typeof(Config))]
public class StyledOutputBenchmarks {
    private static readonly TimeSpan Elapsed = new(1, 25, 31);
    private const double Percentage = 57.91;

    private TextWriter _outputWriter = default!;
    private IAnsiConsole _ansiConsole = default!;

    [GlobalSetup]
    public void GlobalSetup() {
        _outputWriter = Console.Out;
        PrettyConsoleExtensions.Out = TextWriter.Null;
        _ansiConsole = AnsiConsole.Create(new AnsiConsoleSettings {
            Out = new AnsiConsoleOutput(TextWriter.Null)
        });
        Console.SetOut(TextWriter.Null);
    }

    [GlobalCleanup]
    public void GlobalCleanup() {
        PrettyConsoleExtensions.Out = _outputWriter;
        _ansiConsole = AnsiConsole.Console;
        Console.SetOut(_outputWriter);
    }

    [Benchmark]
    public int PrettyConsole() {
        Console.WriteLineInterpolated($"Hello {Green}John{ConsoleColor.DefaultForeground}, status = {Cyan}{Percentage}{ConsoleColor.DefaultForeground}%, elapsed = {Yellow}{Elapsed:c}");
        return int.MaxValue;
    }

    [Benchmark(Baseline = true)]
    public int SpectreConsole() {
        _ansiConsole.MarkupLineInterpolated($"Hello [green]John[/], status = [cyan]{Percentage}[/]%, elapsed = [yellow]{Elapsed:c}[/]");
        return int.MaxValue;
    }

    [Benchmark]
    public int SystemConsole() {
        Console.Write("Hello ");
        Console.ForegroundColor = Green;
        Console.Write("John");
        Console.ResetColor();
        Console.Write(", status = ");
        Console.ForegroundColor = Cyan;
        Console.Write(Percentage);
        Console.ResetColor();
        Console.Write("%, elapsed = ");
        Console.ForegroundColor = Yellow;
        Console.WriteLine("{0:c}", Elapsed);
        Console.ResetColor();
        return int.MaxValue;
    }
}