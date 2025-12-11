using PrettyConsole;
using PrettyConsole.Tests;
using PrettyConsole.Tests.Features;

// var assembly = Assembly.GetExecutingAssembly();

// var tests = assembly.GetTypes()
// 	.Where(x => x.GetInterfaces().Contains(typeof(IPrettyConsoleTest)))
// 	.Select(x => (IPrettyConsoleTest)Activator.CreateInstance(x)!)
// 	.ToArray();

var tests = new IPrettyConsoleTest[] {
    new ColoredOutputTest(),
    new SelectionTest(),
    new MultiSelectionTest(),
    new TableTest(),
    new TreeMenuTest(),
    new SpinnerTest(),
    new ProgressBarDefaultTest(),
    new ProgressBarMultiLineTest(),
    new MultiProgressBarTest(),
    new MultiProgressBarLeftAlignedTest(),
};

foreach (var test in tests) {
    await test.Render();
    Console.NewLine();
}

#pragma warning disable CS8321 // Local function is declared but never used
static void Measure(string label, Action action) {
    long before = GC.GetAllocatedBytesForCurrentThread();
    action();
    long after = GC.GetAllocatedBytesForCurrentThread();
    Console.NewLine();
    Console.WriteLineInterpolated($"{label} - allocated {after - before} bytes");
    Console.NewLine();
}
#pragma warning restore CS8321 // Local function is declared but never used