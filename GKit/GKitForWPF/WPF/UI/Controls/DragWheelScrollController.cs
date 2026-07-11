using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace GKitForWPF.UI.Controls;

/// <summary>
/// Provides wheel and edge scrolling while a WPF/OLE drag loop is active.
/// OLE does not reliably route WM_MOUSEWHEEL to the list HWND, so a short-lived
/// low-level mouse hook is installed only for the lifetime of the drag.
/// </summary>
public sealed class DragWheelScrollController : IDisposable {
    private const int WhMouseLl = 14;
    private const int WmMouseWheel = 0x020A;
    private const int WheelDelta = 120;
    private const double AutoScrollEdgeSize = 36d;
    private const int AutoScrollIntervalMilliseconds = 45;

    private readonly FrameworkElement hitTestHost;
    private readonly ScrollViewer scrollViewer;
    private readonly HwndSource hwndSource;
    private readonly DispatcherTimer autoScrollTimer;
    private readonly LowLevelMouseProc lowLevelMouseProc;
    private IntPtr mouseHook;
    private bool usesWindowMessageFallback;
    private bool usesWpfWheelInput;
    private bool disposed;

    public event Action Scrolled;

    private DragWheelScrollController(FrameworkElement hitTestHost, ScrollViewer scrollViewer,
        HwndSource hwndSource, bool oleDrag) {
        this.hitTestHost = hitTestHost;
        this.scrollViewer = scrollViewer;
        this.hwndSource = hwndSource;

        lowLevelMouseProc = OnLowLevelMouse;
        if (oleDrag) {
            mouseHook = SetWindowsHookEx(WhMouseLl, lowLevelMouseProc, IntPtr.Zero, 0);
            if (mouseHook == IntPtr.Zero) {
                usesWindowMessageFallback = true;
                hwndSource.AddHook(WndProc);
            }
        } else {
            usesWpfWheelInput = true;
            hitTestHost.PreviewMouseWheel += OnPreviewMouseWheel;
        }

        autoScrollTimer = new DispatcherTimer(DispatcherPriority.Input, hitTestHost.Dispatcher) {
            Interval = TimeSpan.FromMilliseconds(AutoScrollIntervalMilliseconds)
        };
        autoScrollTimer.Tick += OnAutoScrollTick;
        autoScrollTimer.Start();
    }

    public static DragWheelScrollController TryCreate(FrameworkElement host, bool oleDrag = true) {
        if (host == null || !host.IsVisible)
            return null;
        ScrollViewer scrollViewer = FindVisualChild<ScrollViewer>(host);
        if (scrollViewer == null && host is ScrollViewer directScrollViewer)
            scrollViewer = directScrollViewer;
        HwndSource source = PresentationSource.FromVisual(host) as HwndSource;
        return scrollViewer == null || source == null
            ? null
            : new DragWheelScrollController(host, scrollViewer, source, oleDrag);
    }

    public bool TryGetPointerPosition(out Point point) {
        point = default;
        if (disposed || !GetCursorPos(out NativePoint cursor))
            return false;
        try {
            point = hitTestHost.PointFromScreen(new Point(cursor.X, cursor.Y));
            return true;
        } catch (InvalidOperationException) {
            return false;
        }
    }

    private IntPtr OnLowLevelMouse(int code, IntPtr message, IntPtr dataPointer) {
        if (!disposed && code >= 0 && message.ToInt32() == WmMouseWheel) {
            LowLevelMouseData data = Marshal.PtrToStructure<LowLevelMouseData>(dataPointer);
            Point screenPoint = new(data.Point.X, data.Point.Y);
            int delta = unchecked((short)((data.MouseData >> 16) & 0xffff));
            if (delta != 0 && IsPointOverHost(screenPoint)) {
                // Leave the hook immediately; scrolling/layout runs on the next
                // input dispatcher turn inside the nested OLE message loop.
                hitTestHost.Dispatcher.BeginInvoke(() => {
                    if (!disposed)
                        ScrollWheel(delta);
                }, DispatcherPriority.Input);
            }
        }
        return CallNextHookEx(mouseHook, code, message, dataPointer);
    }

    private IntPtr WndProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled) {
        if (disposed || message != WmMouseWheel)
            return IntPtr.Zero;

        Point screenPoint = new((short)(lParam.ToInt64() & 0xffff),
            (short)((lParam.ToInt64() >> 16) & 0xffff));
        int delta = (short)((wParam.ToInt64() >> 16) & 0xffff);
        if (!IsPointOverHost(screenPoint) || delta == 0)
            return IntPtr.Zero;

        ScrollWheel(delta);
        handled = true;
        return IntPtr.Zero;
    }

    private bool IsPointOverHost(Point screenPoint) {
        Point localPoint;
        try {
            localPoint = hitTestHost.PointFromScreen(screenPoint);
        } catch (InvalidOperationException) {
            return false;
        }
        return localPoint.X >= 0d && localPoint.Y >= 0d &&
               localPoint.X <= hitTestHost.ActualWidth && localPoint.Y <= hitTestHost.ActualHeight;
    }

    private void ScrollWheel(int delta) {
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
        Scrolled?.Invoke();
    }

    private void OnPreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e) {
        ScrollWheel(e.Delta);
        e.Handled = true;
    }

    private void OnAutoScrollTick(object sender, EventArgs e) {
        if (!TryGetPointerPosition(out Point point))
            return;
        if (point.X < 0d || point.X > hitTestHost.ActualWidth ||
            point.Y < 0d || point.Y > hitTestHost.ActualHeight)
            return;

        double previousOffset = scrollViewer.VerticalOffset;
        if (point.Y < AutoScrollEdgeSize) {
            scrollViewer.LineUp();
            if (point.Y < AutoScrollEdgeSize * 0.35d)
                scrollViewer.LineUp();
        } else if (point.Y > hitTestHost.ActualHeight - AutoScrollEdgeSize) {
            scrollViewer.LineDown();
            if (point.Y > hitTestHost.ActualHeight - AutoScrollEdgeSize * 0.35d)
                scrollViewer.LineDown();
        } else {
            return;
        }

        if (Math.Abs(previousOffset - scrollViewer.VerticalOffset) >= 0.001d)
            Scrolled?.Invoke();
    }

    public void Dispose() {
        if (disposed)
            return;
        disposed = true;
        autoScrollTimer.Stop();
        autoScrollTimer.Tick -= OnAutoScrollTick;
        if (mouseHook != IntPtr.Zero) {
            UnhookWindowsHookEx(mouseHook);
            mouseHook = IntPtr.Zero;
        }
        if (usesWindowMessageFallback)
            hwndSource.RemoveHook(WndProc);
        if (usesWpfWheelInput)
            hitTestHost.PreviewMouseWheel -= OnPreviewMouseWheel;
        Scrolled = null;
    }

    private delegate IntPtr LowLevelMouseProc(int code, IntPtr message, IntPtr dataPointer);

    [StructLayout(LayoutKind.Sequential)]
    private struct LowLevelMouseData {
        public NativePoint Point;
        public uint MouseData;
        public uint Flags;
        public uint Time;
        public UIntPtr ExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int hookId, LowLevelMouseProc callback,
        IntPtr moduleHandle, uint threadId);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hook);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hook, int code, IntPtr message, IntPtr dataPointer);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out NativePoint point);

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
