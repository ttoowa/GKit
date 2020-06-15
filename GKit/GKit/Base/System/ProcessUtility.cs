using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	/// <summary>
	/// 동일한 프로세스가 이미 있는지 확인합니다.
	/// </summary>
	public static class ProcessUtility {

		[DllImport("user32.dll")]
		private static extern int FindWindow(string lpClassName, string lpWindowName);

		public static bool CheckSingleInstance() {
			Process[] processArray = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
			int currentID = Process.GetCurrentProcess().Id;

			for (int i = 0; i < processArray.Length; i++) {
				if (processArray[i].Id != currentID) {
					IntPtr handle = new IntPtr(FindWindow(null, "BigPicture"));
					return false;
				}
			}
			return true;
		}

		public static bool Start(string path, string args = null) {
			try {
				Process process = new Process();
				process.StartInfo.FileName = path;
				if (!string.IsNullOrEmpty(args)) {
					process.StartInfo.Arguments = args;
				}
				process.Start();
				return true;
			} catch (Exception ex) {
				GDebug.Log(ex.ToString(), GLogLevel.Warnning);
				return false;
			}
		}
	}
}