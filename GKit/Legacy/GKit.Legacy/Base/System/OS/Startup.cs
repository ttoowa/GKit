﻿using System;
using Microsoft.Win32;
#if OnUnity
using GKitForUnity.IO;
#elif OnWPF
using GKitForWPF.IO;
#else
using GKit.IO;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public static class Startup {
		private const string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
		public static void Set(string KeyName, bool enable, string arg = null) {
			try {
				RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(runKey, true);

				if (enable) {
					Set(KeyName, false);
					string value = "\"" + IOUtility.AppFileInfo.FullName.Replace('/', '\\') + "\" ";
					if (!string.IsNullOrEmpty(arg)) {
						value += arg;
					}
					startupKey.SetValue(KeyName, value);
					startupKey.Close();
				} else {
					startupKey.DeleteValue(KeyName, false);
					startupKey.Close();
				}
			} catch (Exception ex) {
				GDebug.Log(ex.ToString(), GLogLevel.Warnning);
			}
		}
		public static bool Get(string KeyName) {
			try {
				RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(runKey, true);

				return startupKey.GetValue(KeyName) != null;
			} catch (Exception ex) {
				GDebug.Log(ex.ToString(), GLogLevel.Warnning);
			}
			return false;
		}
	}
}
