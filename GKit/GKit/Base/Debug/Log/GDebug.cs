using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
#if OnUnity
using UnityEngine;
using Debug = UnityEngine.Debug;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public static class GDebug {

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();

		private static bool onConsole;

		static GDebug() {
			onConsole = GetConsoleWindow() != IntPtr.Zero;
		}
		public static bool LogMode {
			get; set;
		} = true;
		public static void Log(object text, GLogLevel logLevel = 0) {
			if (!LogMode)
				return;
			switch (logLevel) {
				default:
				case GLogLevel.Log:
					LogPlatform($"GDebug_Log :: {text}");
					break;
				case GLogLevel.Warnning:
					LogPlatform($"GDebug_Warning :: {text}");
					break;
				case GLogLevel.Error:
					LogPlatform($"GDebug_Error :: {text}");
					throw new Exception($"GLog_Error :: {text}");
			}
		}
		private static void LogPlatform(string text) {
#if OnUnity
			Debug.Log(
#else
			if(onConsole) {
				Console.WriteLine(text);
			}
			Debug.WriteLine(
#endif
						text);
		}
	}
}