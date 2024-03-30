namespace GKitForWPF.UI.Controls;

public class GlobalDraggingState {
    public EditTreeView treeView;
    public ITreeItem draggingItem;
    public bool isIntercepted;

    public GlobalDraggingState(EditTreeView treeView, ITreeItem draggingItem) {
        this.treeView = treeView;
        this.draggingItem = draggingItem;
    }
}