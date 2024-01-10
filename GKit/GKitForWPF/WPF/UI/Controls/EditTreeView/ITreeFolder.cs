using System.Windows.Controls;

namespace GKitForWPF.UI.Controls;

public interface ITreeFolder : ITreeItem {
    UIElementCollection ChildItemCollection { get; }
}