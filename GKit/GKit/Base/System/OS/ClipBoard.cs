#if OnUnity
using UnityEngine;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
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
