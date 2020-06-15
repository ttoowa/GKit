using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	/// 자주 사용되는 산술 함수를 제공하는 클래스입니다.
	/// </summary>
	public static class GMath {
		public const float Float2Byte = 255f;
		public const float Byte2Float = 0.0039215686274f;


		public static float Diagonal(float x, float y) {
            return Mathf.Sqrt(DiagonalPow(x, y));
        }
        public static float DiagonalPow(float x, float y) {
            return x * x + y * y;
        }
		public static double Clamp(double value, double min, double max) {
			if(value < min) {
				return min;
			} else if(value > max) {
				return max;
			} else {
				return value;
			}
		}
		public static float Clamp(float value, float min, float max) {
			if (value < min) {
				return min;
			} else if (value > max) {
				return max;
			} else {
				return value;
			}
		}
		public static int Clamp(int value, int min, int max) {
			if (value < min) {
				return min;
			} else if (value > max) {
				return max;
			} else {
				return value;
			}
		}

		public static float Dot(Vector2 left, Vector2 right) {
			return left.x * right.x + left.y * right.y;
		}
		public static float Cross(this Vector2 left, Vector2 right) {
			return left.x * right.y - left.y * right.x;
		}
		public static Vector2 Cross(this Vector2 value, float scalar) {
			return new Vector2(value.y * scalar, value.x * -scalar);
		}
		public static Vector2 Max(this Vector2 value, float max) {
			return new Vector2(
				Mathf.Max(value.x, max),
				Mathf.Max(value.y, max));
		}
		public static Vector2 Max(this Vector2 value, Vector2 max) {
			return new Vector2(
				Mathf.Max(value.x, max.x),
				Mathf.Max(value.y, max.y));
		}
		public static Vector2 Min(this Vector2 value, float min) {
			return new Vector2(
				Mathf.Min(value.x, min),
				Mathf.Min(value.y, min));
		}
		public static Vector2 Min(this Vector2 value, Vector2 min) {
			return new Vector2(
				Mathf.Min(value.x, min.x),
				Mathf.Min(value.y, min.y));
		}
		public static Vector2 Clamp(this Vector2 value, float min, float max) {
			return new Vector2(
				Mathf.Clamp(value.x, min, max),
				Mathf.Clamp(value.y, min, max));
		}
		public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max) {
			return new Vector2(
				Mathf.Clamp(value.x, min.x, max.x),
				Mathf.Clamp(value.y, min.y, max.y));
		}
		public static Vector3 Clamp(this Vector3 value, float min, float max) {
			return new Vector3(
				Mathf.Clamp(value.x, min, max),
				Mathf.Clamp(value.y, min, max),
				Mathf.Clamp(value.z, min, max));
		}
		public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max) {
			return new Vector3(
				Mathf.Clamp(value.x, min.x, max.x),
				Mathf.Clamp(value.y, min.y, max.y),
				Mathf.Clamp(value.z, min.z, max.z));
		}
		public static Vector2 ToVector2(this string text) {
			text = text.Trim();
			string[] nums = text.Split(',');

			return new Vector2(
				float.Parse(nums[0], CultureInfo.InvariantCulture), 
				float.Parse(nums[1], CultureInfo.InvariantCulture));
		}
		public static Vector3 ToVector3(this string text) {
			text = text.Trim();
			string[] nums = text.Split(',');

			return new Vector3(
				float.Parse(nums[0], CultureInfo.InvariantCulture), 
				float.Parse(nums[1], CultureInfo.InvariantCulture),
				float.Parse(nums[2], CultureInfo.InvariantCulture));
		}
		public static Vector2Int ToVector2Int(this Vector2 value) {
			return new Vector2Int((int)value.x, (int)value.y);
		}

		public static string Base10ToBaseN(long value, string baseNumbers) {
			StringBuilder builder = new StringBuilder();
			int baseLength = baseNumbers.Length;
			do {
				builder.Insert(0, baseNumbers[(int)(value % baseLength)]);
				value /= baseLength;
			}
			while (value > 0);
			return builder.ToString();
		}
		public static long BaseNToBase10(string value, string baseNumbers) {
			long result = 0;
			int baseLength = baseNumbers.Length;
			for (int i = 0; i < value.Length; ++i) {
				result *= baseLength;
				result += baseNumbers.IndexOf(value[i]);
			}
			return result;
		}

		public static float Sigmoid(float value) {
			return 1f / (1f + Mathf.Exp(-value));
		}
		public static float SigmoidDifferential(float value) {
			float sig = Sigmoid(value);
			return sig * (1f - sig);
		}
	}
}
