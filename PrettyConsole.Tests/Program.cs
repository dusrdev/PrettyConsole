// See https://aka.ms/new-console-template for more information

using System.Reflection;

using PrettyConsole.Tests;
using PrettyConsole.Tests.Features;

using static PrettyConsole.Console;

// var assembly = Assembly.GetExecutingAssembly();

// var tests = assembly.GetTypes()
// 	.Where(x => x.GetInterfaces().Contains(typeof(IPrettyConsoleTest)))
// 	.Select(x => (IPrettyConsoleTest)Activator.CreateInstance(x)!)
// 	.ToArray();

ReadOnlySpan<IPrettyConsoleTest> tests = new IPrettyConsoleTest[] {
	new ColoredOutputTest(),
	new SelectionTest(),
	new MultiSelectionTest(),
	new TableTest(),
	new TreeMenuTest(),
	new IndeterminateProgressBarTest(),
};

foreach (var test in tests) {
	test.Render();
	NewLine();
}