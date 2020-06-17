#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public interface ISelectable {
		void SetSelected(bool isSelected);
	}
}
