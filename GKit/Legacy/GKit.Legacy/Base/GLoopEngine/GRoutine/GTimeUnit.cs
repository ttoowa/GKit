#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public enum GTimeUnit {
		Frame,
		Second,
		Millisecond,
	}
}
