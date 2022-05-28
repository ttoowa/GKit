using System;
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
	public static class SearchUtility {
		public static int BinarySearch(Func<int, int> keyToValueFunc, int targetValue, int lower, int upper, int failedReturnKey = 0, uint tolerance = 0, uint maxLoopCount = 0) {
			int loopCount = 0;
			int mid = lower;
			int result;
			for(; ;) {
				if (maxLoopCount > 0 && ++loopCount > maxLoopCount)
					break;

				//Calc mid
				mid = (upper + lower) / 2;
				result = keyToValueFunc(mid);

				if (lower > upper)
					return failedReturnKey;
				if (Mathf.Abs(targetValue - result) <= tolerance)
					break;

				if (result < targetValue) {
					lower = mid + 1;
				} else {
					upper = mid - 1;
				}
			}
			return mid;
		}
		public static int BinarySearch(Func<int, float> keyToValueFunc, float targetValue, int lower, int upper, int failedReturnKey = 0, float tolerance = 0, uint maxLoopCount = 0) {
			int loopCount = 0;
			int mid = lower;
			float result;
			for (; ; ) {
				if (maxLoopCount > 0 && ++loopCount > maxLoopCount)
					break;

				//Calc mid
				mid = (upper + lower) / 2;
				result = keyToValueFunc(mid);

				if (lower > upper)
					return failedReturnKey;
				if (Mathf.Abs(targetValue - result) <= tolerance)
					break;

				if (result < targetValue) {
					lower = mid + 1;
				} else {
					upper = mid - 1;
				}
			}
			return mid;
		}
		public static float BinarySearch(Func<float, float> keyToValueFunc, float targetValue, float lower, float upper, float tolerance = 0.001f, uint maxLoopCount = 0) {
			int loopCount = 0;
			float mid = lower;
			float result;
			for (; ; ) {
				if (maxLoopCount > 0 && ++loopCount > maxLoopCount)
					break;

				//Calc mid
				mid = (upper + lower) * 0.5f;
				result = keyToValueFunc(mid);

				if (Mathf.Abs(targetValue - result) <= tolerance)
					break;
				
				if (result < targetValue) {
					lower = mid;
				} else {
					upper = mid;
				}
			}
			return mid;
		}
	}
}
