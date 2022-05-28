#if OnUnity
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace GKitForUnity {
	public static class UnityUtility {
		//Renderer
		public static float GetBoundX(this Component component) {
			return component.Get<Renderer>().bounds.size.x;
		}
		public static float GetBoundY(this Component component) {
			return component.Get<Renderer>().bounds.size.y;
		}
		public static Vector2 GetBoundSize(this Component component) {
			return component.Get<Renderer>().bounds.size;
		}

		public static float GetBoundX(this GameObject gameObject) {
			return gameObject.Get<Renderer>().bounds.size.x;
		}
		public static float GetBoundY(this GameObject gameObject) {
			return gameObject.Get<Renderer>().bounds.size.y;
		}
		public static Vector2 GetBoundSize(this GameObject gameObject) {
			return gameObject.Get<Renderer>().bounds.size;
		}
	}
}

#endif