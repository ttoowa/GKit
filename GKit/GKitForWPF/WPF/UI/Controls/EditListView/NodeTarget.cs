using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKit.WPF.UI.Controls {
	public class NodeTarget {
		public IListItem node;
		public NodeDirection direction;

		public NodeTarget(IListItem node) {
			this.node = node;
			direction = NodeDirection.Top;
		}
		public NodeTarget(IListItem node, NodeDirection direction) {
			this.node = node;
			this.direction = direction;
		}
	}
}
