#:package PrettyConsole@5.3.0

using System.Diagnostics;
using PrettyConsole;

string[] steps = [
	"Restore",
	"Compile",
	"Link native shims",
	"Run analyzers",
	"Pack artifacts",
];

var step = 0;
var start = Stopwatch.GetTimestamp();

var build = Task.Run(async () => {
	for (; step < steps.Length; Interlocked.Increment(ref step)) {
		await Task.Delay(800);
	}
});

var progressBar = new IndeterminateProgressBar {
	AnimationSequence = IndeterminateProgressBar.Patterns.Braille,
	ForegroundColor = ConsoleColor.Green,
	DisplayElapsedTime = true,
	UpdateRate = 100,
};

await progressBar.RunAsync(build, (builder, out handler) => {
	var current = Volatile.Read(ref step);
	handler = builder.Build(OutputPipe.Error, $"Current step: {ConsoleColor.Green}{steps[current]}");
}, CancellationToken.None);

var elapsed = Stopwatch.GetElapsedTime(start);
Console.WriteLineInterpolated($"Build complete in {ConsoleColor.Green}{elapsed:duration}");
