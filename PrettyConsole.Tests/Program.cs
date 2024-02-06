// See https://aka.ms/new-console-template for more information

using System.Reflection;

using PrettyConsole.Tests;

using static PrettyConsole.Console;

var assembly = Assembly.GetExecutingAssembly();

var tests = assembly.GetTypes()
	.Where(x => x.GetInterfaces().Contains(typeof(IPrettyConsoleTest)))
	.Select(x => (IPrettyConsoleTest)Activator.CreateInstance(x)!)
	.ToArray();

foreach (var test in tests) {
	test.Render();
	NewLine();
}