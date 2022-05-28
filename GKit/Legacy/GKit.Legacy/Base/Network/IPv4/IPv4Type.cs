#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Network {
	public enum IPv4Type {
		IPAddress,
		eIPAddress,
		kIPAddress,
	}
}
