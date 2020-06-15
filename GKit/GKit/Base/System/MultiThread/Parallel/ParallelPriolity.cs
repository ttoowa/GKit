#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.MultiThread {
	public enum ParallelPriolity {
		Low,
		Normal,
		High,
		Full,
	}
}
