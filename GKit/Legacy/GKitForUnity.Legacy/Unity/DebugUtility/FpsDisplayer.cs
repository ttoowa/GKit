#if OnUnity
using UnityEngine;

namespace GKitForUnity {
	public class FpsDisplayer : MonoBehaviour {
		public Font font;
		public Color textColor = new Color(243f / 255f, 203f / 255f, 34f / 255f, 1f);
		public int fontSize = 16;
		private float deltaTime;

		private void Update() {
			deltaTime = Time.unscaledDeltaTime;
		}
		private void OnGUI() {
			int w = Screen.width, h = Screen.height;

			GUIStyle style = new GUIStyle();

			Rect rect = new Rect(0, 0, w, h * 2 / 100);
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = fontSize;
			style.normal.textColor = textColor;
			style.font = font;
			float fps = 1.0f / deltaTime;
			string text = $"{fps.ToString("0.0")} FPS";
			GUI.Label(rect, text, style);
		}
	}
}
#endif