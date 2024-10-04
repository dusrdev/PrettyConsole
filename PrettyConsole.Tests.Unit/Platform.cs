// using System.Runtime.InteropServices;

// namespace PrettyConsole.Tests.Unit;

// public class Platform {
// 	[Fact]
// 	public void CheckConditionalCompilation() {
// 		#if Windows
// 		string str = "Windows";
// 		#else
// 		string str = "Unix";
// 		#endif

// 		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
// 			str.Should().Be("Windows");
// 		} else {
// 			str.Should().Be("Unix");
// 		}
// 	}
// }