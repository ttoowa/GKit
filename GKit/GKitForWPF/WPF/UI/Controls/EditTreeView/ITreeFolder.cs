using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GKit.WPF.UI.Controls {
	public interface ITreeFolder : ITreeItem {
		UIElementCollection ChildItemCollection {
			get;
		}
	}
}
