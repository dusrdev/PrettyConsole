// ReSharper disable InconsistentNaming
using System.Runtime.InteropServices;

namespace PrettyConsole;

public readonly partial record struct Color {
	/// <summary>
	/// Represents the default color for the shell (changes based on platform)
	/// </summary>
	public static readonly ConsoleColor DefaultForegroundColor;

	/// <summary>
	/// Represents the default color for the shell (changes based on platform)
	/// </summary>
	public static readonly ConsoleColor DefaultBackgroundColor;

#if !OS_WINDOWS
	static Color() {
		const ConsoleColor unknown = (ConsoleColor)(-1);
		DefaultForegroundColor = unknown;
		DefaultBackgroundColor = unknown;
	}
#else
	static Color() {
		Kernel32.CONSOLE_SCREEN_BUFFER_INFO csbi = Kernel32.GetBufferInfo(false, out bool succeeded);

		if (succeeded) {
			DefaultForegroundColor = ColorAttributeToConsoleColor((Kernel32.Color)csbi.wAttributes & Kernel32.Color.ForegroundMask);
			DefaultBackgroundColor = ColorAttributeToConsoleColor((Kernel32.Color)csbi.wAttributes & Kernel32.Color.BackgroundMask);
		} else {
			DefaultForegroundColor = ConsoleColor.Gray;
			DefaultBackgroundColor = ConsoleColor.Black;
		}
	}

	private static ConsoleColor ColorAttributeToConsoleColor(Kernel32.Color c) {
		// Turn background colors into foreground colors.
		if ((c & Kernel32.Color.BackgroundMask) != 0) {
			c = (Kernel32.Color)(((int)c) >> 4);
		}
		return (ConsoleColor)c;
	}

	private static class Kernel32 {
		[StructLayout(LayoutKind.Sequential)]
		internal struct CONSOLE_SCREEN_BUFFER_INFO {
			internal COORD dwSize;
			internal COORD dwCursorPosition;
			internal short wAttributes;
			internal SMALL_RECT srWindow;
			internal COORD dwMaximumWindowSize;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal partial struct COORD {
			internal short X;
			internal short Y;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal partial struct SMALL_RECT {
			internal short Left;
			internal short Top;
			internal short Right;
			internal short Bottom;
		}

		internal enum Color : short {
			Black = 0,
			ForegroundBlue = 0x1,
			ForegroundGreen = 0x2,
			ForegroundRed = 0x4,
			ForegroundYellow = 0x6,
			ForegroundIntensity = 0x8,
			BackgroundBlue = 0x10,
			BackgroundGreen = 0x20,
			BackgroundRed = 0x40,
			BackgroundYellow = 0x60,
			BackgroundIntensity = 0x80,

			ForegroundMask = 0xf,
			BackgroundMask = 0xf0,
			ColorMask = 0xff
		}

		internal static CONSOLE_SCREEN_BUFFER_INFO GetBufferInfo(bool throwOnNoConsole, out bool succeeded) {
			IntPtr outputHandle = GetStdHandle(STD_OUTPUT_HANDLE);
			if (outputHandle == InvalidHandleValue) {
				if (throwOnNoConsole) {
					throw new IOException("No console found");
				}
				succeeded = false;
				return default;
			}

			// Note that if stdout is redirected to a file, the console handle may be a file.
			// First try stdout; if this fails, try stderr and then stdin.
			CONSOLE_SCREEN_BUFFER_INFO csbi;
			if (!GetConsoleScreenBufferInfo(outputHandle, out csbi) &&
				!GetConsoleScreenBufferInfo(GetStdHandle(STD_ERROR_HANDLE), out csbi) &&
				!GetConsoleScreenBufferInfo(GetStdHandle(STD_INPUT_HANDLE), out csbi)) {
				int errorCode = Marshal.GetLastPInvokeError();
				throw new IOException("Failed to read console screen buffer info", errorCode);
			}

			succeeded = true;
			return csbi;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetConsoleScreenBufferInfo(
			IntPtr hConsoleOutput,
			out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

		public const int STD_INPUT_HANDLE = -10;
		public const int STD_OUTPUT_HANDLE = -11;
		public const int STD_ERROR_HANDLE = -12;

		private static IntPtr InvalidHandleValue => new(-1);
	}
#endif
}