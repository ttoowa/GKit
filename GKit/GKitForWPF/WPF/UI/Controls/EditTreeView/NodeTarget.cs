namespace GKitForWPF.UI.Controls {
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
