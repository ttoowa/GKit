using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY
using UnityEngine;
#endif

namespace GKit {
	/// <summary>
	/// 클립보드 관리 클래스입니다.
	/// </summary>
	public static class Clipboard {
		public static void SetText(string text) {
#if UNITY
			GUIUtility.systemCopyBuffer = text;
#elif WPF
			System.Windows.Clipboard.SetText(text);
#endif
		}
		public static string GetText() {
#if UNITY
			return GUIUtility.systemCopyBuffer;
#elif WPF
			return System.Windows.Clipboard.GetText();
#endif
		}
	}
}
