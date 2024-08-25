using System.Buffers;
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
	public class ProgressBar : IDisposable {
		/// <summary>
		/// Gets or sets the character used to represent the progress.
		/// </summary>
		public char ProgressChar { get; set; } = '■';

		/// <summary>
		/// Gets or sets the foreground color of the progress bar.
		/// </summary>
		public ConsoleColor ForegroundColor { get; set; } = UnknownColor;

		/// <summary>
		/// Gets or sets the color of the progress portion of the bar.
		/// </summary>
		public ConsoleColor ProgressColor { get; set; } = UnknownColor;

        private readonly IMemoryOwner<char> _progressBufferOwner;

		private readonly Memory<char> _progressBuffer;

        private readonly IMemoryOwner<char> _percentageBufferOwner;

		private readonly string _emptyLine;

        private bool _disposed;

		/// <summary>
		/// Represents a progress bar that can be displayed in the console.
		/// </summary>
		public ProgressBar() {
            int length = ogConsole.BufferWidth - 10;
            _progressBufferOwner = Helper.ObtainMemory(length);
			_progressBuffer = _progressBufferOwner.Memory.Slice(0, length);
            _percentageBufferOwner = Helper.ObtainMemory(20);
            _emptyLine = new string(' ', ogConsole.BufferWidth);
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
			ogConsole.ForegroundColor = ForegroundColor;
			var currentLine = ogConsole.CursorTop;
			ogConsole.SetCursorPosition(0, currentLine);
			ogConsole.WriteLine(_emptyLine);
			ogConsole.WriteLine(_emptyLine);
			ogConsole.SetCursorPosition(0, currentLine);
			if (header.Length is not 0) {
				ogConsole.Out.WriteLine(header);
			}

			ogConsole.Write('[');
			var p = (int)(_progressBuffer.Length * percentage * 0.01);
			ogConsole.ForegroundColor = ProgressColor;
			Span<char> span = _progressBuffer.Span;
			Span<char> full = span.Slice(0, p);
			full.Fill(ProgressChar);
			Span<char> empty = span.Slice(p);
			empty.Fill(' ');
			ogConsole.Out.Write(full);
			ogConsole.Out.Write(empty);
			ogConsole.ForegroundColor = ForegroundColor;
			ogConsole.Write("] ");
			ogConsole.Out.Write(Helper.FormatPercentage(percentage, _percentageBufferOwner.Memory.Span));
			ogConsole.SetCursorPosition(0, currentLine);
			ResetColors();
		}

        /// <summary>
        /// Releases the resources held by this object
        /// </summary>
        public void Dispose() {
            if (Volatile.Read(ref _disposed)) {
                return;
            }
            _progressBufferOwner.Dispose();
            _percentageBufferOwner.Dispose();
            Volatile.Write(ref _disposed, true);
            GC.SuppressFinalize(this);
        }
    }
}