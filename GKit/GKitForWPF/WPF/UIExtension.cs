#if OnWPF
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace GKit.WPF {
	/// <summary>
	/// WPF의 UI 확장 기능을 제공하는 클래스입니다. (LoopEngine 필요)
	/// </summary>
	public static class UIExtension {
		private const float DefaultCoverValue = 0.1f;

		public static void RegisterLoaded(this FrameworkElement element, RoutedEventHandler handler) {
			RoutedEventHandler unregisterEvent = null;
			unregisterEvent = UnregisterEvent;

			element.Loaded += handler;
			element.Loaded += unregisterEvent;

			void UnregisterEvent(object sender, RoutedEventArgs e) {
				element.Loaded -= handler;
				element.Loaded -= unregisterEvent;
			}
		}

		public static void RegisterClickEvent(this Button button, Action action) {
			button.Click += (object sender, RoutedEventArgs e) => { action?.Invoke(); };
		}
		public static void RegisterClickEvent(this Button button, ActionEvent actionEvent) {
			button.Click += (object sender, RoutedEventArgs e) => { actionEvent?.Invoke(); };
		}
		public static void RegisterClickEvent(this Button button, RoutedEventHandler handler) {
			button.Click += (object sender, RoutedEventArgs e) => { handler.Invoke(sender, e); };
		}

		public static void RegisterOnClick(this FrameworkElement control, Action action) {
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				MouseInput.Left.OnUpOnce += () => {
					if (control.IsMouseOver) {
						action?.Invoke();
					}
				};
			};
		}
		public static void RegisterOnRightClick(this FrameworkElement control, Action action) {
			control.MouseRightButtonDown += (object sender, MouseButtonEventArgs e) => {
				MouseInput.Right.OnUpOnce += () => {
					if (control.IsMouseOver) {
						action?.Invoke();
					}
				};
			};
		}
		public static void RegisterDoubleClickEvent(this FrameworkElement control, Action action) {
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				if (e.ClickCount == 2) {
					action?.Invoke();
				}
			};
		}

		public static void RegisterButtonReaction(this Shape control, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(
			GetCoverColor(0f).ToBrush(),
			GetCoverColor(value).ToBrush(),
			GetCoverColor(-value).ToBrush());
		}
		public static void RegisterButtonReaction(this Border control, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(
			GetCoverColor(0f).ToBrush(),
			GetCoverColor(value).ToBrush(),
			GetCoverColor(-value).ToBrush());
		}
		public static void RegisterButtonReaction(this Panel control, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(
			GetCoverColor(0f).ToBrush(),
			GetCoverColor(value).ToBrush(),
			GetCoverColor(-value).ToBrush());
		}
		public static void RegisterButtonReaction(this Control control, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(
			GetCoverColor(0f).ToBrush(),
			GetCoverColor(value).ToBrush(),
			GetCoverColor(-value).ToBrush());
		}
		public static void RegisterButtonReaction(this Control control, Shape transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Fill = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Fill = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Fill = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Control control, Border transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Background = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Control control, Panel transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Background = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Control control, Control transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Background = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Panel control, Shape transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Fill = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Fill = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Fill = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Panel control, Border transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Background = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Panel control, Panel transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Background = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Panel control, Control transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Background = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Shape control, Shape transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Fill = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Fill = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Fill = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Shape control, Border transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Background = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Shape control, Panel transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Background = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Shape control, Control transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Background = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Border control, Shape transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Fill = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Fill = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Fill = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Border control, Border transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Background = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Border control, Panel transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Background = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Border control, Control transparentCover, float value = DefaultCoverValue) {
			control.RegisterButtonReaction(() => {
				transparentCover.Background = GetCoverColor(0f).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(value).ToBrush();
			}, () => {
				transparentCover.Background = GetCoverColor(-value).ToBrush();
			});
		}
		public static void RegisterButtonReaction(this Shape control, Action on, Action over, Action down) {
			control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
				if (control.IsMouseOver) {
					over.TryInvoke();
				} else {
					on.TryInvoke();
				}
				control.ReleaseMouseCapture();
			};
			control.MouseLeave += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released) {
					on.TryInvoke();
				}
			};
			control.MouseEnter += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released) {
					over.TryInvoke();
				}
			};
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				down.TryInvoke();
				control.CaptureMouse();
			};
		}
		public static void RegisterButtonReaction(this Shape control, Color on, Color over, Color down) {
			SolidColorBrush onBrush = new SolidColorBrush(on);
			SolidColorBrush overBrush = new SolidColorBrush(over);
			SolidColorBrush downBrush = new SolidColorBrush(down);
			RegisterButtonReaction(control, onBrush, overBrush, downBrush);
		}
		public static void RegisterButtonReaction(this Shape control, SolidColorBrush on, SolidColorBrush over, SolidColorBrush down) {
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
		public static void RegisterButtonReaction(this Border control, Action on, Action over, Action down) {
			control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
				if (control.IsMouseOver) {
					over.TryInvoke();
				} else {
					on.TryInvoke();
				}
				control.ReleaseMouseCapture();
			};
			control.MouseLeave += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					on.TryInvoke();
			};
			control.MouseEnter += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					over.TryInvoke();
			};
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				down.TryInvoke();
				control.CaptureMouse();
			};
		}
		public static void RegisterButtonReaction(this Border control, Color on, Color over, Color down) {
			SolidColorBrush onBrush = new SolidColorBrush(on);
			SolidColorBrush overBrush = new SolidColorBrush(over);
			SolidColorBrush downBrush = new SolidColorBrush(down);
			RegisterButtonReaction(control, onBrush, overBrush, downBrush);
		}
		public static void RegisterButtonReaction(this Border control, SolidColorBrush on, SolidColorBrush over, SolidColorBrush down) {
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
		public static void RegisterButtonReaction(this Panel control, Action on, Action over, Action down) {
			control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
				if (control.IsMouseOver) {
					over.TryInvoke();
				} else {
					on.TryInvoke();
				}
				control.ReleaseMouseCapture();
			};
			control.MouseLeave += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					on.TryInvoke();
			};
			control.MouseEnter += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					over.TryInvoke();
			};
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				down.TryInvoke();
				control.CaptureMouse();
			};
		}
		public static void RegisterButtonReaction(this Panel control, Color on, Color over, Color down) {
			SolidColorBrush onBrush = new SolidColorBrush(on);
			SolidColorBrush overBrush = new SolidColorBrush(over);
			SolidColorBrush downBrush = new SolidColorBrush(down);
			RegisterButtonReaction(control, onBrush, overBrush, downBrush);
		}
		public static void RegisterButtonReaction(this Panel control, SolidColorBrush on, SolidColorBrush over, SolidColorBrush down) {
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
		public static void RegisterButtonReaction(this Control control, Action on, Action over, Action down) {
			control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
				if (control.IsMouseOver) {
					over.TryInvoke();
				} else {
					on.TryInvoke();
				}
				control.ReleaseMouseCapture();
			};
			control.MouseLeave += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					on.TryInvoke();
			};
			control.MouseEnter += (object sender, MouseEventArgs e) => {
				if (Mouse.LeftButton == MouseButtonState.Released)
					over.TryInvoke();
			};
			control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
				down.TryInvoke();
				control.CaptureMouse();
			};
		}
		public static void RegisterButtonReaction(this Control control, Color on, Color over, Color down) {
			SolidColorBrush onBrush = new SolidColorBrush(on);
			SolidColorBrush overBrush = new SolidColorBrush(over);
			SolidColorBrush downBrush = new SolidColorBrush(down);
			RegisterButtonReaction(control, onBrush, overBrush, downBrush);
		}
		public static void RegisterButtonReaction(this Control control, SolidColorBrush on, SolidColorBrush over, SolidColorBrush down) {
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
		public static Color GetCoverColor(float value) {
			Color color = new Color();
			if(value < 0f) {
				color.R = color.G = color.B = 0;
			} else {
				color.R = color.G = color.B = 255;
			}
			color.A = (byte)(Mathf.Clamp01(Mathf.Abs(value)) * BMath.Float2Byte);
			return color;
		}

		public static void SetOnlyIntInput(this TextBox textBox) {
			SetOnlyRegexInput(textBox, "[^0-9]+");
		}
		public static void SetOnlyFloatInput(this TextBox textBox) {
			SetOnlyRegexInput(textBox, "[^0-9.]+");
		}
		public static void SetOnlyRegexInput(this TextBox textBox, string regexPattern) {
			textBox.PreviewTextInput += (object sender, TextCompositionEventArgs e) => {
				Regex regex = new Regex(regexPattern);
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

		public static bool IsDesignMode(this DependencyObject obj) {
			return DesignerProperties.GetIsInDesignMode(obj);
		}

		public static void DetachParent(this FrameworkElement element) {
			if(element.Parent != null) {
				element.Parent.Cast<Panel>().Children.Remove(element);
			}
		}
		public static void SetParent(this FrameworkElement element, Panel parent) {
			parent.Children.Add(element);
		}

		public delegate void IndexChangedDelegate(int oldIndex, int newIndex);
		public static void SetIndexChangeable(this Control control, StackPanel parentStackPanel, IndexChangedDelegate OnIndexChanged = null) {
			Control grabbedItem = null;
			int grabbedIndex = 0;
			bool onDragging = false;
			control.MouseDown += (object sender, MouseButtonEventArgs e) => {
				if(e.ChangedButton == MouseButton.Left) {
					onDragging = true;
					Mouse.Capture(control);

					grabbedItem = control;
					grabbedIndex = parentStackPanel.Children.IndexOf(control);
				}	
			};
			control.MouseMove += (object sender, MouseEventArgs e) => {
				if (!onDragging)
					return;

				//Find Cursor Index
				int newIndex = -1;
				Control item;
				int count = parentStackPanel.Children.Count;
				for (int i = 0; i < count; ++i) {
					item = (Control)parentStackPanel.Children[i];
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

					parentStackPanel.Children.Remove(grabbedItem);
					parentStackPanel.Children.Insert(newIndex, grabbedItem);

					OnIndexChanged?.Invoke(grabbedIndex, newIndex);
					grabbedIndex = newIndex;
				}
			};
			control.MouseUp += (object sender, MouseButtonEventArgs e) => {
				if (e.ChangedButton == MouseButton.Left) {
					onDragging = false;
					Mouse.Capture(null);
				}
			};
		}


	}
}
#endif