using System.Windows;
using GKitForWPF;

namespace GKitForWPF.UI.Controls {
	public interface ITreeItem : ISelectable {
		string DisplayName {
			get;
		}

		FrameworkElement ItemContext {
			get;
		}

		ITreeFolder ParentItem {
			get; set;
		}

		void SetDisplayName(string name);

		//IListFolder GetParentItem();

		Point TranslatePoint(Point point, UIElement targetElement);
	}
}
