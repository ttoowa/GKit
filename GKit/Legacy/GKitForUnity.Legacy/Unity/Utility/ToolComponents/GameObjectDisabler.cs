#if OnUnity
using UnityEngine;

namespace GKitForUnity {
	public class GameObjectDisabler : MonoBehaviour {
		private void Start() {
			gameObject.SetActive(false);
		}
	}
}
#endif