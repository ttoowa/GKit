using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using GKit;

namespace GKit {
	public static class Startup {
		private const string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
		public static void Set(string KeyName, bool enable, string arg = null) {
			try {
				RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(runKey, true);

				if (enable) {
					Set(KeyName, false);
					string value = "\"" + IOUtility.AppFileInfo.FullName.Replace('/', '\\') + "\" ";
					if(!string.IsNullOrEmpty(arg)) {
						value += arg;
					}
					startupKey.SetValue(KeyName, value);
					startupKey.Close();
				} else {
					startupKey.DeleteValue(KeyName, false);
					startupKey.Close();
				}
			} catch(Exception ex) {
				ex.ToString().Log();
			}
		}
		public static bool Get(string KeyName) {
			try {
				RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(runKey, true);

				return startupKey.GetValue(KeyName) != null;
			} catch (Exception ex) {
				ex.ToString().Log();
			}
			return false;
		}
	}
}
