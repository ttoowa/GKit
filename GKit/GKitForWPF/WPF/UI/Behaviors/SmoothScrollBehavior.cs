using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Input;
using GKitForWPF;
using System.Collections.Generic;

namespace GKitForWPF.UI.Behaviors {
	public class ScrollData {
		public double VerticalOffset {
			get => scrollViewer.VerticalOffset;
			set => scrollViewer.ScrollToVerticalOffset(value);
		}
		public double HorizontalOffset {
			get => scrollViewer.HorizontalOffset;
			set => scrollViewer.ScrollToHorizontalOffset(value);
		}

		public ScrollViewer scrollViewer;
		public double targetVerticalOffset;
		public double targetHorizontalOffset;

		public bool OnPlayingVerticalAnim => VerticalScrollAnim != null;
		public DoubleAnimation VerticalScrollAnim {
			get; set;
		}

		public ScrollData(ScrollViewer scrollViewer) {
			this.scrollViewer = scrollViewer;

			UpdateTargetOffset();
		}
		public void UpdateTargetOffset() {
			targetVerticalOffset = VerticalOffset;
			targetHorizontalOffset = HorizontalOffset;
		}
	}
	public static class SmoothScrollBehavior {
		public static TimeSpan DefaultTimeDuration = new TimeSpan(0, 0, 0, 0, 270);
		public static int DefaultPointsToScroll = 6;
		public static IEasingFunction EasingFuncgion = new PowerEase();

		private static Dictionary<FrameworkElement, ScrollData> scrollDataDict = new Dictionary<FrameworkElement, ScrollData>();

		private static ScrollViewer listBoxScroller = new ScrollViewer();

