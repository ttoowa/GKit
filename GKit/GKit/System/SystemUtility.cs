using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY
using UnityEngine;
using Debug = UnityEngine.Debug;
#endif

namespace GKit {
	public delegate void SingleDelegate<T>(T value);
	public delegate T ReturnDelegate<T>();
	public delegate T2 DuplexDelegate<T1, T2>(T1 value);

	public static class SystemExtension {
		public static float GetElapsedMilliseconds(this Stopwatch stopwatch) {
			return stopwatch.ElapsedTicks / (float)Stopwatch.Frequency * 1000f;
		}
		/// <summary>
		/// 함수를 호출하며 예외를 검사합니다.
		/// </summary>
		public static void SafeInvoke(this Action action) {
			try {
				action?.Invoke();
			} catch(Exception ex) {
				ex.ToString().Log();
			}
		}

		public static int Parse2Int(this string value, int exceptionValue = 0) {
			int result;
			if (int.TryParse(value, out result)) {
				return result;
			} else {
				return 0;
			}
		}
		public static float Parse2Float(this string value, float exceptionValue = 0f) {
			float result;
			if (float.TryParse(value, out result)) {
				return result;
			} else {
				return 0;
			}
		}

		public static bool Contained(this float value, float min, float max) {
			return value <= max && value >= min;
		}
		public static bool Contained(this int value, int min, int max) {
			return value <= max && value >= min;
		}
	}
}
