using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKit.Security {
	public static class SecurityEvent {
		public static event Action OnMemoryHacked;

		internal static void CallMemoryHacked() {
			OnMemoryHacked.TryInvoke();
		}
	}
}
