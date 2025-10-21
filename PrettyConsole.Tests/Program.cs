using PrettyConsole;
using PrettyConsole.Tests;
using PrettyConsole.Tests.Features;

using static PrettyConsole.Console;

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
    new IndeterminateProgressBarTest(),
    new ProgressBarTest()
};

foreach (var test in tests) {
    await test.Render();
    NewLine();
}

#pragma warning disable CS8321 // Local function is declared but never used

static void Measure(string label, Action action) {
    long before = GC.GetAllocatedBytesForCurrentThread();
    action();
    long after = GC.GetAllocatedBytesForCurrentThread();
    NewLine();
    WriteLine($"{label} - allocated {after - before} bytes");
    NewLine();
}
#pragma warning restore CS8321 // Local function is declared but never used