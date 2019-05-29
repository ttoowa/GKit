using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if OnUnity
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
#else
using Vector2 = GKit.Vector2;
using Vector3 = GKit.Vector3;
#endif

namespace GKit {
	public static class MathExtension {

		public static Vector2 GetNormal(this float radianAngle) {
			return new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));
		}
		public static float GetNearAngleDelta(this float delta) {
			delta = delta % 360f;
			if(delta < -180f) {
				delta += 360f;
			} else if(delta > 180f) {
				delta -= 360f;
			}
			return delta;
		}
		public static Vector2 GetNearAngleDelta(this Vector2 delta) {
			delta.x = delta.x.GetNearAngleDelta();
			delta.y = delta.y.GetNearAngleDelta();
			return delta;
		}
		public static Vector3 GetNearAngleDelta(this Vector3 delta) {
			delta.x = delta.x.GetNearAngleDelta();
			delta.y = delta.y.GetNearAngleDelta();
			delta.z = delta.z.GetNearAngleDelta();
			return delta;
		}
		public static double GetDelta(this double src, double dest, float speed) {
			return (dest - src) * speed;
		}
		public static float GetDelta(this float src, float dest, float speed) {
			return (dest - src) * speed;
		}
		public static Vector2 GetVector2(this float value) {
			return new Vector2(value, value);
		}
		public static Vector3 GetVector3(this float value) {
			return new Vector3(value, value, value);
		}
	}
}
