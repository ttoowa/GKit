using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
#if OnUnity
using UnityEngine;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	/// <summary>
	/// 1D 스플라인 곡선 함수를 제공하는 클래스입니다.
	/// </summary>
	public static class GSpline1D {
		public static float Bezier2(float t, float p0, float p1, float p2) {
			float t2 = t * t;
			float tInv = 1 - t;
			float tInv2 = tInv * tInv;
			float pos = (tInv2 * p0) + (2f * t * tInv * p1) + (t2 * p2);
			return pos;
		}
		public static float Bezier3(float t, float p0, float p1, float p2, float p3) {
			float t2 = t * t;
			float t3 = t2 * t;
			float tInv = 1 - t;
			float tInv2 = tInv * tInv;
			float tInv3 = tInv2 * tInv;

			float pos = tInv3 * p0;
			pos += 3 * tInv2 * t * p1;
			pos += 3 * tInv * t2 * p2;
			pos += t3 * p3;

			return pos;
		}
		public static float Bezier4(float t, float p0, float p1, float p2, float p3, float p4, float p5) {
			float t2 = t * t;
			float t3 = t2 * t;
			float t4 = t3 * t;
			float t5 = t4 * t;
			float tInv = 1 - t;
			float tInv2 = tInv * tInv;
			float tInv3 = tInv2 * tInv;
			float tInv4 = tInv3 * tInv;
			float tInv5 = tInv4 * tInv;

			float pos = p0 * tInv5 +
				5f * p1 * t * tInv4 +
				10f * p2 * t2 * tInv3 +
				10f * p3 * t3 * tInv2 +
				5f * p4 * t4 * tInv +
				p5 * t5;

			return pos;
		}
		public static float CatmullRom(float t, float prevAnchor, float prev, float next, float nextAnchor) {
			float t2 = t * t;
			float t3 = t2 * t;
			float a = 2.0f * prev;
			float b = next - prevAnchor;
			float c = 2.0f * prevAnchor - 5.0f * prev + 4.0f * next - nextAnchor;
			float d = prevAnchor * -1.0f + 3.0f * prev - 3.0f * next + nextAnchor;

			//The cubic polynomial: a + b * t + c * t^2 + d * t^3
			float pos = 0.5f * (a + (b * t) + (c * t2) + (d * t3));

			return pos;
		}
	}
	/// <summary>
	/// 2D 스플라인 곡선 함수를 제공하는 클래스입니다.
	/// </summary>
	public static class GSpline2D {
		public static Vector2 Bezier2(float t, Vector2 p0, Vector2 p1, Vector2 p2) {
			float t2 = t * t;
			float tInv = 1 - t;
			float tInv2 = tInv * tInv;
			Vector2 pos = (tInv2 * p0) + (2f * t * tInv * p1) + (t2 * p2);
			return pos;
		}
		public static Vector2 Bezier3(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) {
			float t2 = t * t;
			float t3 = t2 * t;
			float tInv = 1 - t;
			float tInv2 = tInv * tInv;
			float tInv3 = tInv2 * tInv;

			Vector2 pos = tInv3 * p0;
			pos += 3 * tInv2 * t * p1;
			pos += 3 * tInv * t2 * p2;
			pos += t3 * p3;

			return pos;
		}
		public static float Bezier3_X2Y(float x, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, int maxLoopCount = 10) {
			x = Mathf.Clamp01(x);
			float xTolerance = 0.001f;

			float lower = 0f;
			float upper = 1f;
			float percent = (upper + lower) * 0.5f;

			int loopCount = 0;
			Vector2 result = Bezier3(percent, p0, p1, p2, p3);
			while (Mathf.Abs(x - result.x) > xTolerance) {
				if (++loopCount > maxLoopCount) {
					break;
				}
				if (x > result.x) {
					lower = percent;
				} else {
					upper = percent;
				}
				percent = (upper + lower) * 0.5f;

				result = Bezier3(percent, p0, p1, p2, p3);
			}
			return result.y;
		}
		public static Vector2 Bezier4(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 p5) {
			float t2 = t * t;
			float t3 = t2 * t;
			float t4 = t3 * t;
			float t5 = t4 * t;
			float tInv = 1 - t;
			float tInv2 = tInv * tInv;
			float tInv3 = tInv2 * tInv;
			float tInv4 = tInv3 * tInv;
			float tInv5 = tInv4 * tInv;

			Vector2 pos = p0 * tInv5 +
				5f * p1 * t * tInv4 +
				10f * p2 * t2 * tInv3 +
				10f * p3 * t3 * tInv2 +
				5f * p4 * t4 * tInv +
				p5 * t5;

			return pos;
		}
		public static Vector2 CatmullRom(float t, Vector2 prevAnchor, Vector2 prev, Vector2 next, Vector2 nextAnchor) {
			float t2 = t * t;
			float t3 = t2 * t;
			Vector2 a = 2.0f * prev;
			Vector2 b = next - prevAnchor;
			Vector2 c = 2.0f * prevAnchor - 5.0f * prev + 4.0f * next - nextAnchor;
			Vector2 d = prevAnchor * -1.0f + 3.0f * prev - 3.0f * next + nextAnchor;

			//The cubic polynomial: a + b * t + c * t^2 + d * t^3
			Vector2 pos = 0.5f * (a + (b * t) + (c * t2) + (d * t3));

			return pos;
		}
	}
	/// <summary>
	/// 3D 스플라인 곡선 함수를 제공하는 클래스입니다.
	/// </summary>
	public static class GSpline3D {
		public static Vector3 Bezier2(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
			float t2 = t * t;
			float tInv = 1 - t;
			float tInv2 = tInv * tInv;
			Vector3 pos = (tInv2 * p0) + (2f * t * tInv * p1) + (t2 * p2);
			return pos;
		}
		public static Vector3 Bezier3(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
			float t2 = t * t;
			float t3 = t2 * t;
			float tInv = 1 - t;
			float tInv2 = tInv * tInv;
			float tInv3 = tInv2 * tInv;

			Vector3 pos = tInv3 * p0;
			pos += 3 * tInv2 * t * p1;
			pos += 3 * tInv * t2 * p2;
			pos += t3 * p3;

			return pos;
		}
		public static Vector3 Bezier4(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 p5) {
			float t2 = t * t;
			float t3 = t2 * t;
			float t4 = t3 * t;
			float t5 = t4 * t;
			float tInv = 1 - t;
			float tInv2 = tInv * tInv;
			float tInv3 = tInv2 * tInv;
			float tInv4 = tInv3 * tInv;
			float tInv5 = tInv4 * tInv;

			Vector3 pos = p0 * tInv5 +
				5f * p1 * t * tInv4 +
				10f * p2 * t2 * tInv3 +
				10f * p3 * t3 * tInv2 +
				5f * p4 * t4 * tInv +
				p5 * t5;

			return pos;
		}
		public static Vector3 CatmullRom(float t, Vector3 prevAnchor, Vector3 prev, Vector3 next, Vector3 nextAnchor) {
			float t2 = t * t;
			float t3 = t2 * t;
			Vector3 a = 2.0f * prev;
			Vector3 b = next - prevAnchor;
			Vector3 c = 2.0f * prevAnchor - 5.0f * prev + 4.0f * next - nextAnchor;
			Vector3 d = prevAnchor * -1.0f + 3.0f * prev - 3.0f * next + nextAnchor;

			//The cubic polynomial: a + b * t + c * t^2 + d * t^3
			Vector3 pos = 0.5f * (a + (b * t) + (c * t2) + (d * t3));

			return pos;
		}
	}
}
