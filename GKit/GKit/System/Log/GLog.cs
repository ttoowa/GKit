using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY
using UnityEngine;
using Debug = UnityEngine.Debug;
#elif WPF
#endif

namespace GKit {
	public static class GDebug {

		public static void Log(this string text, GLogLevel logLevel = 0) {
			switch (logLevel) {
				case GLogLevel.Log:
					LogPlatform("GLog_Log ::\n" + text);
					break;
				case GLogLevel.Warnning:
					LogPlatform("GLog_Warning ::\n" + text);
					break;
				case GLogLevel.Error:
					LogPlatform("GLog_Error ::\n" + text);
					throw new Exception("GLog_Error ::\n" + text);
			}
		}
		private static void LogPlatform(string text) {
#if UNITY
			Debug.Log(
#elif WPF
					Debug.WriteLine(
#endif
						text);
		}
	}
}
