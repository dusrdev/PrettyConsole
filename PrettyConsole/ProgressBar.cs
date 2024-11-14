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

		private readonly char[] _percentageBuffer;

		private int _currentProgress = 0;

		private readonly char[] _pBuffer;

#if NET9_0_OR_GREATER
		private readonly Lock _lock = new();
#else
		private readonly object _lock = new();
#endif

		/// <summary>
		/// Represents a progress bar that can be displayed in the console.
		/// </summary>
		public ProgressBar() {
			_pBuffer = new char[baseConsole.BufferWidth];
			_percentageBuffer = new char[20];
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
			lock (_lock) {
				if (header.Length is 0) {
					header = Utils.FormatPercentage(percentage, _percentageBuffer);
				}
				int pLength = baseConsole.BufferWidth - header.Length - 5;
				var p = (int)(pLength * percentage * 0.01);
				if (p == _currentProgress) {
					return;
				}
				_currentProgress = p;
				ResetColors();
				baseConsole.ForegroundColor = ForegroundColor;
				var currentLine = GetCurrentLine();
				ClearNextLinesError(1);
				Error.Write('[');
				baseConsole.ForegroundColor = ProgressColor;
				Span<char> span = _pBuffer.AsSpan(0, p);
				span.Fill(ProgressChar);
				Span<char> end = WhiteSpace.AsSpan(0, pLength - p);
				Error.Write(span);
				Error.Write(end);
				baseConsole.ForegroundColor = ForegroundColor;
				Error.Write("] ");
				Error.Write(header);
				ResetColors();
				GoToLine(currentLine);
			}
		}
	}
}