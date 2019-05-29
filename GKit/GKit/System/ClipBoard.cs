using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if OnUnity
using UnityEngine;
#endif

namespace GKit {
	/// <summary>
	/// 클립보드 관리 클래스입니다.
	/// </summary>
	public static class Clipboard {
		public static void SetText(string text) {
#if OnUnity
			GUIUtility.systemCopyBuffer = text;
#else
			System.Windows.Clipboard.SetText(text);
#endif
		}
		public static string GetText() {
#if OnUnity
			return GUIUtility.systemCopyBuffer;
#else
			return System.Windows.Clipboard.GetText();
#endif
		}
	}
}
