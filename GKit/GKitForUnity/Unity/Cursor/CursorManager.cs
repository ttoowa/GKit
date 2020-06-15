using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GKitForUnity {
	/// <summary>
	/// 커서 이미지를 관리하는 클래스입니다.
	/// </summary>
	public static class CursorManager {
		public static void SetCursor(CursorInfo cursor) {
			Texture2D image = null;
			Vector2 hotspot = Vector2.zero;
			if(cursor != null) {
				image = cursor.image;
				hotspot = new Vector2(image.width * hotspot.x, image.height * hotspot.y);
			}
			Cursor.SetCursor(image, hotspot, CursorMode.Auto);
		}
	}
}