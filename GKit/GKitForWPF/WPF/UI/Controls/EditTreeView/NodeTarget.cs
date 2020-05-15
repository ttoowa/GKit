using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKit.WPF.UI.Controls {
	public class NodeTarget {
		public ITreeItem node;
		public NodeDirection direction;

		public NodeTarget(ITreeItem node) {
			this.node = node;
			direction = NodeDirection.Top;
		}
		public NodeTarget(ITreeItem node, NodeDirection direction) {
			this.node = node;
			this.direction = direction;
		}
	}
}
