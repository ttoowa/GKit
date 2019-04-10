#if WPF
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace GKit {
	/// <summary>
	/// WPF의 UI 확장 기능을 제공하는 클래스입니다. (MainCore 필요)
	/// </summary>
	public static class UIExtension {
		private static SolidColorBrush OnBrush = "#00000000".ToBrush();
		private static SolidColorBrush OverBrush = "#FFFFFF22".ToBrush();
		private static SolidColorBrush DownBrush = "#00000022".ToBrush();

		public static void SetOnClick(this FrameworkElement control, Action action) {
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				MouseInput.OnLeftUpOnce += () => {
					if (control.IsMouseOver) {
						action.SafeInvoke();
					}
				};
			};
		}
		public static void SetOnRightClick(this FrameworkElement control, Action action) {
			control.MouseRightButtonDown += (object sender, MouseButtonEventArgs e) => {
				MouseInput.OnRightUpOnce += () => {
					if (control.IsMouseOver) {
						action.SafeInvoke();
					}
				};
			};
		}
		public static void SetDoubleClickEvent(this FrameworkElement control, Action action) {
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				if (e.ClickCount == 2) {
					action.SafeInvoke();
				}
			};
		}
		public static void SetBtnColor(this Shape control) {
			control.SetBtnColor(OnBrush, OverBrush, DownBrush);
		}
		public static void SetBtnColor(this Control control, Shape target) {
			control.SetBtnColor(() => {
				target.Fill = OnBrush;
			}, () => {
				target.Fill = OverBrush;
			}, () => {
				target.Fill = DownBrush;
			});
		}
		public static void SetBtnColor(this Control control, Border target) {
			control.SetBtnColor(() => {
				target.Background = OnBrush;
			}, () => {
				target.Background = OverBrush;
			}, () => {
				target.Background = DownBrush;
			});
		}
		public static void SetBtnColor(this Control control, Panel target) {
			control.SetBtnColor(() => {
				target.Background = OnBrush;
			}, () => {
				target.Background = OverBrush;
			}, () => {
				target.Background = DownBrush;
			});
		}
		public static void SetBtnColor(this Control control, Control target) {
			control.SetBtnColor(() => {
				target.Background = OnBrush;
			}, () => {
				target.Background = OverBrush;
			}, () => {
				target.Background = DownBrush;
			});
		}
		public static void SetBtnColor(this Panel control, Shape target) {
			control.SetBtnColor(() => {
				target.Fill = OnBrush;
			}, () => {
				target.Fill = OverBrush;
			}, () => {
				target.Fill = DownBrush;
			});
		}
		public static void SetBtnColor(this Panel control, Border target) {
			control.SetBtnColor(() => {
				target.Background = OnBrush;
			}, () => {
				target.Background = OverBrush;
			}, () => {
				target.Background = DownBrush;
			});
		}
		public static void SetBtnColor(this Panel control, Panel target) {
			control.SetBtnColor(() => {
				target.Background = OnBrush;
			}, () => {
				target.Background = OverBrush;
			}, () => {
				target.Background = DownBrush;
			});
		}
		public static void SetBtnColor(this Panel control, Control target) {
			control.SetBtnColor(() => {
				target.Background = OnBrush;
			}, () => {
				target.Background = OverBrush;
			}, () => {
				target.Background = DownBrush;
			});
		}
		public static void SetBtnColor(this Shape control, Shape target) {
			control.SetBtnColor(() => {
				target.Fill = OnBrush;
			}, () => {
				target.Fill = OverBrush;
			}, () => {
				target.Fill = DownBrush;
			});
		}
		public static void SetBtnColor(this Shape control, Border target) {
			control.SetBtnColor(() => {
				target.Background = OnBrush;
			}, () => {
				target.Background = OverBrush;
			}, () => {
				target.Background = DownBrush;
			});
		}
		public static void SetBtnColor(this Shape control, Panel target) {
			control.SetBtnColor(() => {
				target.Background = OnBrush;
			}, () => {
				target.Background = OverBrush;
			}, () => {
				target.Background = DownBrush;
			});
		}
		public static void SetBtnColor(this Shape control, Control target) {
			control.SetBtnColor(() => {
				target.Background = OnBrush;
			}, () => {
				target.Background = OverBrush;
			}, () => {
				target.Background = DownBrush;
			});
		}
		public static void SetBtnColor(this Border control, Shape target) {
			control.SetBtnColor(() => {
				target.Fill = OnBrush;
			}, () => {
				target.Fill = OverBrush;
			}, () => {
				target.Fill = DownBrush;
			});
		}
		public static void SetBtnColor(this Border control, Border target) {
			control.SetBtnColor(() => {
				target.Background = OnBrush;
			}, () => {
				target.Background = OverBrush;
			}, () => {
				target.Background = DownBrush;
			});
		}
		public static void SetBtnColor(this Border control, Panel target) {
			control.SetBtnColor(() => {
				target.Background = OnBrush;
			}, () => {
				target.Background = OverBrush;
			}, () => {
				target.Background = DownBrush;
			});
		}
		public static void SetBtnColor(this Border control, Control target) {
			control.SetBtnColor(() => {
				target.Background = OnBrush;
			}, () => {
				target.Background = OverBrush;
			}, () => {
				target.Background = DownBrush;
			});
		}
		public static void SetBtnColor(this Shape control, Action on, Action over, Action down) {
			control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
				if (control.IsMouseOver) {
					over.SafeInvoke();
				} else {
					on.SafeInvoke();
				}
				control.ReleaseMouseCapture();
			};
			control.MouseLeave += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released) {
					on.SafeInvoke();
				}
			};
			control.MouseEnter += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released) {
					over.SafeInvoke();
				}
			};
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				down.SafeInvoke();
				control.CaptureMouse();
			};
		}
		public static void SetBtnColor(this Shape control, Color on, Color over, Color down) {
			SolidColorBrush onBrush = new SolidColorBrush(on);
			SolidColorBrush overBrush = new SolidColorBrush(over);
			SolidColorBrush downBrush = new SolidColorBrush(down);
			SetBtnColor(control, onBrush, overBrush, downBrush);
		}
		public static void SetBtnColor(this Shape control, SolidColorBrush on, SolidColorBrush over, SolidColorBrush down) {
			control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
				if (control.IsMouseOver) {
					control.Fill = over;
				} else {
					control.Fill = on;
				}
				control.ReleaseMouseCapture();
			};
			control.MouseLeave += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released) {
					control.Fill = on;
				}
			};
			control.MouseEnter += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released) {
					control.Fill = over;
				}
			};
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				control.Fill = down;
				control.CaptureMouse();
			};
		}
		public static void SetBtnColor(this Border control) {
			control.SetBtnColor(OnBrush, OverBrush, DownBrush);
		}
		public static void SetBtnColor(this Border control, Action on, Action over, Action down) {
			control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
				if (control.IsMouseOver) {
					over.SafeInvoke();
				} else {
					on.SafeInvoke();
				}
				control.ReleaseMouseCapture();
			};
			control.MouseLeave += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					on.SafeInvoke();
			};
			control.MouseEnter += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					over.SafeInvoke();
			};
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				down.SafeInvoke();
				control.CaptureMouse();
			};
		}
		public static void SetBtnColor(this Border control, Color on, Color over, Color down) {
			SolidColorBrush onBrush = new SolidColorBrush(on);
			SolidColorBrush overBrush = new SolidColorBrush(over);
			SolidColorBrush downBrush = new SolidColorBrush(down);
			SetBtnColor(control, onBrush, overBrush, downBrush);
		}
		public static void SetBtnColor(this Border control, SolidColorBrush on, SolidColorBrush over, SolidColorBrush down) {
			control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
				if (control.IsMouseOver) {
					control.Background = over;
				} else {
					control.Background = on;
				}
				control.ReleaseMouseCapture();
			};
			control.MouseLeave += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					control.Background = on;
			};
			control.MouseEnter += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					control.Background = over;
			};
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				control.Background = down;
				control.CaptureMouse();
			};
		}
		public static void SetBtnColor(this Panel control) {
			control.SetBtnColor(OnBrush, OverBrush, DownBrush);
		}
		public static void SetBtnColor(this Panel control, Action on, Action over, Action down) {
			control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
				if (control.IsMouseOver) {
					over.SafeInvoke();
				} else {
					on.SafeInvoke();
				}
				control.ReleaseMouseCapture();
			};
			control.MouseLeave += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					on.SafeInvoke();
			};
			control.MouseEnter += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					over.SafeInvoke();
			};
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				down.SafeInvoke();
				control.CaptureMouse();
			};
		}
		public static void SetBtnColor(this Panel control, Color on, Color over, Color down) {
			SolidColorBrush onBrush = new SolidColorBrush(on);
			SolidColorBrush overBrush = new SolidColorBrush(over);
			SolidColorBrush downBrush = new SolidColorBrush(down);
			SetBtnColor(control, onBrush, overBrush, downBrush);
		}
		public static void SetBtnColor(this Panel control, SolidColorBrush on, SolidColorBrush over, SolidColorBrush down) {
			control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
				if (control.IsMouseOver) {
					control.Background = over;
				} else {
					control.Background = on;
				}
				control.ReleaseMouseCapture();
			};
			control.MouseLeave += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					control.Background = on;
			};
			control.MouseEnter += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					control.Background = over;
			};
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				control.Background = down;
				control.CaptureMouse();
			};
		}
		public static void SetBtnColor(this Control control) {
			control.SetBtnColor(OnBrush, OverBrush, DownBrush);
		}
		public static void SetBtnColor(this Control control, Action on, Action over, Action down) {
			control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
				if (control.IsMouseOver) {
					over.SafeInvoke();
				} else {
					on.SafeInvoke();
				}
				control.ReleaseMouseCapture();
			};
			control.MouseLeave += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					on.SafeInvoke();
			};
			control.MouseEnter += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					over.SafeInvoke();
			};
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				down.SafeInvoke();
				control.CaptureMouse();
			};
		}
		public static void SetBtnColor(this Control control, Color on, Color over, Color down) {
			SolidColorBrush onBrush = new SolidColorBrush(on);
			SolidColorBrush overBrush = new SolidColorBrush(over);
			SolidColorBrush downBrush = new SolidColorBrush(down);
			SetBtnColor(control, onBrush, overBrush, downBrush);
		}
		public static void SetBtnColor(this Control control, SolidColorBrush on, SolidColorBrush over, SolidColorBrush down) {
			control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
				if (control.IsMouseOver) {
					control.Background = over;
				} else {
					control.Background = on;
				}
				control.ReleaseMouseCapture();
			};
			control.MouseLeave += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					control.Background = on;
			};
			control.MouseEnter += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					control.Background = over;
			};
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				control.Background = down;
				control.CaptureMouse();
			};
		}
		public static Color Light(this Color color, int value) {
			byte r, g, b;
			r = (byte)Math.Max(0, Math.Min(255, color.R + value));
			g = (byte)Math.Max(0, Math.Min(255, color.G + value));
			b = (byte)Math.Max(0, Math.Min(255, color.B + value));
			return Color.FromArgb(color.A, r, g, b);
		}

		public static void SetOnlyNumberInput(this TextBox textBox) {
			textBox.PreviewTextInput += (object sender, TextCompositionEventArgs e) => {
				Regex regex = new Regex("[^0-9]+");
				e.Handled = regex.IsMatch(e.Text);
			};
		}

		public static Vector2 GetAbsolutePosition(this FrameworkElement control, Vector2 point) {
			return (Vector2)control.PointToScreen(new Point(point.x, point.y));
		}
		public static Vector2 GetAbsolutePosition(this FrameworkElement control) {
			return (Vector2)control.PointToScreen(new Point(0, 0));
		}
		public static Vector2 GetPosition(this FrameworkElement control) {
			UIElement container = VisualTreeHelper.GetParent(control) as UIElement;
			return (Vector2)control.TranslatePoint(new Point(0, 0), container);
		}
		public static Vector2 GetPosition(this FrameworkElement control, UIElement parent) {
			return (Vector2)control.TranslatePoint(new Point(0, 0), parent);
		}

		public delegate void IndexChangedDelegate(int beforeIndex, int newIndex);
		public static void SetIndexChangeable(this Control control, StackPanel parent, GLoopEngine core, IndexChangedDelegate OnIndexChanged = null) {
			Control grabbedItem = null;
			int grabbedIndex = 0;
			bool cancelFlag = false;

			Action OnItemGrabDrag = () => {
				if(!MouseInput.LeftAuto) {
					cancelFlag = true;
				}
				if (cancelFlag)
					return;

				//Find Cursor Index
				int newIndex = -1;
				Control item;
				int count = parent.Children.Count;
				for (int i = 0; i < count; ++i) {
					item = (Control)parent.Children[i];
					float itemHalfHeight = (float)(item.ActualHeight * 0.5f);

					if (MouseInput.AbsolutePosition.y < item.GetAbsolutePosition(new Vector2(0, itemHalfHeight)).y) {
						newIndex = i;
						break;
					}
				}
				if (newIndex == -1) {
					newIndex = count;
				}
				if (newIndex != grabbedIndex) {
					if (newIndex > grabbedIndex) {
						--newIndex;
					}

					parent.Children.Remove(grabbedItem);
					parent.Children.Insert(newIndex, grabbedItem);

					OnIndexChanged?.Invoke(grabbedIndex, newIndex);
					grabbedIndex = newIndex;
				}
			};
			Action OnItemGrabStart = () => {
				grabbedItem = control;
				grabbedIndex = parent.Children.IndexOf(control);
				core.AddLoopAction(OnItemGrabDrag, GLoopCycle.EveryFrame, GWhen.MouseUpRemove);
			};

			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				OnItemGrabStart();
			};
		}
	}
}
#endif