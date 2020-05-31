using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

namespace GKit {
	/// <summary>
	/// 동일한 프로세스가 이미 있는지 확인합니다.
	/// </summary>
	public static class ProcessChecker {
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
	}
}