		public static DependencyProperty VerticalOffsetProperty =
			DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(SmoothScrollBehavior),
												new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));
		public static void SetVerticalOffset(FrameworkElement target, double value) {
			target.SetValue(VerticalOffsetProperty, value);
		}
		public static double GetVerticalOffset(FrameworkElement target) {
			return (double)target.GetValue(VerticalOffsetProperty);
		}

		public static DependencyProperty TimeDurationProperty =
			DependencyProperty.RegisterAttached("TimeDuration", typeof(TimeSpan), typeof(SmoothScrollBehavior),
												new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 0)));
		public static void SetTimeDuration(FrameworkElement target, TimeSpan value) {
			target.SetValue(TimeDurationProperty, value);
		}
		public static TimeSpan GetTimeDuration(FrameworkElement target) {
			return (TimeSpan)target.GetValue(TimeDurationProperty);
		}

		public static DependencyProperty PointsToScrollProperty =
			DependencyProperty.RegisterAttached("PointsToScroll", typeof(double), typeof(SmoothScrollBehavior),
												new PropertyMetadata(0.0));
		public static void SetPointsToScroll(FrameworkElement target, double value) {
			target.SetValue(PointsToScrollProperty, value);
		}
		public static double GetPointsToScroll(FrameworkElement target) {
			return (double)target.GetValue(PointsToScrollProperty);
		}


		public static DependencyProperty IsEnabledProperty =
												DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(SmoothScrollBehavior),
												new UIPropertyMetadata(false, OnIsEnabledChanged));
		public static void SetIsEnabled(FrameworkElement target, bool value) {
			target.SetValue(IsEnabledProperty, value);
		}
		public static bool GetIsEnabled(FrameworkElement target) {
			return (bool)target.GetValue(IsEnabledProperty);
		}

		private static void AnimateVerticalScroll(ScrollViewer scrollViewer, double ToValue) {
			ScrollData scrollData = GetOrCreateScrollData(scrollViewer);


			if(scrollData.OnPlayingVerticalAnim) {
				//Stop animation
				scrollViewer.BeginAnimation(VerticalOffsetProperty, null);
			}

			double durationMillisec = GetTimeDuration(scrollViewer).TotalMilliseconds;

			DoubleAnimation scrollAnim = new DoubleAnimation() {
				From = scrollViewer.VerticalOffset,
				To = ToValue,
				Duration = new Duration(new TimeSpan(0, 0, 0, 0, (int)durationMillisec)),
				EasingFunction = SmoothScrollBehavior.EasingFuncgion,
			};
			scrollData.VerticalScrollAnim = scrollAnim;
			scrollAnim.Completed += ScrollAnim_Completed;
			
			scrollViewer.BeginAnimation(VerticalOffsetProperty, scrollData.VerticalScrollAnim);

			void ScrollAnim_Completed(object sender, EventArgs e) {
				if(scrollData.VerticalScrollAnim == scrollAnim) {
					scrollData.VerticalScrollAnim = null;
				}
				scrollDataDict.Remove(scrollViewer);
			}
		}

		private static ScrollData GetOrCreateScrollData(ScrollViewer scrollViewer) {
			if(scrollDataDict.ContainsKey(scrollViewer)) {
				return scrollDataDict[scrollViewer];
			} else {
				ScrollData scrollData = new ScrollData(scrollViewer);
				scrollDataDict[scrollViewer] = scrollData;

				return scrollData;
			}
		}

		private static double NormalizeScrollPos(ScrollViewer scroll, double scrollChange, Orientation o) {
			double returnValue = scrollChange;

			if (scrollChange < 0) {
				returnValue = 0;
			}

			if (o == Orientation.Vertical && scrollChange > scroll.ScrollableHeight) {
				returnValue = scroll.ScrollableHeight;
			} else if (o == Orientation.Horizontal && scrollChange > scroll.ScrollableWidth) {
				returnValue = scroll.ScrollableWidth;
			}

			return returnValue;
		}

		private static void UpdateListBoxScrollPosition(object sender) {
			ListBox listbox = sender as ListBox;

			if (listbox != null) {
				double scrollTo = 0;

				for (int i = 0; i < (listbox.SelectedIndex); i++) {
					ListBoxItem tempItem = listbox.ItemContainerGenerator.ContainerFromItem(listbox.Items[i]) as ListBoxItem;

					if (tempItem != null) {
						scrollTo += tempItem.ActualHeight;
					}
				}

				AnimateVerticalScroll(listBoxScroller, scrollTo);
			}
		}

		private static void SetEventHandlersForScrollViewer(ScrollViewer scrollViewer) {
			scrollViewer.PreviewMouseWheel += new MouseWheelEventHandler(ScrollViewerPreviewMouseWheel);
			scrollViewer.PreviewKeyDown += new KeyEventHandler(ScrollViewerPreviewKeyDown);
			scrollViewer.PreviewMouseLeftButtonUp += Scroller_PreviewMouseLeftButtonUp;
			scrollViewer.ScrollChanged += Scroller_ScrollChanged;

			SetTimeDuration(scrollViewer, DefaultTimeDuration);
			SetPointsToScroll(scrollViewer, DefaultPointsToScroll);
		}

		private static void Scroller_ScrollChanged(object sender, ScrollChangedEventArgs e) {
			if (!scrollDataDict.ContainsKey((ScrollViewer)sender))
				return;
			ScrollData scrollData = GetOrCreateScrollData((ScrollViewer)sender);

			if (!scrollData.OnPlayingVerticalAnim && Mathf.Abs((float)e.VerticalChange) > 0d) {
				scrollData.targetVerticalOffset = scrollData.VerticalOffset;
			}
			if(Mathf.Abs((float)e.HorizontalChange) > 0d) {
				scrollData.targetHorizontalOffset = scrollData.HorizontalOffset;
			}
		}

		private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			var target = sender;

			if(target != null) {
				FrameworkElement element = null;

				if (target is ScrollViewer) {
					element = target as ScrollViewer;
					element.Loaded += ScrollViewerLoaded;
				} else if (target is ListBox) {
					element = target as ListBox;
					element.Loaded += ListBoxLoaded;
				}

				void ScrollViewerLoaded(object sender2, RoutedEventArgs e2) {
					element.Loaded -= ScrollViewerLoaded;

					SetEventHandlersForScrollViewer(element as ScrollViewer);
				}
				void ListBoxLoaded(object sender2, RoutedEventArgs e2) {
					element.Loaded -= ListBoxLoaded;

					ListBox listbox = element as ListBox;

					listBoxScroller = FindControlUtility.GetFirstChildOfType<ScrollViewer>(listbox);
					SetEventHandlersForScrollViewer(listBoxScroller);

					SetTimeDuration(listBoxScroller, new TimeSpan(0, 0, 0, 0, 200));
					SetPointsToScroll(listBoxScroller, 16.0);

					listbox.SelectionChanged += ListBoxSelectionChanged;
					listbox.Loaded += SmoothScrollBehavior.ListBoxLoaded;
					listbox.LayoutUpdated += ListBoxLayoutUpdated;
				}
			}
		}

		private static void Scroller_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			ScrollData scrollData = GetOrCreateScrollData((ScrollViewer)sender);

			scrollData.UpdateTargetOffset();
		}

		private static void ListBoxLayoutUpdated(object sender, EventArgs e) {
			UpdateListBoxScrollPosition(sender);
		}
		private static void ListBoxLoaded(object sender, RoutedEventArgs e) {
			UpdateListBoxScrollPosition(sender);
		}
		private static void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
			UpdateListBoxScrollPosition(sender);
		}

		private static void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e) {
			ScrollData scrollData = GetOrCreateScrollData((ScrollViewer)sender);

			double newVOffset = scrollData.targetVerticalOffset - (e.Delta + 2f);

			scrollData.scrollViewer.ScrollToVerticalOffset(scrollData.targetVerticalOffset);

			if (newVOffset < 0) {
				newVOffset = 0;
			} else if (newVOffset > scrollData.scrollViewer.ScrollableHeight) {
				newVOffset = scrollData.scrollViewer.ScrollableHeight;
			} else {
			}

			AnimateVerticalScroll(scrollData.scrollViewer, newVOffset);
			scrollData.targetVerticalOffset = newVOffset;

			e.Handled = true;
		}
		private static void ScrollViewerPreviewKeyDown(object sender, KeyEventArgs e) {
			ScrollData scrollData = GetOrCreateScrollData((ScrollViewer)sender);
			ScrollViewer scrollViewer = scrollData.scrollViewer;

			double keyScrollPoints = GetPointsToScroll(scrollViewer) * 20d;
			double newVerticalPos = scrollData.VerticalOffset;
			bool isKeyHandled = false;

			switch (e.Key) {
				case Key.Up:
					newVerticalPos = NormalizeScrollPos(scrollViewer, (newVerticalPos - keyScrollPoints), Orientation.Vertical);
					isKeyHandled = true;
					break;
				case Key.Down:
					newVerticalPos = NormalizeScrollPos(scrollViewer, (newVerticalPos + keyScrollPoints), Orientation.Vertical);
					isKeyHandled = true;
					break;
				case Key.PageDown:
					newVerticalPos = NormalizeScrollPos(scrollViewer, (newVerticalPos + scrollViewer.ViewportHeight), Orientation.Vertical);
					isKeyHandled = true;
					break;
				case Key.PageUp:
					newVerticalPos = NormalizeScrollPos(scrollViewer, (newVerticalPos - scrollViewer.ViewportHeight), Orientation.Vertical);
					isKeyHandled = true;
					break;
			}
			scrollData.targetVerticalOffset = newVerticalPos;

			if (newVerticalPos != GetVerticalOffset(scrollViewer)) {
				scrollData.targetVerticalOffset = newVerticalPos;
				AnimateVerticalScroll(scrollViewer, newVerticalPos);
			}

			e.Handled = isKeyHandled;
		}

		private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) {
			ScrollViewer scrollViewer = target as ScrollViewer;
			if (scrollViewer != null) {
				scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
			}
		}
	}
}