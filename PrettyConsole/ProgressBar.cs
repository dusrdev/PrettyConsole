using System.Buffers;
using System.Runtime.CompilerServices;

namespace PrettyConsole;

public static partial class Console {
	/// <summary>
	/// Represents a progress bar that can be displayed in the console.
	/// </summary>
	/// <remarks>
	/// The progress bar update isn't tied to unit of time, it's up to the user to update it as needed. By managing the when the Update method is called, the user can have a more precise control over the progress bar. More calls, means more frequent rendering but at the cost of performance (very frequent updates may cause the terminal to lose sync with the method and will produce visual bugs, such as items not rendering in the right place)
	/// </remarks>
	public class ProgressBar : IDisposable {
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

        private readonly IMemoryOwner<char> _progressBufferOwner;

		private readonly Memory<char> _progressBuffer;

        private readonly IMemoryOwner<char> _percentageBufferOwner;

		private readonly string _emptyLine;

		private int _currentProgress = 0;

        private volatile bool _disposed;

		/// <summary>
		/// Represents a progress bar that can be displayed in the console.
		/// </summary>
		public ProgressBar() {
            int length = baseConsole.BufferWidth - 10;
            _progressBufferOwner = Utils.ObtainMemory(length);
			_progressBuffer = _progressBufferOwner.Memory.Slice(0, length);
			_progressBuffer.Span.Fill(' ');
            _percentageBufferOwner = Utils.ObtainMemory(20);
            _emptyLine = new string(' ', baseConsole.BufferWidth);
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
			var currentLine = baseConsole.CursorTop;
			baseConsole.SetCursorPosition(0, currentLine);
			Error.WriteLine(_emptyLine);
			Error.WriteLine(_emptyLine);
			baseConsole.SetCursorPosition(0, currentLine);
			if (header.Length is not 0) {
				Error.WriteLine(header);
			}

			Error.Write('[');
			var p = (int)(_progressBuffer.Length * percentage * 0.01);
			baseConsole.ForegroundColor = ProgressColor;
			Span<char> span = _progressBuffer.Span;
			Span<char> full = span.Slice(_currentProgress, p);
			full.Fill(ProgressChar);
			_currentProgress = p;
			// Span<char> empty = span.Slice(p);
			// empty.Fill(' ');
			// Error.Write(full);
			// Error.Write(empty);
			Error.Write(span);
			baseConsole.ForegroundColor = ForegroundColor;
			Error.Write("] ");
			Error.Write(Utils.FormatPercentage(percentage, _percentageBufferOwner.Memory.Span));
			baseConsole.SetCursorPosition(0, currentLine);
			ResetColors();
		}

        /// <summary>
        /// Releases the resources held by this object
        /// </summary>
        public void Dispose() {
            if (_disposed) {
                return;
            }
            _progressBufferOwner.Dispose();
            _percentageBufferOwner.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}