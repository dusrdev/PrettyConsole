#:package PrettyConsole@5.4.0

using PrettyConsole;

Console.CursorVisible = false;
for (int i = 0; i <= 100; i += 4) {
	Console.Overwrite(i, static ii => {
		ProgressBar.Render(OutputPipe.Error, ii, ConsoleColor.Cyan, maxLineWidth: 40);
		Console.NewLine(OutputPipe.Error);
		Console.WriteInterpolated(OutputPipe.Error, $"Downloading assets... {ConsoleColor.Cyan}{ii}");
	}, lines: 2, pipe: OutputPipe.Error);

	await Task.Delay(70);
}
Console.CursorVisible = true;

Console.ClearNextLines(2, OutputPipe.Error);
Console.WriteLineInterpolated($"{ConsoleColor.Green}Download complete!");
