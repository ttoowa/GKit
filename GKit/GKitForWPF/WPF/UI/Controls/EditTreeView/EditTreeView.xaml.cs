using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using GKitForWPF.Graphics;
using GKitForWPF.UI.Converters;

namespace GKitForWPF.UI.Controls;

public partial class EditTreeView : UserControl, ITreeFolder {
    public delegate void ItemDelegate(ITreeItem item);

    public delegate void ItemLoopDelegate(ITreeItem item, ref bool breakFlag);

    public delegate void ItemMoveDelegate(ITreeItem item, ITreeFolder oldParent, ITreeFolder newParent, int index);

    public delegate void MessageDelegate(string message);

    private const float SideEventRatio = 0.25f;
    private const float CenterEventRatio = 1f - SideEventRatio * 2f;

    public static readonly DependencyProperty DraggingCursorBrushProperty =
        DependencyProperty.RegisterAttached(nameof(DraggingCursorBrush), typeof(Brush), typeof(EditTreeView), new PropertyMetadata("4DFFEF".ToBrush()));

    public static readonly DependencyProperty AutoApplyItemMoveProperty =
        DependencyProperty.RegisterAttached(nameof(AutoApplyItemMove), typeof(bool), typeof(EditTreeView), new PropertyMetadata(true));

    public static readonly DependencyProperty CanMultiSelectProperty =
        DependencyProperty.RegisterAttached(nameof(CanMultiSelect), typeof(bool), typeof(EditTreeView), new PropertyMetadata(true));

    public static readonly DependencyProperty ItemShadowVisibleProperty =
        DependencyProperty.RegisterAttached(nameof(ItemShadowVisible), typeof(bool), typeof(EditTreeView), new PropertyMetadata(false));

    private FrameworkElement draggingClone;
    private float dragStartOffset;

    //Drag
    private bool isMousePressed;
    private bool onDragging;
    private bool onMouseCapture;
    private ITreeItem pressedItem;

    //Option
    public bool AutoApplyItemMove {
        get => (bool)GetValue(AutoApplyItemMoveProperty);
        set => SetValue(AutoApplyItemMoveProperty, value);
    }

    public bool CanMultiSelect {
        get => (bool)GetValue(CanMultiSelectProperty);
        set => SetValue(CanMultiSelectProperty, value);
    }

    public bool ItemShadowVisible {
        get => (bool)GetValue(ItemShadowVisibleProperty);
        set => SetValue(ItemShadowVisibleProperty, value);
    }

    public Brush DraggingCursorBrush {
        get => (Brush)GetValue(DraggingCursorBrushProperty);
        set => SetValue(DraggingCursorBrushProperty, value);
    }

    public float DisplayHeight => 0f;

    //Node
    public bool HasParentItem => false;

    public ITreeFolder ManualRootFolder { get; set; }

    public ITreeFolder RootFolder => ManualRootFolder == null ? this : ManualRootFolder;

    //Select
    public SelectedItemSet SelectedItemSet { get; private set; }

    public ITreeFolder SelectedItemParent {
        get {
            if (SelectedItemSet.Count == 1) {
                var item = SelectedItemSet.First as ITreeItem;
                if (item is ITreeFolder) {
                    return item as ITreeFolder;
                }

                return item.ParentItem;
            }

            return null;
        }
    }

    //Info
    public string DisplayName => "Root";

    public ITreeFolder ParentItem {
        get => null;
        set => throw new NotImplementedException();
    }

    public UIElementCollection ChildItemCollection => ChildItemStackPanel.Children;
    public FrameworkElement ItemContext => throw new NotImplementedException("Root의 ItemContext는 사용할 수 없습니다.");

    public void SetDisplayName(string name) {
    }

    public void SetSelected(bool isSelected) {
    }

    public event ItemMoveDelegate ItemMoved;
    public event MessageDelegate MessageOccured;

    public EditTreeView() {
        InitializeComponent();

        InitMembers();
        InitBindings();

        SetDraggingState(false);
        SetDraggingCursorVisible(false);
    }

    private void InitMembers() {
        SelectedItemSet = new SelectedItemSet();
    }

    private void InitBindings() {
        var boolToVisibilityConverter = new BoolToVisibilityConverter();

        ItemShadow.SetBinding(VisibilityProperty, new Binding(nameof(ItemShadowVisible)) { Source = this, Mode = BindingMode.OneWay, Converter = boolToVisibilityConverter });
        DraggingCursor.SetBinding(Border.BackgroundProperty, new Binding(nameof(DraggingCursorBrush)) { Source = this });
    }

