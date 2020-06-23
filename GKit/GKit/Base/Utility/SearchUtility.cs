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
		public static int BinarySearch(Func<int, int> function, int targetValue, int lower, int upper, int failedReturnKey = 0, int tolerance = 0, int maxLoopCount = 0) {
			int loopCount = 0;
			int mid;
			int result;
			for(; ;) {
				mid = (upper + lower) / 2;
				result = function(mid);

				if (lower > upper)
					return failedReturnKey;
				if (Mathf.Abs(targetValue - result) <= tolerance)
					break;
				if (maxLoopCount > 0 && ++loopCount > maxLoopCount) {
					break;
				}

				if(result == targetValue) {
					break;
				} else if (result < targetValue) {
					lower = mid + 1;
				} else {
					upper = mid - 1;
				}
			}
			return mid;
		}
		public static int BinarySearch(Func<int, float> function, float targetValue, int lower, int upper, int failedReturnKey = 0, float tolerance = 0, int maxLoopCount = 0) {
			int loopCount = 0;
			int mid;
			float result;
			for (; ; ) {
				mid = (upper + lower) / 2;
				result = function(mid);

				if (lower > upper)
					return failedReturnKey;
				if (Mathf.Abs(targetValue - result) <= tolerance)
					break;
				if (maxLoopCount > 0 && ++loopCount > maxLoopCount) {
					break;
				}

				if (result < targetValue) {
					lower = mid + 1;
				} else {
					upper = mid - 1;
				}
			}
			return mid;
		}
		public static float BinarySearch(Func<float, float> function, float targetValue, float lower, float upper, float tolerance = 0.001f, int maxLoopCount = 0) {
			int loopCount = 0;
			float mid;
			float result;
			for (; ; ) {
				mid = (upper + lower) * 0.5f;
				result = function(mid);

				if (Mathf.Abs(targetValue - result) <= tolerance)
					break;
				if (maxLoopCount > 0 && ++loopCount > maxLoopCount)
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
