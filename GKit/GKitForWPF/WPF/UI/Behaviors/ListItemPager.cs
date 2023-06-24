using System.Windows.Controls;
using System.Windows.Input;

namespace GKitForWPF.UI.Behaviors;

public interface ListItemPageProvider {
    public int GetCurrentIndex();
    public int GetPageCount();
    public void SetCurrentIndex(int index);
}

public class ListItemPager {
    public static void ApplyListItemPager(ScrollViewer viewer, ListItemPageProvider element) {
        viewer.PreviewKeyDown += (sender, args) => {
            switch (args.Key) {
                case Key.Up: {
                    int index = element.GetCurrentIndex();
                    if (index == -1) return;
                    if (index == 0) return;
                    element.SetCurrentIndex(index - 1);
                    args.Handled = true; // This prevents the ScrollViewer from scrolling.
                    break;
                }
                case Key.Down: {
                    int index = element.GetCurrentIndex();
                    int length = element.GetPageCount();
                    if (index == -1) return;
                    if (length - 1 <= index) return;
                    element.SetCurrentIndex(index + 1);
                    args.Handled = true; // This prevents the ScrollViewer from scrolling.
                    break;
                }
            }
        };
    }
}