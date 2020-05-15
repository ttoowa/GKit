using GKit;
using GKit.WPF.UI.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace GKit.WPF.UI.Controls {
	public delegate void ListItemDelegate(ITreeItem item);
	public delegate void ListItemLoopDelegate(ITreeItem item, ref bool breakFlag);
	public delegate void ListItemMoveDelegate(ITreeItem item, ITreeFolder oldParent, ITreeFolder newParent, int index);
	public delegate void MessageDelegate(string message);

	/// <summary>
	/// EditTreeView.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class EditTreeView : UserControl, ITreeFolder {
		public static readonly DependencyProperty DraggingCursorBrushProperty = DependencyProperty.RegisterAttached(nameof(DraggingCursorBrush), typeof(Brush), typeof(EditTreeView), new PropertyMetadata("4DFFEF".ToBrush()));
		public static readonly DependencyProperty AutoApplyItemMoveProperty = DependencyProperty.RegisterAttached(nameof(AutoApplyItemMove), typeof(bool), typeof(EditTreeView), new PropertyMetadata(true));
		public static readonly DependencyProperty CanMultiSelectProperty = DependencyProperty.RegisterAttached(nameof(CanMultiSelect), typeof(bool), typeof(EditTreeView), new PropertyMetadata(true));
		public static readonly DependencyProperty ItemShadowVisibleProperty = DependencyProperty.RegisterAttached(nameof(ItemShadowVisible), typeof(bool), typeof(EditTreeView), new PropertyMetadata(false));

		private const float SideEventRatio = 0.25f;
		private const float CenterEventRatio = 1f - SideEventRatio * 2f;

		//Option
		public bool AutoApplyItemMove {
			get {
				return (bool)GetValue(AutoApplyItemMoveProperty);
			}
			set {
				SetValue(AutoApplyItemMoveProperty, value);
			}
		}
		public bool CanMultiSelect {
			get {
				return (bool)GetValue(CanMultiSelectProperty);
			}
			set {
				SetValue(CanMultiSelectProperty, value);
			}
		}
		public bool ItemShadowVisible {
			get {
				return (bool)GetValue(ItemShadowVisibleProperty);
			}
			set {
				SetValue(ItemShadowVisibleProperty, value);
			}
		}

		//Info
		public string DisplayName => "Root";
		public Brush DraggingCursorBrush {
			get {
				return (Brush)GetValue(DraggingCursorBrushProperty);
			}
			set {
				SetValue(DraggingCursorBrushProperty, value);
			}
		}
		public float DisplayHeight => 0f;

		//Node
		public bool HasParentItem => false;
		public ITreeFolder ParentItem {
			get {
				return null;
			}
			set {
				throw new NotImplementedException();
			}
		}
		public UIElementCollection ChildItemCollection => ChildItemStackPanel.Children;
		public FrameworkElement ItemContext => throw new NotImplementedException("Root의 ItemContext는 사용할 수 없습니다.");
		public ITreeFolder ManualRootFolder {
			get; set;
		}
		public ITreeFolder RootFolder => ManualRootFolder == null ? this : ManualRootFolder;

		//Drag
		private bool isMousePressed;
		private bool onDragging;
		private bool onMouseCapture;
		private float dragStartOffset;
		private FrameworkElement draggingClone;
		private ITreeItem pressedItem;

		//Select
		public SelectedListItemSet SelectedItemSet {
			get; private set;
		}
		public ITreeFolder SelectedItemParent {
			get {
				if (SelectedItemSet.Count == 1) {
					ITreeItem item = SelectedItemSet.First;
					if (item is ITreeFolder) {
						return item as ITreeFolder;
					} else {
						return item.ParentItem;
					}
				}
				return null;
			}
		}

		public event ListItemMoveDelegate ItemMoved;
		public event MessageDelegate MessageOccured;

		public EditTreeView() {
			InitializeComponent();

			InitMembers();
			InitBindings();

			SetDraggingState(false);
			SetDraggingCursorVisible(false);
		}
		private void InitMembers() {
			SelectedItemSet = new SelectedListItemSet();
		}
		private void InitBindings() {
			BoolToVisibilityConverter boolToVisibilityConverter = new BoolToVisibilityConverter();

			ItemShadow.SetBinding(VisibilityProperty, new Binding(nameof(ItemShadowVisible)) { Source = this, Mode = BindingMode.OneWay, Converter = boolToVisibilityConverter });
			DraggingCursor.SetBinding(Border.BackgroundProperty, new Binding(nameof(DraggingCursorBrush)) { Source = this });
		}

		//Events
		private void ItemContext_MouseDown(object sender, MouseButtonEventArgs e) {
			if (e.ChangedButton != MouseButton.Left)
				return;

			isMousePressed = true;

			ITreeItem item = null;
			if (e.OriginalSource is FrameworkElement) {
				item = GetPressedItem((FrameworkElement)e.OriginalSource);
			}

			//Control 혹은 Shift를 누른 상태에선 Pressed에서 이벤트를 종료한다.
			if (item != null) {
				if (CanMultiSelect && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
					//Control select
					if (SelectedItemSet.Contains(item)) {
						SelectedItemSet.RemoveSelectedItem(item);
					} else {
						SelectedItemSet.AddSelectedItem(item);
					}
					
					pressedItem = SelectedItemSet.Count > 0 ? SelectedItemSet.Last : null;
				} else if (CanMultiSelect && pressedItem != null && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))) {
					//Shift select
					//pressedItem(Exclusive) ~ Item(Inclusive) 까지 선택한다.
					ShiftSelectItems(pressedItem, item);
				} else if (e.ClickCount == 2) {
					//Double click

				} else {
					//Normal select
					onMouseCapture = true;

					pressedItem = item;
					dragStartOffset = (float)e.GetPosition((IInputElement)item).Y;

					Mouse.Capture((IInputElement)sender, CaptureMode.Element);
				}
			}
		}
		private void ItemContext_MouseMove(object sender, MouseEventArgs e) {
			if (isMousePressed && e.LeftButton != MouseButtonState.Pressed) {
				ItemContext_MouseUp(sender, null);
				return;
			}
			if (!onMouseCapture)
				return;

			Point cursorPos = e.GetPosition(ChildItemScrollViewer);
			Point absoluteCursorPos = e.GetPosition(ChildItemStackPanel);

			if (!onDragging) {
				float distance = Mathf.Abs(dragStartOffset - (float)e.GetPosition((IInputElement)pressedItem).Y);

				if (distance < 10) {
					return;
				} else {
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
			}

			//Dragging
			GetNodeTarget((float)cursorPos.Y);

			//Move draggingClone
			draggingClone.Margin = new Thickness(0d, cursorPos.Y - dragStartOffset, 0d, 0d);
			draggingClone.Opacity = 0.3d;
		}
		private void ItemContext_MouseUp(object sender, MouseButtonEventArgs e) {
			if ((e != null && e.ChangedButton != MouseButton.Left) || !onMouseCapture)
				return;

			isMousePressed = false;
			onMouseCapture = false;

			Mouse.Capture(null);

			if (!onDragging) {
				SelectedItemSet.SetSelectedItem(pressedItem);
				return;
			}

			if (e != null) {
				Point cursorPos = e.GetPosition(ChildItemScrollViewer);
				Point absoluteCursorPos = e.GetPosition(ChildItemStackPanel);
				NodeTarget target = GetNodeTarget((float)cursorPos.Y);
				MoveSelectedItems(SelectedItemSet, target);
			}

			RemoveDraggingClone();
			SetDraggingState(false);
			SetDraggingCursorVisible(false);

		}

		public void SetDisplayName(string name) {
		}
		public void SetDisplaySelected(bool isSelected) {
		}

		private void ShiftSelectItems(ITreeItem startItem, ITreeItem endItem) {
			//Unselect
			SelectedItemSet.UnselectItems();

			//Select
			SelectedItemSet.AddSelectedItem(startItem);

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
						SelectedItemSet.RemoveSelectedItem(targetItem);
					} else {
						SelectedItemSet.AddSelectedItem(targetItem);
					}
				}
			}
		}

		//TreeSearch
		public void ForeachItems(ListItemLoopDelegate nodeItemDelegate) {
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
		public void ForeachItemsOptimize(ListItemLoopDelegate nodeItemDelegate) {
			float childContextHeight = (float)ChildItemScrollViewer.ActualHeight;

			ForeachItemsRecursion(this);

			bool ForeachItemsRecursion(ITreeFolder folder) {
				bool breakFlag = false;

				foreach (ITreeItem item in folder.ChildItemCollection) {
					//화면 영역 밖에 벗어나면 스킵
					FrameworkElement itemView = (FrameworkElement)item;
					float itemTop = (float)item.TranslatePoint(new Point(), ChildItemScrollViewer).Y;
					if ((itemTop < -itemView.ActualHeight || itemTop > childContextHeight)) {
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
			if (draggingClone != null)
				return;

			draggingClone = (FrameworkElement)Activator.CreateInstance(pressedItem.GetType());
			draggingClone.IsHitTestVisible = false;
			draggingClone.VerticalAlignment = VerticalAlignment.Top;
			((ITreeItem)draggingClone).SetDisplayName(((ITreeItem)refItem).DisplayName);
			ContentGrid.Children.Add(draggingClone);
		}
		private void RemoveDraggingClone() {
			if (draggingClone == null)
				return;

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

		//Node
		//private void AddItemToSelectedPosition(IListItem item) {
		//	IListFolder selectedItemParent = SelectedItemParent;

		//	selectedItemParent.ChildItemCollection.Add(item as UIElement);
		//	item.ParentItem = selectedItemParent;
		//}
		//private void AddFolderToSelectedPosition(IListFolder folder) {
		//	IListFolder selectedItemParent = SelectedItemParent;

		//	selectedItemParent.ChildItemCollection.Add(folder as UIElement);
		//	folder.ParentItem = selectedItemParent;
		//}

		private bool MoveSelectedItems(SelectedListItemSet selectedItemSet, NodeTarget target) {
			if (target == null)
				return false;

			ITreeFolder[] selectedFolders = selectedItemSet
				.Where(item => item is ITreeFolder)
				.Select(item => item as ITreeFolder).ToArray();

			//자신 내부에 이동시도시 Reject
			foreach (ITreeFolder folder in selectedFolders) {
				if (folder == target.node)
					return false;

				if (IsContainsChildRecursive(folder, target.node)) {
					MessageOccured?.Invoke("자신의 하위 폴더로 이동할 수 없습니다.");
					return false;
				}
			}

			//정렬
			ITreeItem[] sortedSelectedItems = CollectSelectedItems();

			//이동
			if (target.node is ITreeFolder && target.direction == NodeDirection.Bottom && ((ITreeFolder)target.node).ChildItemCollection.Count > 0) {
				target.direction = NodeDirection.InnerTop;
			}
			if (target.direction == NodeDirection.Bottom || target.direction == NodeDirection.InnerTop) {
				sortedSelectedItems = sortedSelectedItems.Reverse().ToArray();
			}
			foreach (ITreeItem item in sortedSelectedItems) {
				ITreeFolder oldParent = item.ParentItem;
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
					newParent = target.node.ParentItem;
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

		//Utility
		private bool IsContainsChildRecursive(ITreeFolder folder, ITreeItem target) {
			foreach (ITreeItem childItem in folder.ChildItemCollection) {
				if (childItem == target) {
					return true;
				} else if (childItem is ITreeFolder) {
					//Recursion
					if (IsContainsChildRecursive(childItem as ITreeFolder, target)) {
						return true;
					}
				}
			}
			return false;
		}
		private ITreeItem[] CollectSelectedItems() {
			List<ITreeItem> resultList = new List<ITreeItem>();

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
			List<ITreeItem> resultList = new List<ITreeItem>();

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

			float bottom = (float)ChildItemStackPanel.TranslatePoint(new Point(0f, ChildItemStackPanel.ActualHeight), ChildItemScrollViewer).Y;

			//최하단으로 이동
			const float ClippingBias = 2f;
			if (cursorPosY > bottom - ClippingBias) {
				cursorPosY = (float)bottom - 1f;

				target = new NodeTarget(RootFolder, NodeDirection.InnerBottom);
				SetDraggingCursorPosition(bottom, 10f);
			}

			if (target == null) {
				//성능을 위해 탐색하면서 SetDraggingCursorPosition을 같이 호출합니다.
				ForeachItemsOptimize((ITreeItem item, ref bool breakFlag) => {
					if (item != pressedItem) {
						float top = (float)item.TranslatePoint(new Point(0, 0), ChildItemScrollViewer).Y;
						float diff = cursorPosY - top;

						float itemHeight = (float)item.ItemContext.ActualHeight;
						if (diff > 0f && diff < itemHeight) {
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
						}
					}
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
			} else if (parent != null && !(parent is Window) && parent is FrameworkElement) {
				return GetPressedItem((FrameworkElement)parent);
			} else {
				return null;
			}
		}

	}
}
