#:package PrettyConsole@6.0.0

using PrettyConsole;

Console.CursorVisible = false;
for (int i = 0; i <= 100; i += 4) {
	Console.Overwrite(i, static ii => {
		ProgressBar.Render(OutputPipe.Error, ii, Color.Cyan, maxLineWidth: 40);
		Console.NewLine(OutputPipe.Error);
		Console.WriteInterpolated(OutputPipe.Error, $"Downloading assets... {Color.Cyan}{ii}");
	}, lines: 2, pipe: OutputPipe.Error);

	await Task.Delay(70);
}
Console.CursorVisible = true;

Console.ClearNextLines(2, OutputPipe.Error);
Console.WriteLineInterpolated($"{Color.Green}Download complete!");
