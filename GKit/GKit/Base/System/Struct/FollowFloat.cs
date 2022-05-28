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
	public class FollowFloat {
		public float dstValue;
		public float currentValue;
		public float Delta => dstValue - currentValue;

		public event Arg1Delegate<float> OnUpdated;

		public FollowFloat() {

		}
		public void UpdateValue(float deltaFactor) {
			currentValue += (dstValue - currentValue) * deltaFactor;

			OnUpdated?.Invoke(currentValue);
		}
	}
}
