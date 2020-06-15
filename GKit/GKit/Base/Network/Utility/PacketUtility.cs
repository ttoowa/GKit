#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Network {
	public static class PacketUtility {

		public static byte[] ToArray(this byte data) {
			return new byte[] { data };
		}
	}
}
