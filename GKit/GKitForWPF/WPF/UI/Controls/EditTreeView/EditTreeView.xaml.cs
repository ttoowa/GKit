using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using GKitForWPF.Graphics;
using GKitForWPF.UI.Behaviors;
using GKitForWPF.UI.Converters;

namespace GKitForWPF.UI.Controls;

public partial class EditTreeView : UserControl, ITreeFolder, IListItemPageProvider {
    public delegate void ItemDelegate(ITreeItem item);

    public delegate void ItemLoopDelegate(ITreeItem item, ref bool breakFlag);

    public delegate void ItemMoveDelegate(ITreeItem item, ITreeFolder oldParent, ITreeFolder newParent, int index);

    public delegate void GlobalDropDelegate(ITreeItem srcItem, NodeTarget target);

    public delegate void MessageDelegate(string message);

    private const float SideEventRatio = 0.25f;
    private const float CenterEventRatio = 1f - SideEventRatio * 2f;

    public static readonly DependencyProperty DraggingCursorBrushProperty =
        DependencyProperty.RegisterAttached(nameof(DraggingCursorBrush), typeof(Brush), typeof(EditTreeView),
            new PropertyMetadata("4DFFEF".ToBrush()));

    public static readonly DependencyProperty AutoApplyItemMoveProperty =
        DependencyProperty.RegisterAttached(nameof(AutoApplyItemMove), typeof(bool), typeof(EditTreeView),
            new PropertyMetadata(true));

    public static readonly DependencyProperty CanMultiSelectProperty =
        DependencyProperty.RegisterAttached(nameof(CanMultiSelect), typeof(bool), typeof(EditTreeView),
            new PropertyMetadata(true));

    public static readonly DependencyProperty ItemShadowVisibleProperty =
        DependencyProperty.RegisterAttached(nameof(ItemShadowVisible), typeof(bool), typeof(EditTreeView),
            new PropertyMetadata(false));

    public readonly List<EditTreeView> GlobalDraggingTargetList = new();

    private FrameworkElement draggingClone;
    private float dragStartOffset;

