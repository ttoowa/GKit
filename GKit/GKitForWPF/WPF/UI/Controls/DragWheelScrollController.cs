using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace GKitForWPF.UI.Controls;

/// <summary>
/// Routes native mouse-wheel messages to a virtualized list while WPF/OLE drag
/// is active. OLE's nested drag loop does not reliably raise MouseWheel events.
/// </summary>
public sealed class DragWheelScrollController : IDisposable {
    private const int WmMouseWheel = 0x020A;
    private const int WheelDelta = 120;

    private readonly FrameworkElement hitTestHost;
    private readonly ScrollViewer scrollViewer;
    private readonly HwndSource hwndSource;
    private bool disposed;

    public event Action Scrolled;

    private DragWheelScrollController(FrameworkElement hitTestHost, ScrollViewer scrollViewer,
        HwndSource hwndSource) {
        this.hitTestHost = hitTestHost;
        this.scrollViewer = scrollViewer;
        this.hwndSource = hwndSource;
        hwndSource.AddHook(WndProc);
    }

    public static DragWheelScrollController TryCreate(FrameworkElement host) {
        if (host == null || !host.IsVisible)
            return null;
        ScrollViewer scrollViewer = FindVisualChild<ScrollViewer>(host);
        if (scrollViewer == null && host is ScrollViewer directScrollViewer)
            scrollViewer = directScrollViewer;
        HwndSource source = PresentationSource.FromVisual(host) as HwndSource;
        return scrollViewer == null || source == null
            ? null
            : new DragWheelScrollController(host, scrollViewer, source);
    }

    private IntPtr WndProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled) {
        if (disposed || message != WmMouseWheel)
            return IntPtr.Zero;

        Point screenPoint = new((short)(lParam.ToInt64() & 0xffff),
            (short)((lParam.ToInt64() >> 16) & 0xffff));
        Point localPoint;
        try {
            localPoint = hitTestHost.PointFromScreen(screenPoint);
        } catch (InvalidOperationException) {
            return IntPtr.Zero;
        }
        if (localPoint.X < 0d || localPoint.Y < 0d ||
            localPoint.X > hitTestHost.ActualWidth || localPoint.Y > hitTestHost.ActualHeight)
            return IntPtr.Zero;

        int delta = (short)((wParam.ToInt64() >> 16) & 0xffff);
        if (delta == 0)
            return IntPtr.Zero;
        int notchCount = Math.Max(1, Math.Abs(delta) / WheelDelta);
        int configuredLines = SystemParameters.WheelScrollLines;
        if (configuredLines < 0) {
            for (int i = 0; i < notchCount; ++i) {
                if (delta > 0)
                    scrollViewer.PageUp();
                else
                    scrollViewer.PageDown();
            }
        } else {
            int lineCount = notchCount * Math.Max(1, configuredLines);
            for (int i = 0; i < lineCount; ++i) {
                if (delta > 0)
                    scrollViewer.LineUp();
                else
                    scrollViewer.LineDown();
            }
        }

        handled = true;
        scrollViewer.Dispatcher.BeginInvoke(() => {
            if (!disposed)
                Scrolled?.Invoke();
        }, DispatcherPriority.Input);
        return IntPtr.Zero;
    }

    public void Dispose() {
        if (disposed)
            return;
        disposed = true;
        hwndSource.RemoveHook(WndProc);
        Scrolled = null;
    }

    private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject {
        if (parent is T match)
            return match;
        int childCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childCount; ++i) {
            if (FindVisualChild<T>(VisualTreeHelper.GetChild(parent, i)) is T child)
                return child;
        }
        return null;
    }
}
