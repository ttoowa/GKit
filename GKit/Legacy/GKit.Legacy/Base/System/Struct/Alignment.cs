#if !OnWPF
#if OnUnity
namespace GKitForUnity
#else
namespace GKit
#endif
{
	public enum HorizontalAlignment {
		Left,
		Center,
		Right,
		Stretch,
	}
	public enum VerticalAlignment {
		Top,
		Center,
		Bottom,
		Stretch,
	}
}
#endif