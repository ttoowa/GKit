using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if OnUnity
using UnityEngine;
using Debug = UnityEngine.Debug;
#endif

namespace GKit {
	public delegate void Arg1Delegate<ArgT>(ArgT value);
	public delegate void Arg2Delegate<ArgT1, ArgT2>(ArgT1 value1, ArgT2 value2);
	public delegate void Arg3Delegate<ArgT1, ArgT2, ArgT3>(ArgT1 value1, ArgT2 value2, ArgT3 value3);
	public delegate void Arg3Delegate<ArgT1, ArgT2, ArgT3, ArgT4>(ArgT1 value1, ArgT2 value2, ArgT3 value3, ArgT4 value4);
	public delegate ReturnT ReturnDelegate<ReturnT>();
	public delegate ReturnT Arg1ReturnDelegate<ReturnT, ArgT>(ArgT value);
	public delegate ReturnT Arg2ReturnDelegate<ReturnT, ArgT1, ArgT2>(ArgT1 value1, ArgT2 value2);
	public delegate ReturnT Arg2ReturnDelegate<ReturnT, ArgT1, ArgT2, ArgT3>(ArgT1 value1, ArgT2 value2, ArgT3 value3);
	public delegate ReturnT Arg2ReturnDelegate<ReturnT, ArgT1, ArgT2, ArgT3, ArgT4>(ArgT1 value1, ArgT2 value2, ArgT3 value3, ArgT4 value4);

	public static class SystemExtension {
		public static float GetElapsedMilliseconds(this Stopwatch stopwatch) {
			return stopwatch.ElapsedTicks / (float)Stopwatch.Frequency * 1000f;
		}
		/// <summary>
		/// 함수를 호출하며 예외를 검사합니다.
		/// </summary>
		public static bool TryInvoke(this Action action) {
			try {
				action?.Invoke();
				return true;
			} catch(Exception ex) {
				GDebug.Log(ex.ToString(), GLogLevel.Warnning);
				return false;
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

		public static T Cast<T> (this object obj) {
			return (T)obj;
		}

		public static bool HasTrueValue(this bool? value) {
			return value.HasValue && value.Value;
		}
	}
}
