using System.Runtime.CompilerServices;

namespace PrettyConsole;

public static partial class Console {
	/// <summary>
	/// Represents a progress bar that can be displayed in the console.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The progress bar update isn't tied to unit of time, it's up to the user to update it as needed. By managing the when the Update method is called, the user can have a more precise control over the progress bar. More calls, means more frequent rendering but at the cost of performance (very frequent updates may cause the terminal to lose sync with the method and will produce visual bugs, such as items not rendering in the right place)
	/// </para>
	/// <para>
	/// Updating the progress bar with decreasing percentages will cause visual bugs as it is optimized to skip rendering pre-filled characters.
	/// </para>
	/// </remarks>
	public class ProgressBar {
		/// <summary>
		/// Gets or sets the character used to represent the progress.
		/// </summary>
		public char ProgressChar { get; set; } = '■';

		/// <summary>
		/// Gets or sets the foreground color of the progress bar.
		/// </summary>
		public ConsoleColor ForegroundColor { get; set; } = Color.DefaultForegroundColor;

		/// <summary>
		/// Gets or sets the color of the progress portion of the bar.
		/// </summary>
		public ConsoleColor ProgressColor { get; set; } = Color.DefaultForegroundColor;

        private readonly char[] _progressBuffer;

        private readonly char[] _percentageBuffer;

		private readonly ReadOnlyMemory<char> _emptyLine;

		private int _currentProgress = 0;

		/// <summary>
		/// Represents a progress bar that can be displayed in the console.
		/// </summary>
		public ProgressBar() {
            int length = baseConsole.BufferWidth - 10;
            _progressBuffer = new char[length];
			_progressBuffer.AsSpan().Fill(' ');
            _percentageBuffer = new char[20];
			_emptyLine = WhiteSpace.AsMemory(0, baseConsole.BufferWidth);
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
		public void Update(double percentage, ReadOnlySpan<char> header) {
			ResetColors();
			baseConsole.ForegroundColor = ForegroundColor;
			var currentLine = GetCurrentLine();
			GoToLine(currentLine);
			Error.WriteLine(_emptyLine.Span);
			Error.WriteLine(_emptyLine.Span);
			GoToLine(currentLine);
			if (header.Length is not 0) {
				Error.WriteLine(header);
			}

			Error.Write('[');
			var p = (int)(_progressBuffer.Length * percentage * 0.01);
			baseConsole.ForegroundColor = ProgressColor;
			Span<char> span = _progressBuffer;
			// full is only the part between old progress and new progress, the rest was written in previous iterations
			Span<char> full = span.Slice(_currentProgress, p - _currentProgress);
			full.Fill(ProgressChar);
			_currentProgress = p;
			Error.Write(span);
			baseConsole.ForegroundColor = ForegroundColor;
			Error.Write("] ");
			Error.Write(Utils.FormatPercentage(percentage, _percentageBuffer));
			GoToLine(currentLine);
			ResetColors();
		}
    }
}