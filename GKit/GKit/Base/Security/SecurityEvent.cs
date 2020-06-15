using System;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Security {
	public static class SecurityEvent {
		public static event Action OnMemoryHacked;

		internal static void CallMemoryHacked() {
			OnMemoryHacked.TryInvoke();
		}
	}
}
