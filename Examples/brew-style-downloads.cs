#:package PrettyConsole@5.3.0

using PrettyConsole;

bool keepProgressOutput = true;

var downloads = BrewStyleDownloads.CreateDownloadTasks();
var count = downloads.Count;

var spinner = IndeterminateProgressBar.Patterns.Braille;
var spinnerLength = spinner.Count;
int spinnerIndex = 0;

var bufferWidth = Console.BufferWidth;

var task = Task.WhenAll(downloads.Select(BrewStyleDownloads.AdvanceDownload));

Console.CursorVisible = false; // Hide cursor to see progress better
while (!task.IsCompleted) {
	spinnerIndex = (spinnerIndex + 1) % spinnerLength;

	Console.Overwrite(() => {
		foreach (var download in downloads) {
			int written = 0;
			if (download.IsComplete) {
				written += Console.WriteInterpolated(OutputPipe.Error, $"{ConsoleColor.Green}✔︎{ConsoleColor.DefaultForeground} {download.Name}") - 1;
				// I remove 1 from written here because "✔︎" is 2 characters long but renders a single block in the terminal
			} else {
				written += Console.WriteInterpolated(OutputPipe.Error, $"{ConsoleColor.Green}{spinner[spinnerIndex]}{ConsoleColor.DefaultForeground} {download.Name}");
			}

			var current = (double)download.BytesDownloaded;
			var total = (double)download.FileSize;

			var padding = bufferWidth - 34 - written;

			// progress section takes 34 characters - found by measuring in test run
			// var progressLength = Console.WriteLineInterpolated(... without WhiteSpace)
			Console.WriteLineInterpolated(OutputPipe.Error, $"{new WhiteSpace(padding)}[Downloaded {current,10:bytes}/{total,10:bytes}]");
		}
	}, count);

	await Task.Delay(25);
}
Console.CursorVisible = true; // restore cursor visibility

if (keepProgressOutput) {
	Console.SkipLines(count);
	Console.WriteLine(); // Add a newline to separate progress from future outputs
} else {
	Console.ClearNextLines(count, OutputPipe.Error);
}

Console.WriteLineInterpolated($"{ConsoleColor.Green}Done!{ConsoleColor.DefaultForeground}");

/// <summary>
/// Represents a single download in a "brew" style feed with progress tracking.
/// </summary>
sealed class DownloadStyleTask {
	public DownloadStyleTask(string name, long fileSize) {
		Name = name;
		FileSize = fileSize;
	}

	public string Name { get; }

	public long BytesDownloaded { get; private set; }

	public bool IsComplete { get; private set; }

	public long FileSize { get; }

	/// <summary>
	/// Advance the download and notify any listeners.
	/// </summary>
	public void Advance(long bytes) {
		if (bytes <= 0 || IsComplete) {
			return;
		}

		var updated = BytesDownloaded + bytes;
		BytesDownloaded = updated >= FileSize ? FileSize : updated;
		if (BytesDownloaded == FileSize) IsComplete = true;
	}
}

static class BrewStyleDownloads
{
	/// <summary>
	/// Creates sample download tasks you can feed into a renderer or progress loop.
	/// Each task exposes an event so a UI can react when progress changes.
	/// </summary>
	public static List<DownloadStyleTask> CreateDownloadTasks() {
		var tasks = new List<DownloadStyleTask>
		{
			new("git", 48_000_000),
			new("curl", 27_500_000),
			new("openssl", 96_000_000),
			new("python@3.13", 121_000_000)
		};

		return tasks;
	}

	public static async Task AdvanceDownload(DownloadStyleTask task) {
		const long minChunkSize = 1_000_000;

		while (task.BytesDownloaded < task.FileSize) {
			var chunk = Random.Shared.NextInt64(minChunkSize, 2 * minChunkSize);
			var remaining = task.FileSize - task.BytesDownloaded;
			var current = Math.Min(chunk, remaining);
			task.Advance(current);
			var delay = Random.Shared.Next(150, 250);
			await Task.Delay(delay);
		}
	}
}
