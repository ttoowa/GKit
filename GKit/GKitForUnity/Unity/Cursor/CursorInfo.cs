using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GKitForUnity {
	/// <summary>
	/// 이미지와 중심점을 포함한 커서 데이터 단위입니다.
	/// </summary>
	public class CursorInfo {
		public Texture2D image;
		public Vector2 hotspot;

		public CursorInfo(Texture2D image, Vector2 hotspot) {
			this.image = image;
			this.hotspot = hotspot;
		}
	}
}