    //Drag
    private bool isPressed;
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
            if (SelectedItemSet.Count > 0) {
                ITreeItem item = SelectedItemSet.Last() as ITreeItem;
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

    public event ItemMoveDelegate ItemMoved;
    public event MessageDelegate MessageOccured;
    public event GlobalDropDelegate GlobalDropped;

    public EditTreeView() {
        InitializeComponent();

        InitMembers();
        InitBindings();

        SetDraggingState(false);
        SetDraggingCursorVisible(false);
    }

    public void SetDisplayName(string name) {
    }

    public void SetSelected(bool isSelected) {
    }

    private void InitMembers() {
        SelectedItemSet = new SelectedItemSet();
    }

    private void InitBindings() {
        BoolToVisibilityConverter boolToVisibilityConverter = new();

        ItemShadow.SetBinding(VisibilityProperty,
            new Binding(nameof(ItemShadowVisible))
                { Source = this, Mode = BindingMode.OneWay, Converter = boolToVisibilityConverter });
        DraggingCursor.SetBinding(Border.BackgroundProperty,
            new Binding(nameof(DraggingCursorBrush)) { Source = this });
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
                    SelectedItemSet.Remove(item);
                } else {
                    SelectedItemSet.Add(item);
                }

                pressedItem = SelectedItemSet.Count > 0 ? SelectedItemSet.Last as ITreeItem : null;
            } else if (CanMultiSelect && pressedItem != null &&
                       (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))) {
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

        isPressed = true;
    }

    private void ItemContext_MouseMove(object sender, MouseEventArgs e) {
        if (isPressed && e.LeftButton != MouseButtonState.Pressed) {
            isPressed = false;
            onMouseCapture = false;

            ItemContext_MouseUp(sender, null);
            return;
        }

        if (!onMouseCapture) {
            return;
        }

        bool isHandled = false;
        if (onDragging) {
            foreach (EditTreeView globalDraggingTarget in GlobalDraggingTargetList) {
                // IsMouseOver 가 동작하지 않으므로 위치 검사를 통해 isOver 를 알아냅니다.
                Point cursorRelPoint = e.GetPosition(globalDraggingTarget);
                bool isOver = cursorRelPoint.X >= 0 && cursorRelPoint.X <= globalDraggingTarget.ActualWidth &&
                              cursorRelPoint.Y >= 0 && cursorRelPoint.Y <= globalDraggingTarget.ActualHeight;
                // HitTestResult result = VisualTreeHelper.HitTest(globalDraggingTarget, e.GetPosition(globalDraggingTarget));
                if (isOver) {
                    if (!isHandled) {
                        isHandled = true;

                        OnGlobalMouseMove(globalDraggingTarget, e);
                    }
                } else {
                    globalDraggingTarget.SetDraggingCursorVisible(false);
                }
            }
        }

        if (!isHandled) {
            OnLocalMouseMove(e);
        }
    }

    private void OnGlobalMouseMove(EditTreeView treeView, MouseEventArgs e) {
        Point cursorPos = e.GetPosition(treeView.ChildItemScrollViewer);
        Point absoluteCursorPos = e.GetPosition(treeView.ChildItemStackPanel);

        SetDraggingCursorVisible(false);
        treeView.SetDraggingCursorVisible(true);
        treeView.GetNodeTargetWithUpdateCursor((float)cursorPos.Y, false);
    }

    private void OnLocalMouseMove(MouseEventArgs e) {
        Point cursorPos = e.GetPosition(ChildItemScrollViewer);
        Point absoluteCursorPos = e.GetPosition(ChildItemStackPanel);

        if (!onDragging) {
            float distance = Mathf.Abs(dragStartOffset - (float)e.GetPosition((IInputElement)pressedItem).Y);

            if (distance < 10) {
                return;
            }

            //Select item
            if (SelectedItemSet.Count > 1) {
                if (SelectedItemSet.Contains(pressedItem)) {
                    SelectedItemSet.Remove(pressedItem);
                    SelectedItemSet.Add(pressedItem);
                } else {
                    SelectedItemSet.SetSingle(pressedItem);
                }
            }

            //Start drag
            SetDraggingState(true);
            CreateDraggingClone((FrameworkElement)pressedItem);
        }

        //Dragging
        SetDraggingCursorVisible(true);
        GetNodeTargetWithUpdateCursor((float)cursorPos.Y);

        //Move draggingClone
        draggingClone.Margin = new Thickness(0d, cursorPos.Y - dragStartOffset, 0d, 0d);
        draggingClone.Opacity = 0.3d;
    }

    private void ItemContext_MouseUp(object sender, MouseButtonEventArgs e) {
        if (!isPressed || !onMouseCapture) {
            return;
        }

        if ((e != null && e.ChangedButton != MouseButton.Left) || !onMouseCapture) {
            return;
        }

        isPressed = false;
        onMouseCapture = false;

        Mouse.Capture(null);

        bool isHandled = false;
        foreach (EditTreeView globalDraggingTarget in GlobalDraggingTargetList) {
            if (globalDraggingTarget.IsMouseOver) {
                if (!isHandled) {
                    isHandled = true;
                    ItemContext_GlobalMouseUp(globalDraggingTarget, e);
                }
            }

            globalDraggingTarget.SetDraggingCursorVisible(false);
        }

        if (!isHandled) {
            ItemContext_LocalMouseUp(e);
        }

        RemoveDraggingClone();
        SetDraggingState(false);
        SetDraggingCursorVisible(false);
    }

    private void ItemContext_GlobalMouseUp(EditTreeView treeView, MouseButtonEventArgs e) {
        if (e == null) return;

        Point cursorPos = e.GetPosition(treeView.ChildItemScrollViewer);
        Point absoluteCursorPos = e.GetPosition(treeView.ChildItemStackPanel);
        NodeTarget target = treeView.GetNodeTargetWithUpdateCursor((float)cursorPos.Y, false);

        treeView.GlobalDropped?.Invoke(pressedItem, target);
    }

    private void ItemContext_LocalMouseUp(MouseButtonEventArgs e) {
        if (!onDragging) {
            SelectedItemSet.SetSingle(pressedItem);
            return;
        }

        if (e == null) return;
        Point cursorPos = e.GetPosition(ChildItemScrollViewer);
        Point absoluteCursorPos = e.GetPosition(ChildItemStackPanel);
        NodeTarget target = GetNodeTargetWithUpdateCursor((float)cursorPos.Y);

        if (SelectedItemSet.Count > 1) {
            MoveSelectedItems(SelectedItemSet.ItemSet.Select(x => x as ITreeItem).ToArray(), target);
        } else {
            MoveSelectedItems(new ITreeItem[] { pressedItem }, target);
        }
    }

    private void Background_MouseDown(object sender, MouseButtonEventArgs e) {
        SelectedItemSet.Clear();
    }

    private void ShiftSelectItems(ITreeItem startItem, ITreeItem endItem) {
        //Unselect
        SelectedItemSet.Clear();

        //Select
        SelectedItemSet.Add(startItem);

        ITreeItem[] items = CollectItems();
        int startIndex = Array.IndexOf(items, startItem);
        int endIndex = Array.IndexOf(items, endItem);
        if (startIndex > endIndex) {
            int tempValue = endIndex;
            endIndex = startIndex;
            startIndex = tempValue;
        }

        for (int i = startIndex; i <= endIndex; ++i) {
            ITreeItem targetItem = items[i];
            if (targetItem != startItem) {
                if (SelectedItemSet.Contains(targetItem)) {
                    SelectedItemSet.Remove(targetItem);
                } else {
                    SelectedItemSet.Add(targetItem);
                }
            }
        }
    }

    // Notify
    public void NotifyItemRemoved(ITreeItem item) {
        SelectedItemSet.Remove(item);
    }

    // TreeSearch
    public void ForeachItems(ItemLoopDelegate nodeItemDelegate) {
        ForeachItemsRecursion(this);

        bool ForeachItemsRecursion(ITreeFolder folder) {
            bool breakFlag = false;

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
        // 보이는 영역의 item만 탐색해 nodeItemDelegate 함수를 실행합니다.
        float childContextHeight = (float)ChildItemScrollViewer.ActualHeight;

        ForeachItemsRecursion(this);

        bool ForeachItemsRecursion(ITreeFolder folder) {
            bool breakFlag = false;

            foreach (ITreeItem item in folder.ChildItemCollection) {
                FrameworkElement itemView = (FrameworkElement)item;

                //화면 영역 밖에 벗어나면 continue
                float itemTop = (float)item.TranslatePoint(new Point(), ChildItemScrollViewer).Y;
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

    public bool MoveItem(ITreeItem item, ITreeFolder newParent, int index) {
        if (item == null || newParent == null) {
            return false;
        }

        if (item == newParent) {
            return false;
        }

        if (index < 0 || index > newParent.ChildItemCollection.Count) {
            return false;
        }

        if (item is ITreeFolder && IsContainsChildRecursive(item as ITreeFolder, newParent)) {
            MessageOccured?.Invoke("자신의 하위 폴더로 이동할 수 없습니다.");
            return false;
        }

        ITreeFolder oldParent = item.ParentItem;
        if (oldParent == null) {
            return false;
        }

        if (oldParent == newParent) {
            int oldIndex = oldParent.ChildItemCollection.IndexOf(item as UIElement);
            if (oldIndex == index) {
                return false;
            } else if (oldIndex < index) {
                --index;
            }
        }

        oldParent.ChildItemCollection.Remove(item as UIElement);
        newParent.ChildItemCollection.Insert(index, item as UIElement);
        item.ParentItem = newParent;

        ItemMoved?.Invoke(item, oldParent, newParent, index);

        return true;
    }

    // DraggingStates
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

    private bool MoveSelectedItems(ITreeItem[] items, NodeTarget target) {
        if (target == null) {
            return false;
        }

        ITreeFolder[] selectedFolders = items.Where(item => item is ITreeFolder)
            .Select(item => item as ITreeFolder).ToArray();

        // 자신 내부에 이동시도시 Reject
        foreach (ITreeFolder folder in selectedFolders) {
            if (folder == target.node) {
                return false;
            }

            if (IsContainsChildRecursive(folder, target.node)) {
                MessageOccured?.Invoke("Cannot move to its own subfolder.");
                return false;
            }
        }

        // 정렬
        ITreeItem[] sortedSelectedItems = CollectSelectedItems(items);

        // 이동
        // if (target.node is ITreeFolder && target.direction == NodeDirection.Bottom &&
        //     ((ITreeFolder)target.node).ChildItemCollection.Count > 0) {
        //     target.direction = NodeDirection.InnerTop;
        // }

        if (target.direction == NodeDirection.Bottom || target.direction == NodeDirection.InnerTop) {
            sortedSelectedItems = sortedSelectedItems.Reverse().ToArray();
        }

        foreach (ITreeItem item in sortedSelectedItems) {
            ITreeFolder oldParent = item.ParentItem ?? this;
            ITreeFolder newParent = null;
            int index = -1;

            FrameworkElement uiItem = (FrameworkElement)item;

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
                index = newParent.ChildItemCollection.IndexOf(target.node as UIElement) +
                        (target.direction == NodeDirection.Bottom ? 1 : 0);
            }

            if (AutoApplyItemMove) {
                newParent.ChildItemCollection.Insert(index, item as UIElement);
            }

            item.ParentItem = newParent;

            ItemMoved?.Invoke(item, oldParent, newParent, index);
        }

        return true;
    }

    // Utility
    private bool IsContainsChildRecursive(ITreeFolder folder, ITreeItem target) {
        foreach (ITreeItem childItem in folder.ChildItemCollection) {
            if (childItem == target) {
                return true;
            }

            if (childItem is ITreeFolder)
                //Recursion
            {
                if (IsContainsChildRecursive(childItem as ITreeFolder, target)) {
                    return true;
                }
            }
        }

        return false;
    }

    private ITreeItem[] CollectSelectedItems(ITreeItem[] selectedItemSet) {
        List<ITreeItem> resultList = new();

        CollectSelectedItemsRecursion(this);

        return resultList.ToArray();

        void CollectSelectedItemsRecursion(ITreeItem item) {
            //Collect
            if (selectedItemSet.Contains(item)) {
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
        List<ITreeItem> resultList = new();

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

    private NodeTarget GetNodeTargetWithUpdateCursor(float cursorPosY, bool withoutSelf = true) {
        NodeTarget target = null;

        float bottom = (float)ChildItemStackPanel
            .TranslatePoint(new Point(0f, ChildItemStackPanel.ActualHeight), ChildItemScrollViewer).Y;

        //최하단으로 이동
        const float ClippingBias = 2f;
        if (cursorPosY > bottom - ClippingBias) {
            cursorPosY = bottom - 1f;

            target = new NodeTarget(RootFolder, NodeDirection.InnerBottom);
            SetDraggingCursorPosition(bottom, 10f);
        }

        if (target == null)
            //성능을 위해 탐색하면서 SetDraggingCursorPosition을 같이 호출합니다.
        {
            ForeachItemsOptimize((ITreeItem item, ref bool breakFlag) => {
                if (withoutSelf && item == pressedItem)
                    return;
                // 보이지 않는 상태면 제외
                FrameworkElement frameworkItem = item as FrameworkElement;
                if (frameworkItem != null) {
                    // 부모를 포함해서 Visibility가 Visible이 아니면 제외
                    if (!frameworkItem.IsUserVisible()) {
                        return;
                    }
                }

                float top = (float)item.TranslatePoint(new Point(0, 0), ChildItemScrollViewer).Y;
                float diff = cursorPosY - top;

                float itemHeight = (float)item.ItemContext.ActualHeight;
                if (diff <= 0f || diff >= itemHeight) {
                    return;
                }

                target = new NodeTarget(item);

                if (item is ITreeFolder) {
                    //Folder
                    float sideEventHeight = itemHeight * SideEventRatio;
                    float centerEventHeight = itemHeight * CenterEventRatio;

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
                    float sideEventHeight = itemHeight * 0.5f;

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
            });
        }

        SetDraggingCursorVisible(target != null);

        return target;
    }

    private ITreeItem GetPressedItem(FrameworkElement pressedElement) {
        //부모 트리로 Item이 나올 때까지 탐색하는 함수이다.
        DependencyObject parent = pressedElement.Parent;

        if (pressedElement is ITreeItem) {
            return pressedElement as ITreeItem;
        }

        if (parent != null && !(parent is Window) && parent is FrameworkElement) {
            return GetPressedItem((FrameworkElement)parent);
        }

        return null;
    }

    public int GetCurrentIndex() {
        ISelectable selectedItem = SelectedItemSet.LastOrDefault();
        if (selectedItem == null) {
            return -1;
        }

        TwoWayDictionary<int, ITreeItem> map = GetIndexMap();
        if (!map.Reverse.ContainsKey(selectedItem as ITreeItem)) {
            return -1;
        }

        return map.Reverse[selectedItem as ITreeItem];
        ;
    }

    public int GetPageCount() {
        return CollectItems().Length;
    }

    public void SetCurrentIndex(int index) {
        TwoWayDictionary<int, ITreeItem> map = GetIndexMap();

        if (!map.Forward.ContainsKey(index)) {
            return;
        }

        ITreeItem item = map.Forward[index];
        SelectedItemSet.SetSingle(item);
    }

    public TwoWayDictionary<int, ITreeItem> GetIndexMap() {
        ITreeItem[] items = CollectItems();
        TwoWayDictionary<int, ITreeItem> map = new();

        for (int i = 0; i < items.Length; ++i) {
            map.Add(i, items[i]);
        }

        return map;
    }
}