using UnityEngine;

namespace GKitForUnity {
	public class FpsInitializer : MonoBehaviour {
		public int targetFps = 60;

		private void Start() {
			Application.targetFrameRate = targetFps;
		}
	}
}
