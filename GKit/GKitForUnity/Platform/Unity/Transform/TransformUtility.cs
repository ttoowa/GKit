extern alias CoreModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using RectTransform = CoreModule::UnityEngine.RectTransform;

namespace GKit.Unity {
	public static class TransformUtility {
		//Node
		public static void SetParent(this GameObject child, GameObject parent, bool worldPositionStays = false) {
			if (parent == null) {
				child.transform.parent = null;
			}
			else {
				child.transform.SetParent(parent.transform, worldPositionStays);
			}
		}
		public static void SetParent(this GameObject child, Transform parent, bool worldPositionStays = false) {
			if (parent == null) {
				child.transform.parent = null;
			}
			else {
				child.transform.SetParent(parent, worldPositionStays);
			}
		}
		public static void AddChild(this GameObject parent, GameObject child, bool worldPositionStays = false) {
			child.SetParent(parent, worldPositionStays);
		}
		public static void AddChild(this GameObject parent, Transform child, bool worldPositionStays = false) {
			child.SetParent(parent.transform, worldPositionStays);
		}
		public static void DetachChilds(this GameObject gameObject) {
			gameObject.transform.DetachChildren();
		}
		public static void ClearChilds(this GameObject gameObject) {
			int childCount = gameObject.transform.childCount;
			for (int i = childCount - 1; i >= 0; --i) {
				GameObject.Destroy(gameObject.transform.GetChild(i));
			}
		}

		//Position
		public static void SetLocalX(this GameObject gameObject, float x) {
			Vector3 localPos = gameObject.transform.localPosition;
			gameObject.transform.localPosition = new Vector3(x, localPos.y, localPos.z);
		}
		public static void SetLocalY(this GameObject gameObject, float y) {
			Vector3 localPos = gameObject.transform.localPosition;
			gameObject.transform.localPosition = new Vector3(localPos.x, y, localPos.z);
		}
		public static void SetLocalZ(this GameObject gameObject, float z) {
			Vector3 localPos = gameObject.transform.localPosition;
			gameObject.transform.localPosition = new Vector3(localPos.x, localPos.y, z);
		}
		public static void SetLocalXY(this GameObject gameObject, float x, float y) {
			gameObject.transform.localPosition = new Vector3(x, y, gameObject.transform.localPosition.z);
		}
		public static void SetLocalXYZ(this GameObject gameObject, float x, float y, float z) {
			gameObject.transform.localPosition = new Vector3(x, y, z);
		}
		public static float GetLocalX(this GameObject gameObject) {
			return gameObject.transform.localPosition.x;
		}
		public static float GetLocalY(this GameObject gameObject) {
			return gameObject.transform.localPosition.y;
		}
		public static float GetLocalZ(this GameObject gameObject) {
			return gameObject.transform.localPosition.z;
		}
		public static Vector2 GetLocalXY(this GameObject gameObject) {
			return (Vector2)gameObject.transform.localPosition;
		}

		public static void SetLocalX(this Component component, float x) {
			Vector3 localPos = component.transform.localPosition;
			component.transform.localPosition = new Vector3(x, localPos.y, localPos.z);
		}
		public static void SetLocalY(this Component component, float y) {
			Vector3 localPos = component.transform.localPosition;
			component.transform.localPosition = new Vector3(localPos.x, y, localPos.z);
		}
		public static void SetLocalZ(this Component component, float z) {
			Vector3 localPos = component.transform.localPosition;
			component.transform.localPosition = new Vector3(localPos.x, localPos.y, z);
		}
		public static void SetLocalXY(this Component component, float x, float y) {
			component.transform.localPosition = new Vector3(x, y, component.transform.localPosition.z);
		}
		public static void SetLocalXYZ(this Component component, float x, float y, float z) {
			component.transform.localPosition = new Vector3(x, y, z);
		}
		public static float GetLocalX(this Component component) {
			return component.transform.localPosition.x;
		}
		public static float GetLocalY(this Component component) {
			return component.transform.localPosition.y;
		}
		public static float GetLocalZ(this Component component) {
			return component.transform.localPosition.z;
		}
		public static Vector2 GetLocalXY(this Component component) {
			return (Vector2)component.transform.localPosition;
		}

		public static void SetLocalX(this Transform transform, float x) {
			transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
		}
		public static void SetLocalY(this Transform transform, float y) {
			transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
		}
		public static void SetLocalZ(this Transform transform, float z) {
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
		}
		public static void SetLocalXY(this Transform transform, float x, float y) {
			transform.localPosition = new Vector3(x, y, transform.localPosition.z);
		}
		public static void SetLocalXYZ(this Transform transform, float x, float y, float z) {
			transform.localPosition = new Vector3(x, y, z);
		}
		public static float GetLocalX(this Transform transform) {
			return transform.localPosition.x;
		}
		public static float GetLocalY(this Transform transform) {
			return transform.localPosition.y;
		}
		public static float GetLocalZ(this Transform transform) {
			return transform.localPosition.z;
		}
		public static Vector2 GetLocalXY(this Transform transform) {
			return (Vector2)transform.localPosition;
		}
	}
}
