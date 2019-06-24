#if OnUnity
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GKit.Unity {
	public class GameObjectDisabler : MonoBehaviour {
		private void Start() {
			gameObject.SetActive(false);
		}
	}
}
#endif