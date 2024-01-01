using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using GKitForWPF.Graphics;

namespace GKitForWPF;

/// <summary>
///     WPF의 UI 확장 기능을 제공하는 클래스입니다.
/// </summary>
public static class UIUtility {
    public delegate void IndexChangedDelegate(int oldIndex, int newIndex);

    private const float DefaultCoverValue = 0.1f;

    public static Color Light(this Color color, int value) {
        byte r, g, b;
        r = (byte)Math.Max(0, Math.Min(255, color.R + value));
        g = (byte)Math.Max(0, Math.Min(255, color.G + value));
        b = (byte)Math.Max(0, Math.Min(255, color.B + value));
        return Color.FromArgb(color.A, r, g, b);
    }

    public static Color GetCoverColor(float lightnessAlpha) {
        Color color = new();
        if (lightnessAlpha < 0f)
            color.R = color.G = color.B = 0;
        else
            color.R = color.G = color.B = 255;

        color.A = (byte)(Mathf.Clamp01(Mathf.Abs(lightnessAlpha)) * GMath.Float2Byte);
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
            Regex regex = new(regexPattern);
            e.Handled = regex.IsMatch(e.Text);
        };
    }

    public static Vector2 GetAbsolutePosition(this FrameworkElement control) {
        return GetAbsolutePosition(control, new Vector2(0, 0));
    }

    public static Vector2 GetAbsolutePosition(this FrameworkElement control, Vector2 point) {
        return GetAbsolutePosition(control, new Point(point.x, point.y));
    }

    public static Vector2 GetAbsolutePosition(this FrameworkElement control, Point point) {
        //return (Vector2)PresentationSource.FromVisual(control).CompositionTarget.TransformToDevice.Transform(point);
        return (Vector2)control.PointToScreen(point);
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
        if (element.Parent != null)
            element.Parent.Cast<Panel>().Children.Remove(element);
    }

    public static void SetParent(this FrameworkElement element, Panel parent) {
        parent.Children.Add(element);
    }


    public static void SetIndexChangeableContext<ElementType>(this StackPanel context, IndexChangedDelegate OnIndexChanged = null) where ElementType : FrameworkElement {
        ElementType grabbedItem = null;
        int grabbedIndex = 0;
        bool onDragging = false;
        context.MouseDown += (object sender, MouseButtonEventArgs e) => {
            if (e.ChangedButton != MouseButton.Left)
                return;

            if (e.OriginalSource is FrameworkElement)
                grabbedItem = GetPressedItem((FrameworkElement)e.OriginalSource);

            if (grabbedItem == null)
                return;

            int initGrabbedIndex = context.Children.IndexOf(grabbedItem);
            grabbedIndex = initGrabbedIndex;

            Mouse.Capture(grabbedItem);
            onDragging = true;
        };
        context.MouseMove += (object sender, MouseEventArgs e) => {
            if (!onDragging)
                return;

            //Find Cursor Index
            int newIndex = -1;
            FrameworkElement item;
            int count = context.Children.Count;
            for (int i = 0; i < count; ++i) {
                item = (FrameworkElement)context.Children[i];
                float itemHalfHeight = (float)(item.ActualHeight * 0.5f);

                if (MouseInput.AbsolutePosition.y < item.GetAbsolutePosition(new Vector2(0, itemHalfHeight)).y) {
                    newIndex = i;
                    break;
                }
            }

            if (newIndex == -1)
                newIndex = count;

            if (newIndex != grabbedIndex) {
                if (newIndex > grabbedIndex)
                    --newIndex;

                context.Children.Remove(grabbedItem);
                context.Children.Insert(newIndex, grabbedItem);

                OnIndexChanged?.Invoke(grabbedIndex, newIndex);
                grabbedIndex = newIndex;
            }
        };
        context.MouseUp += (object sender, MouseButtonEventArgs e) => {
            if (e.ChangedButton != MouseButton.Left)
                return;

            onDragging = false;
            Mouse.Capture(null);
        };

        ElementType GetPressedItem(FrameworkElement pressedElement) {
            //부모 트리로 Item이 나올 때까지 탐색하는 함수이다.
            DependencyObject parent = pressedElement.Parent;

            if (pressedElement is ElementType && parent == context)
                return pressedElement as ElementType;
            else if (parent != null && !(parent is Window) && parent is FrameworkElement)
                return GetPressedItem((FrameworkElement)parent);
            else
                return null;
        }
    }

    public static void SetIndexChangeable(this Control control, IndexChangedDelegate OnIndexChanged = null) {
        FrameworkElement grabbedItem = null;
        int grabbedIndex = 0;
        bool onDragging = false;
        control.MouseDown += (object sender, MouseButtonEventArgs e) => {
            if (e.ChangedButton != MouseButton.Left)
                return;

            StackPanel parentPanel = control.Parent as StackPanel;
            if (parentPanel == null)
                return;

            grabbedItem = control;
            grabbedIndex = parentPanel.Children.IndexOf(control);

            Mouse.Capture(control);
            onDragging = true;
        };
        control.MouseMove += (object sender, MouseEventArgs e) => {
            if (!onDragging)
                return;

            StackPanel parentPanel = grabbedItem.Parent as StackPanel;
            if (parentPanel == null)
                return;

            //Find Cursor Index
            int newIndex = -1;
            FrameworkElement item;
            int count = parentPanel.Children.Count;
            for (int i = 0; i < count; ++i) {
                item = (FrameworkElement)parentPanel.Children[i];
                float itemHalfHeight = (float)(item.ActualHeight * 0.5f);

                if (MouseInput.AbsolutePosition.y < item.GetAbsolutePosition(new Vector2(0, itemHalfHeight)).y) {
                    newIndex = i;
                    break;
                }
            }

            if (newIndex == -1)
                newIndex = count;

            if (newIndex != grabbedIndex) {
                if (newIndex > grabbedIndex)
                    --newIndex;

                parentPanel.Children.Remove(grabbedItem);
                parentPanel.Children.Insert(newIndex, grabbedItem);

                OnIndexChanged?.Invoke(grabbedIndex, newIndex);
                grabbedIndex = newIndex;
            }
        };
        control.MouseUp += (object sender, MouseButtonEventArgs e) => {
            if (e.ChangedButton != MouseButton.Left)
                return;

            onDragging = false;
            Mouse.Capture(null);
        };
    }

    public static T Duplicate<T>(this T reference) where T : FrameworkElement {
        return XamlReader.Parse(XamlWriter.Save(reference)) as T;
    }

    public static bool IsUserVisible(this UIElement element) {
        if (!element.IsVisible)
            return false;

        FrameworkElement container = VisualTreeHelper.GetParent(element) as FrameworkElement;
        if (container == null)
            throw new ArgumentNullException("container");

        Rect bounds = element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.RenderSize.Width, element.RenderSize.Height));
        Rect rect = new(0.0, 0.0, container.ActualWidth, container.ActualHeight);
        return rect.IntersectsWith(bounds);
    }

    public static void SetTextOptimize(this TextBox textBox, string text, int tempMemorySize = 10000000) {
        if (textBox.Text == text)
            return;

        GC.TryStartNoGCRegion(tempMemorySize);
        textBox.Text = text;
        GC.EndNoGCRegion();
    }

    # region Click events

    public static void RegisterLoadedOnce(this FrameworkElement element, RoutedEventHandler handler) {
        RoutedEventHandler unregisterEvent = null;
        unregisterEvent = UnregisterEvent;

        element.Loaded += handler;
        element.Loaded += unregisterEvent;

        void UnregisterEvent(object sender, RoutedEventArgs e) {
            element.Loaded -= handler;
            element.Loaded -= unregisterEvent;
        }
    }

    public static void RegisterLayoutUpdatedOnce(this FrameworkElement element, EventHandler handler) {
        EventHandler unregisterEvent = null;
        unregisterEvent = UnregisterEvent;

        element.LayoutUpdated += handler;
        element.LayoutUpdated += unregisterEvent;

        void UnregisterEvent(object sender, EventArgs e) {
            element.LayoutUpdated -= handler;
            element.LayoutUpdated -= unregisterEvent;
        }
    }

    public static void RegisterClickEvent(this Button button, Action action, bool handled = false) {
        button.Click += (object sender, RoutedEventArgs e) => {
            action?.Invoke();
            if (handled)
                e.Handled = true;
        };
    }

    public static void RegisterClickEvent(this Button button, ActionEvent actionEvent, bool handled = false) {
        button.Click += (object sender, RoutedEventArgs e) => {
            actionEvent?.Invoke();
            if (handled)
                e.Handled = true;
        };
    }

    public static void RegisterClickEvent(this Button button, RoutedEventHandler handler, bool handled = false) {
        button.Click += (object sender, RoutedEventArgs e) => {
            handler.Invoke(sender, e);
            if (handled)
                e.Handled = true;
        };
    }

    public static void RegisterClickEvent(this FrameworkElement control, Action action, bool handled = false, bool checkMouseUp = true) {
        control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
            if (checkMouseUp) {
                MouseInput.Left.UpOnce += () => {
                    if (control.IsMouseOver)
                        action?.Invoke();

                    if (handled)
                        e.Handled = true;
                };
            } else {
                action?.Invoke();
                if (handled)
                    e.Handled = true;
            }
        };
    }

    public static void RegisterRightClickEvent(this FrameworkElement control, Action action, bool handled = false, bool checkMouseUp = true) {
        control.MouseRightButtonDown += (object sender, MouseButtonEventArgs e) => {
            if (checkMouseUp) {
                MouseInput.Right.UpOnce += () => {
                    if (control.IsMouseOver)
                        action?.Invoke();

                    if (handled)
                        e.Handled = true;
                };
            } else {
                action?.Invoke();
                if (handled)
                    e.Handled = true;
            }
        };
    }

    public static void RegisterDoubleClickEvent(this FrameworkElement control, Action action, bool handled = false) {
        control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
            if (e.ClickCount == 2)
                action?.Invoke();

            if (handled)
                e.Handled = true;
        };
    }

    public static void RegisterButtonReaction(this Shape control, float value = DefaultCoverValue) {
        control.RegisterButtonReaction(GetCoverColor(0f).ToBrush(), GetCoverColor(value).ToBrush(), GetCoverColor(-value).ToBrush());
    }

    public static void RegisterButtonReaction(this Border control, float value = DefaultCoverValue) {
        control.RegisterButtonReaction(GetCoverColor(0f).ToBrush(), GetCoverColor(value).ToBrush(), GetCoverColor(-value).ToBrush());
    }

    public static void RegisterButtonReaction(this Panel control, float value = DefaultCoverValue) {
        control.RegisterButtonReaction(GetCoverColor(0f).ToBrush(), GetCoverColor(value).ToBrush(), GetCoverColor(-value).ToBrush());
    }

    public static void RegisterButtonReaction(this Control control, float value = DefaultCoverValue) {
        control.RegisterButtonReaction(GetCoverColor(0f).ToBrush(), GetCoverColor(value).ToBrush(), GetCoverColor(-value).ToBrush());
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
            if (control.IsMouseOver)
                over.TryInvoke();
            else
                on.TryInvoke();

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

    public static void RegisterButtonReaction(this Shape control, Color on, Color over, Color down) {
        SolidColorBrush onBrush = new(on);
        SolidColorBrush overBrush = new(over);
        SolidColorBrush downBrush = new(down);
        RegisterButtonReaction(control, onBrush, overBrush, downBrush);
    }

    public static void RegisterButtonReaction(this Shape control, SolidColorBrush on, SolidColorBrush over, SolidColorBrush down) {
        control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
            if (control.IsMouseOver)
                control.Fill = over;
            else
                control.Fill = on;

            control.ReleaseMouseCapture();
        };
        control.MouseLeave += (object sender, MouseEventArgs e) => {
            if (Mouse.LeftButton == MouseButtonState.Released)
                control.Fill = on;
        };
        control.MouseEnter += (object sender, MouseEventArgs e) => {
            if (Mouse.LeftButton == MouseButtonState.Released)
                control.Fill = over;
        };
        control.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
            control.Fill = down;
            control.CaptureMouse();
        };
    }

    public static void RegisterButtonReaction(this Border control, Action on, Action over, Action down) {
        control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
            if (control.IsMouseOver)
                over.TryInvoke();
            else
                on.TryInvoke();

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
        SolidColorBrush onBrush = new(on);
        SolidColorBrush overBrush = new(over);
        SolidColorBrush downBrush = new(down);
        RegisterButtonReaction(control, onBrush, overBrush, downBrush);
    }

    public static void RegisterButtonReaction(this Border control, SolidColorBrush on, SolidColorBrush over, SolidColorBrush down) {
        control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
            if (control.IsMouseOver)
                control.Background = over;
            else
                control.Background = on;

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
            if (control.IsMouseOver)
                over.TryInvoke();
            else
                on.TryInvoke();

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
        SolidColorBrush onBrush = new(on);
        SolidColorBrush overBrush = new(over);
        SolidColorBrush downBrush = new(down);
        RegisterButtonReaction(control, onBrush, overBrush, downBrush);
    }

    public static void RegisterButtonReaction(this Panel control, SolidColorBrush on, SolidColorBrush over, SolidColorBrush down) {
        control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
            if (control.IsMouseOver)
                control.Background = over;
            else
                control.Background = on;

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
            if (control.IsMouseOver)
                over.TryInvoke();
            else
                on.TryInvoke();

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
        SolidColorBrush onBrush = new(on);
        SolidColorBrush overBrush = new(over);
        SolidColorBrush downBrush = new(down);
        RegisterButtonReaction(control, onBrush, overBrush, downBrush);
    }

    public static void RegisterButtonReaction(this Control control, SolidColorBrush on, SolidColorBrush over, SolidColorBrush down) {
        control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
            if (control.IsMouseOver)
                control.Background = over;
            else
                control.Background = on;

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

    #endregion
}