#:package PrettyConsole@5.4.0

using PrettyConsole;

var environment = PromptSelection(
	title: "Select environment",
	options: ["Development", "Staging", "Production"]);

var features = PromptMultiSelection(
	title: "Choose features (space to pick multiple)",
	options: ["Core", "Metrics", "Tracing"]);

var region = PromptSelection(
	title: "Pick region",
	options: ["us-east", "us-west", "eu-central"]);

Console.WriteLineInterpolated($"{ConsoleColor.Green}Ready to deploy!");
Console.WriteLineInterpolated($"Environment: {Markup.Underline}{environment}{Markup.ResetUnderline}");
Console.WriteLineInterpolated($"Features:    {Markup.Underline}{string.Join(", ", features)}{Markup.ResetUnderline}");
Console.WriteLineInterpolated($"Region:      {Markup.Underline}{region}{Markup.ResetUnderline}");

// The following helper methods wrap the selection in Overwrite to resemble page routes
// If you want simple scrolling you can use Console.Selection or Console.MultiSelection directly.

static string PromptSelection(string title, string[] options) {
	string selection = string.Empty;

	while (selection.Length == 0) {
		Console.Overwrite(() => {
			selection = Console.Selection(options, $"{ConsoleColor.Cyan}{title}{ConsoleColor.DefaultForeground}:");
			if (selection.Length == 0) {
				Console.WriteLineInterpolated(OutputPipe.Error, $"{ConsoleColor.Red}Invalid choice. Try again.");
			}
		}, lines: options.Length + 3, pipe: OutputPipe.Out);
	}

	return selection;
}

static string[] PromptMultiSelection(string title, string[] options) {
	string[] selection = Array.Empty<string>();

	while (selection.Length == 0) {
		Console.Overwrite(() => {
			selection = Console.MultiSelection(options, $"{ConsoleColor.Cyan}{title}{ConsoleColor.DefaultForeground}:");
			if (selection.Length == 0) {
				Console.WriteLineInterpolated(OutputPipe.Error, $"{ConsoleColor.Red}Please pick at least one option.");
			}
		}, lines: options.Length + 3, pipe: OutputPipe.Out);
	}

	return selection;
}
