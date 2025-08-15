using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

		// small buffer to write percentages
		private readonly char[] _percentageBuffer = new char[20];

		// The buffer used for writing the progress
		private readonly List<char> _buffer = new(256);

		private int _currentProgress = 0;

#if NET9_0_OR_GREATER
		private readonly Lock _lock = new();
#else
		private readonly object _lock = new();
#endif

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
		/// <param name="status">The status text to be displayed after the progress bar.</param>
		public void Update(double percentage, ReadOnlySpan<char> status) {
			lock (_lock) {
				percentage = Math.Clamp(percentage, 0, 100);
				int bufferWidth = GetWidthOrDefault();

				// Ensure buffer capacity
				_buffer.EnsureCapacity(bufferWidth);
				CollectionsMarshal.SetCount(_buffer, bufferWidth);
				Span<char> buf = CollectionsMarshal.AsSpan(_buffer);

				var percentageSpan = Utils.FormatPercentage(percentage, _percentageBuffer);
				int pLength = Math.Max(0, bufferWidth - status.Length - 12);
				int p = Math.Clamp((int)(pLength * percentage * 0.01), 0, pLength);
				if (p == _currentProgress) {
					return;
				}
				_currentProgress = p;

				ResetColors();
				baseConsole.ForegroundColor = ForegroundColor;
				var currentLine = GetCurrentLine();
				ClearNextLines(1, OutputPipe.Error);
				if (status.Length != 0) {
					Error.Write(status);
				}
				Error.Write(" [");
				baseConsole.ForegroundColor = ProgressColor;

				Span<char> progressSpan = buf.Slice(0, p);
				progressSpan.Fill(ProgressChar);

				int tailLength = Math.Max(0, pLength - p);
				if (tailLength > 0) {
					Span<char> whiteSpaceSpan = buf.Slice(p, tailLength);
					whiteSpaceSpan.Fill(' ');
					p += tailLength;
				}

				Error.Write(buf.Slice(0, p));

				baseConsole.ForegroundColor = ForegroundColor;
				Error.Write("] ");
				Error.Write(percentageSpan);
				ResetColors();
				GoToLine(currentLine);

				// Reset buffer
				_buffer.Clear();
			}
		}
	}
}