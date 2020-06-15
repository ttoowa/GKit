#if OnUnity
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace GKitForUnity {
	public static class UnityUtility {
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

		//Camera
		public static Vector3 GetProjectionPos(this Camera cam, Vector2 screenPos, float worldZPos) {
			const float Factor = 1f;

			Vector3 camPos = cam.transform.position;

			Vector3 frontPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Factor));
			Vector3 vector = frontPos - camPos; //단위벡터

			float length = Mathf.Abs(worldZPos - camPos.z) / vector.z;

			return camPos + vector * length;
		}

		//Resource
		public static T GetResource<T>(this string path) where T : UnityEngine.Object {
			return GResourceUtility.Get<T>(path);
		}
		public static Texture2D GetTexture(this string localPath, TextureFormat format = TextureFormat.RGBA32, bool generateMipmap = true) {
			if (File.Exists(localPath)) {
				byte[] binary = File.ReadAllBytes(localPath);
				Texture2D tex = new Texture2D(2, 2, format, generateMipmap);
				tex.LoadImage(binary);
				return tex;
			}
			return null;
		}

		//Management
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

		//Text
		public static void SetMultilineText(this TextMesh textMesh, string text, float maxWidth) {
			textMesh.text = text;

			if (string.IsNullOrEmpty(text)) {
				return;
			}

			StringBuilder builder = new StringBuilder();
			CharacterInfo charInfo = new CharacterInfo();
			char[] textArr = text.ToCharArray();
			float widthStack = 0f;
			for (int i = 0; i < textArr.Length; ++i) {
				char character = textArr[i];
				if (character == '\n') {
					widthStack = 0f;
				} else {
					textMesh.font.GetCharacterInfo(character, out charInfo, textMesh.fontSize, textMesh.fontStyle);

					float advance = charInfo.advance * textMesh.characterSize * 0.1f;
					widthStack += advance;

					if (widthStack >= maxWidth) {
						widthStack = advance;
						builder.Append(Environment.NewLine);
					}
				}
				builder.Append(textArr[i]);
			}
			textMesh.text = builder.ToString();
		}
		public static float GetAdvance(this char character, TextMesh refTextMesh) {
			CharacterInfo charInfo = new CharacterInfo();
			refTextMesh.font.GetCharacterInfo(character, out charInfo, refTextMesh.fontSize, refTextMesh.fontStyle);
			float advance = charInfo.advance * refTextMesh.characterSize * 0.1f;
			return advance;
		}
	}
}

#endif