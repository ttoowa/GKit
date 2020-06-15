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
	public class MotionValue<T> where T : struct {
		public T dstValue;
		public T currentValue;
		public T Delta => (dynamic)dstValue - (dynamic)currentValue;
		public event Arg1Delegate<T> OnUpdated;

		public MotionValue() {

		}
		public void UpdateValue(float deltaFactor) {
			currentValue += ((dynamic)dstValue - (dynamic)currentValue) * deltaFactor;

			OnUpdated?.Invoke(currentValue);
		}
	}
}
