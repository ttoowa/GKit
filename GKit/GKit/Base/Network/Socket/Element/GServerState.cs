#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Network {
	public enum GServerState {
		Stopped,
		Starting,
		Running,
		Stopping,
	}
}
