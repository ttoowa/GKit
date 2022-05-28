#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Network {
	public enum GClientState {
		Disconnected,
		Connecting,
		Connected,
		Disconnecting,
	}
}
