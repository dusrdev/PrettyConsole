using System.Runtime.CompilerServices;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
	/// <summary>
	/// Represents a progress bar that can be displayed in the console.
	/// </summary>
	/// <remarks>
	/// The progress bar update isn't tied to unit of time, it's up to the user to update it as needed. By managing the when the Update method is called, the user can have a more precise control over the progress bar. More calls, means more frequent rendering but at the cost of performance (very frequent updates may cause the terminal to lose sync with the method and will produce visual bugs, such as items not rendering in the right place)
	/// </remarks>
	public class ProgressBar {
		/// <summary>
		/// Gets or sets the character used to represent the progress.
		/// </summary>
		public char ProgressChar { get; set; } = '■';

		/// <summary>
		/// Gets or sets the foreground color of the progress bar.
		/// </summary>
		public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Gray;

		/// <summary>
		/// Gets or sets the color of the progress portion of the bar.
		/// </summary>
		public ConsoleColor ProgressColor { get; set; } = ConsoleColor.Gray;

		private readonly char[] _buffer;

		private readonly char[] _emptyLine;

		/// <summary>
		/// Represents a progress bar that can be displayed in the console.
		/// </summary>
		public ProgressBar() {
			_buffer = GC.AllocateUninitializedArray<char>(ogConsole.BufferWidth - 10);
			_emptyLine = GC.AllocateUninitializedArray<char>(ogConsole.BufferWidth);
			_emptyLine.AsSpan().Fill(' ');
		}

		/// <summary>
		/// Updates the progress bar with the specified percentage.
		/// </summary>
		/// <param name="percentage">The percentage value (0-100) representing the progress.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Update(int percentage) => Update(percentage, ReadOnlySpan<char>.Empty);

		/// <summary>
		/// Updates the progress bar with the specified percentage.
		/// </summary>
		/// <param name="percentage">The percentage value (0-100) representing the progress.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Update(double percentage) => Update(percentage, ReadOnlySpan<char>.Empty);

		/// <summary>
		/// Updates the progress bar with the specified percentage and header text.
		/// </summary>
		/// <param name="percentage">The percentage value (0-100) representing the progress.</param>
		/// <param name="header">The header text to be displayed above the progress bar.</param>
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public void Update(double percentage, ReadOnlySpan<char> header) {
			ResetColors();
			ogConsole.ForegroundColor = ForegroundColor;
			var currentLine = ogConsole.CursorTop;
			ogConsole.SetCursorPosition(0, currentLine);
			ogConsole.Out.WriteLine(_emptyLine.AsSpan());
			ogConsole.Out.WriteLine(_emptyLine.AsSpan());
			ogConsole.SetCursorPosition(0, currentLine);
			if (header.Length is not 0) {
				ogConsole.Out.WriteLine(header);
			}

			ogConsole.Out.Write('[');
			var p = (int)(_buffer.Length * percentage * 0.01);
			ogConsole.ForegroundColor = ProgressColor;
			Span<char> span = _buffer;
			Span<char> full = span[..p];
			full.Fill(ProgressChar);
			Span<char> empty = span[p..];
			empty.Fill(' ');
			ogConsole.Out.Write(full);
			ogConsole.Out.Write(empty);
			ogConsole.ForegroundColor = ForegroundColor;
			ogConsole.Out.Write(']');
			ogConsole.Out.Write(' ');
			ogConsole.Out.Write(percentage.FormattedPercentage());
			ogConsole.SetCursorPosition(0, currentLine);
			ResetColors();
		}
	}
}