    //Events
    private void ItemContext_MouseDown(object sender, MouseButtonEventArgs e) {
        if (e.ChangedButton != MouseButton.Left) {
            return;
        }

        ITreeItem item = null;
        if (e.OriginalSource is FrameworkElement) {
            item = GetPressedItem((FrameworkElement)e.OriginalSource);
        }

        //Control 혹은 Shift를 누른 상태에선 Pressed에서 이벤트를 종료한다.
        if (item != null) {
            if ((CanMultiSelect && Keyboard.IsKeyDown(Key.LeftCtrl)) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                //Control select
                if (SelectedItemSet.Contains(item)) {
                    SelectedItemSet.RemoveSelectedItem(item);
                } else {
                    SelectedItemSet.AddSelectedItem(item);
                }

                pressedItem = SelectedItemSet.Count > 0 ? SelectedItemSet.Last as ITreeItem : null;
            } else if (CanMultiSelect && pressedItem != null && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))) {
                //Shift select
                //pressedItem(Exclusive) ~ Item(Inclusive) 까지 선택한다.
                ShiftSelectItems(pressedItem, item);
            } else if (e.ClickCount == 2) {
                //Double click
            } else {
                //Normal select


                pressedItem = item;
                dragStartOffset = (float)e.GetPosition((IInputElement)item).Y;

                Mouse.Capture((IInputElement)sender, CaptureMode.Element);

                onMouseCapture = true;
            }
        }

        isMousePressed = true;
    }

    private void ItemContext_MouseMove(object sender, MouseEventArgs e) {
        if (isMousePressed && e.LeftButton != MouseButtonState.Pressed) {
            ItemContext_MouseUp(sender, null);
            return;
        }

        if (!onMouseCapture) {
            return;
        }

        var cursorPos = e.GetPosition(ChildItemScrollViewer);
        var absoluteCursorPos = e.GetPosition(ChildItemStackPanel);

        if (!onDragging) {
            var distance = Mathf.Abs(dragStartOffset - (float)e.GetPosition((IInputElement)pressedItem).Y);

            if (distance < 10) {
                return;
            }

            //Select item
            if (SelectedItemSet.Contains(pressedItem)) {
                SelectedItemSet.RemoveSelectedItem(pressedItem);
                SelectedItemSet.AddSelectedItem(pressedItem);
            } else {
                SelectedItemSet.SetSelectedItem(pressedItem);
            }

            //Start drag
            SetDraggingState(true);
            SetDraggingCursorVisible(true);
            CreateDraggingClone((FrameworkElement)pressedItem);
        }

        //Dragging
        GetNodeTarget((float)cursorPos.Y);

        //Move draggingClone
        draggingClone.Margin = new Thickness(0d, cursorPos.Y - dragStartOffset, 0d, 0d);
        draggingClone.Opacity = 0.3d;
    }

    private void ItemContext_MouseUp(object sender, MouseButtonEventArgs e) {
        if ((e != null && e.ChangedButton != MouseButton.Left) || !onMouseCapture) {
            return;
        }

        isMousePressed = false;
        onMouseCapture = false;

        Mouse.Capture(null);

        if (!onDragging) {
            SelectedItemSet.SetSelectedItem(pressedItem);
            return;
        }

        if (e != null) {
            var cursorPos = e.GetPosition(ChildItemScrollViewer);
            var absoluteCursorPos = e.GetPosition(ChildItemStackPanel);
            var target = GetNodeTarget((float)cursorPos.Y);
            MoveSelectedItems(SelectedItemSet, target);
        }

        RemoveDraggingClone();
        SetDraggingState(false);
        SetDraggingCursorVisible(false);
    }

    private void Background_MouseDown(object sender, MouseButtonEventArgs e) {
        SelectedItemSet.UnselectItems();
    }

    private void ShiftSelectItems(ITreeItem startItem, ITreeItem endItem) {
        //Unselect
        SelectedItemSet.UnselectItems();

        //Select
        SelectedItemSet.AddSelectedItem(startItem);

        var items = CollectItems();
        var startIndex = Array.IndexOf(items, startItem);
        var endIndex = Array.IndexOf(items, endItem);
        if (startIndex > endIndex) {
            var tempValue = endIndex;
            endIndex = startIndex;
            startIndex = tempValue;
        }

        for (var i = startIndex; i <= endIndex; ++i) {
            var targetItem = items[i];
            if (targetItem != startItem) {
                if (SelectedItemSet.Contains(targetItem)) {
                    SelectedItemSet.RemoveSelectedItem(targetItem);
                } else {
                    SelectedItemSet.AddSelectedItem(targetItem);
                }
            }
        }
    }

    //Notify
    public void NotifyItemRemoved(ITreeItem item) {
        SelectedItemSet.RemoveSelectedItem(item);
    }

    //TreeSearch
    public void ForeachItems(ItemLoopDelegate nodeItemDelegate) {
        ForeachItemsRecursion(this);

        bool ForeachItemsRecursion(ITreeFolder folder) {
            var breakFlag = false;

            foreach (ITreeItem item in folder.ChildItemCollection) {
                nodeItemDelegate(item, ref breakFlag);

                if (item is ITreeFolder) {
                    if (!ForeachItemsRecursion(item as ITreeFolder)) {
                        return false;
                    }
                }

                if (breakFlag) {
                    return false;
                }
            }

            return true;
        }
    }

    public void ForeachItemsOptimize(ItemLoopDelegate nodeItemDelegate) {
        var childContextHeight = (float)ChildItemScrollViewer.ActualHeight;

        ForeachItemsRecursion(this);

        bool ForeachItemsRecursion(ITreeFolder folder) {
            var breakFlag = false;

            foreach (ITreeItem item in folder.ChildItemCollection) {
                //화면 영역 밖에 벗어나면 스킵
                var itemView = (FrameworkElement)item;
                var itemTop = (float)item.TranslatePoint(new Point(), ChildItemScrollViewer).Y;
                if (itemTop < -itemView.ActualHeight || itemTop > childContextHeight) {
                    continue;
                }

                nodeItemDelegate(item, ref breakFlag);

                if (item is ITreeFolder) {
                    if (!ForeachItemsRecursion(item as ITreeFolder)) {
                        return false;
                    }
                }

                if (breakFlag) {
                    return false;
                }
            }

            return true;
        }
    }

    //DraggingStates
    private void CreateDraggingClone(FrameworkElement refItem) {
        if (draggingClone != null) {
            return;
        }

        draggingClone = (FrameworkElement)Activator.CreateInstance(pressedItem.GetType());
        draggingClone.IsHitTestVisible = false;
        draggingClone.VerticalAlignment = VerticalAlignment.Top;
        ((ITreeItem)draggingClone).SetDisplayName(((ITreeItem)refItem).DisplayName);
        ContentGrid.Children.Add(draggingClone);
    }

    private void RemoveDraggingClone() {
        if (draggingClone == null) {
            return;
        }

        ContentGrid.Children.Remove(draggingClone);
        draggingClone = null;
    }

    private void SetDraggingState(bool onDragging) {
        this.onDragging = onDragging;
    }

    private void SetDraggingCursorVisible(bool visible) {
        DraggingCursor.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
    }

    private void SetDraggingCursorPosition(float top, float height) {
        DraggingCursor.Margin = new Thickness(0d, top, 0d, 0d);
        DraggingCursor.Height = height;
    }

    private bool MoveSelectedItems(SelectedItemSet selectedItemSet, NodeTarget target) {
        if (target == null) {
            return false;
        }

        var selectedFolders = selectedItemSet.Where(item => item is ITreeFolder).Select(item => item as ITreeFolder).ToArray();

        //자신 내부에 이동시도시 Reject
        foreach (var folder in selectedFolders) {
            if (folder == target.node) {
                return false;
            }

            if (IsContainsChildRecursive(folder, target.node)) {
                MessageOccured?.Invoke("자신의 하위 폴더로 이동할 수 없습니다.");
                return false;
            }
        }

        //정렬
        var sortedSelectedItems = CollectSelectedItems();

        //이동
        if (target.node is ITreeFolder && target.direction == NodeDirection.Bottom && ((ITreeFolder)target.node).ChildItemCollection.Count > 0) {
            target.direction = NodeDirection.InnerTop;
        }

        if (target.direction == NodeDirection.Bottom || target.direction == NodeDirection.InnerTop) {
            sortedSelectedItems = sortedSelectedItems.Reverse().ToArray();
        }

        foreach (var item in sortedSelectedItems) {
            var oldParent = item.ParentItem ?? this;
            ITreeFolder newParent = null;
            var index = -1;

            var uiItem = (FrameworkElement)item;

            if (oldParent != null) {
                oldParent.ChildItemCollection.Remove(item as UIElement);
            } else if (uiItem.Parent != null) {
                ((Panel)uiItem.Parent).Children.Remove(uiItem);
            }

            if (target.direction == NodeDirection.InnerTop) {
                //폴더 내부로
                newParent = target.node as ITreeFolder;
                index = 0;
            } else if (target.direction == NodeDirection.InnerBottom) {
                //폴더 내부로
                newParent = target.node as ITreeFolder;
                index = newParent.ChildItemCollection.Count;
            } else {
                //아이템 위아래로
                newParent = target.node.ParentItem ?? this;
                index = newParent.ChildItemCollection.IndexOf(target.node as UIElement) + (target.direction == NodeDirection.Bottom ? 1 : 0);
            }

            if (AutoApplyItemMove) {
                newParent.ChildItemCollection.Insert(index, item as UIElement);
            }

            item.ParentItem = newParent;

            ItemMoved?.Invoke(item, oldParent, newParent, index);
        }

        return true;
    }

    //Utility
    private bool IsContainsChildRecursive(ITreeFolder folder, ITreeItem target) {
        foreach (ITreeItem childItem in folder.ChildItemCollection) {
            if (childItem == target) {
                return true;
            }

            if (childItem is ITreeFolder) {
                //Recursion
                if (IsContainsChildRecursive(childItem as ITreeFolder, target)) {
                    return true;
                }
            }
        }

        return false;
    }

    private ITreeItem[] CollectSelectedItems() {
        var resultList = new List<ITreeItem>();

        CollectSelectedItemsRecursion(this);

        return resultList.ToArray();

        void CollectSelectedItemsRecursion(ITreeItem item) {
            //Collect
            if (SelectedItemSet.Contains(item)) {
                resultList.Add(item);
            }

            //Recursion
            if (item is ITreeFolder) {
                foreach (ITreeItem childItem in ((ITreeFolder)item).ChildItemCollection) {
                    CollectSelectedItemsRecursion(childItem);
                }
            }
        }
    }

    private ITreeItem[] CollectItems() {
        var resultList = new List<ITreeItem>();

        CollectItemsRecursion(this);

        return resultList.ToArray();

        void CollectItemsRecursion(ITreeItem item) {
            //Collect
            resultList.Add(item);

            //Recursion
            if (item is ITreeFolder) {
                foreach (ITreeItem childItem in ((ITreeFolder)item).ChildItemCollection) {
                    CollectItemsRecursion(childItem);
                }
            }
        }
    }

    private NodeTarget GetNodeTarget(float cursorPosY) {
        NodeTarget target = null;

        var bottom = (float)ChildItemStackPanel.TranslatePoint(new Point(0f, ChildItemStackPanel.ActualHeight), ChildItemScrollViewer).Y;

        //최하단으로 이동
        const float ClippingBias = 2f;
        if (cursorPosY > bottom - ClippingBias) {
            cursorPosY = bottom - 1f;

            target = new NodeTarget(RootFolder, NodeDirection.InnerBottom);
            SetDraggingCursorPosition(bottom, 10f);
        }

        if (target == null) {
            //성능을 위해 탐색하면서 SetDraggingCursorPosition을 같이 호출합니다.
            ForeachItemsOptimize((ITreeItem item, ref bool breakFlag) => {
                if (item != pressedItem) {
                    var top = (float)item.TranslatePoint(new Point(0, 0), ChildItemScrollViewer).Y;
                    var diff = cursorPosY - top;

                    var itemHeight = (float)item.ItemContext.ActualHeight;
                    if (diff > 0f && diff < itemHeight) {
                        target = new NodeTarget(item);

                        if (item is ITreeFolder) {
                            //Folder
                            var sideEventHeight = itemHeight * SideEventRatio;
                            var centerEventHeight = itemHeight * CenterEventRatio;

                            if (diff < sideEventHeight) {
                                //Top
                                target.direction = NodeDirection.Top;
                                SetDraggingCursorPosition(top, sideEventHeight);
                            } else if (diff > itemHeight - sideEventHeight) {
                                //Bottom
                                target.direction = NodeDirection.Bottom;
                                SetDraggingCursorPosition(top + itemHeight - sideEventHeight, sideEventHeight);
                            } else {
                                //Center
                                target.direction = NodeDirection.InnerBottom;
                                SetDraggingCursorPosition(top + sideEventHeight, centerEventHeight);
                            }
                        } else {
                            //Item
                            var sideEventHeight = itemHeight * 0.5f;

                            if (diff < sideEventHeight) {
                                //Top
                                target.direction = NodeDirection.Top;
                                SetDraggingCursorPosition(top, sideEventHeight);
                            } else {
                                //Bottom
                                target.direction = NodeDirection.Bottom;
                                SetDraggingCursorPosition(top + sideEventHeight, sideEventHeight);
                            }
                        }

                        breakFlag = true;
                    }
                }
            });
        }


        SetDraggingCursorVisible(target != null);

        return target;
    }

    private ITreeItem GetPressedItem(FrameworkElement pressedElement) {
        //부모 트리로 Item이 나올 때까지 탐색하는 함수이다.
        var parent = pressedElement.Parent;

        if (pressedElement is ITreeItem) {
            return pressedElement as ITreeItem;
        }

        if (parent != null && !(parent is Window) && parent is FrameworkElement) {
            return GetPressedItem((FrameworkElement)parent);
        }

        return null;
    }
}