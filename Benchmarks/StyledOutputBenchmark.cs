using System.Runtime.CompilerServices;

using BenchmarkDotNet.Attributes;

using Spectre.Console;

using PrettyConsole;
using static System.ConsoleColor;

namespace Benchmarks;

/// <summary>
/// Runs a benchmark printing the following output:
/// Hello {Green}John{ResetColor}, status = {Cyan}{Percentage}{Reset}%, Elapsed = {Yellow}{Elapsed:c}{Reset}
/// </summary>
public class StyledOutputBenchmarks {
	private static readonly TimeSpan Elapsed = new(1, 25, 31);
	private const double Percentage = 57.91;

	[Benchmark(Baseline = true)]
	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SystemConsole() {
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
	}

	[Benchmark]
	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SpectreConsole() {
		AnsiConsole.MarkupLineInterpolated($"Hello [green]John[/], status = [cyan]{Percentage}[/]%, elapsed = [yellow]{Elapsed:c}[/]");
	}

	[Benchmark]
	[MethodImpl(MethodImplOptions.NoInlining)]
	public void PrettyConsole() {
		Console.WriteLineInterpolated($"Hello {Green}John{ConsoleColor.Default}, status = {Cyan}{Percentage}{ConsoleColor.Default}%, elapsed = {Yellow}{Elapsed:hr}");
	}
}