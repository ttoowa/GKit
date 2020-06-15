using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GKitForWPF.UI.Controls {
	public interface ITreeItem {
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
		void SetDisplaySelected(bool isSelected);

		//IListFolder GetParentItem();

		Point TranslatePoint(Point point, UIElement targetElement);
	}
}
