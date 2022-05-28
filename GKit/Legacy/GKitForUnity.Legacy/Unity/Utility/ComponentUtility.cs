using System;
using System.Collections.Generic;
using UnityEngine;

namespace GKitForUnity {
	public static class ComponentUtility {
		/// <summary>
		/// GetComponent<T>() 를 줄인 함수입니다.
		/// </summary>
		public static T Get<T>(this Component component) {
			return component.GetComponent<T>();
		}
		/// <summary>
		/// GetComponent<T>() 를 줄인 함수입니다.
		/// </summary>
		public static T Get<T>(this GameObject gameObject) {
			return gameObject.GetComponent<T>();
		}

		private static GameObject[] staticObjects;
		public static void UpdateGameObjectList() {
			staticObjects = Resources.FindObjectsOfTypeAll<GameObject>();
		}
		public static GameObject FindGameObject(this string name, bool useFirstFrameList = true) {
			if (staticObjects == null || !useFirstFrameList) {
				staticObjects = Resources.FindObjectsOfTypeAll<GameObject>();
			}
			for (int i = 0; i < staticObjects.Length; ++i) {
				if (staticObjects[i].name == name) {
					return staticObjects[i];
				}
			}
			return null;
		}
		public static GameObject[] FindGameObjects(this string name, bool useFirstFrameList = true) {
			try {
				if (staticObjects == null || !useFirstFrameList) {
					staticObjects = Resources.FindObjectsOfTypeAll<GameObject>();
				}
				Queue<GameObject> objQueue = new Queue<GameObject>();
				for (int i = 0; i < staticObjects.Length; ++i) {
					if (staticObjects[i].name == name) {
						objQueue.Enqueue(staticObjects[i]);
					}
				}
				return objQueue.ToArray();
			} catch (Exception ex) {
				GDebug.Log($"Can't find gameObject'{name}'{Environment.NewLine}{ex.ToString()}", GLogLevel.Warnning);
				return null;
			}
		}
		public static GameObject FindGameObject(this GameObject gameObject, string name) {
			return FindGameObject(gameObject.transform, name);
		}
		public static GameObject FindGameObject(this Component component, string name) {
			return FindGameObject(component.transform, name);
		}
		public static GameObject FindGameObject(this Transform transform, string name) {
			try {
				int count = transform.childCount;
				for (int i = 0; i < count; ++i) {
					Transform child = transform.GetChild(i);
					if (child.name == name) {
						return child.gameObject;
					}
				}
			} catch (Exception ex) {
				GDebug.Log($"Can't find gameObject'{name}'{Environment.NewLine}{ex.ToString()}", GLogLevel.Warnning);
			}
			return null;
		}
	}
}
