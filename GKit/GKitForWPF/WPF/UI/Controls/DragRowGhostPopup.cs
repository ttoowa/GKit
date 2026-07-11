using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GKitForWPF.UI.Controls;

/// <summary>
/// A bitmap-backed drag preview hosted in a separate, hit-test-transparent HWND.
/// Moving it does not invalidate the source list or its adorner layer.
/// </summary>
public sealed class DragRowGhostPopup : IDisposable {
    private const int FollowIntervalMilliseconds = 16;
    private const int WmNcHitTest = 0x0084;
    private const int HtTransparent = -1;
    private const int GwlExStyle = -20;
    private const int WsExTransparent = 0x00000020;
    private const int WsExToolWindow = 0x00000080;
    private const int WsExNoActivate = 0x08000000;
    private static readonly Brush InvalidOverlayBrush = CreateFrozenBrush(Color.FromArgb(105, 220, 45, 45));
    private static readonly Brush InvalidBorderBrush = CreateFrozenBrush(Color.FromRgb(245, 70, 70));
    private static readonly Brush NormalBorderBrush = CreateFrozenBrush(Color.FromArgb(145, 219, 181, 94));

    private readonly FrameworkElement host;
    private readonly Popup popup;
    private readonly Border invalidOverlay;
    private readonly DispatcherTimer followTimer;
    private readonly double pointerOffsetY;
    private HwndSource popupSource;
    private double lastVerticalOffset = double.NaN;
    private bool isInvalid;
    private bool disposed;

    private DragRowGhostPopup(FrameworkElement host, FrameworkElement row, double pointerOffsetY) {
        this.host = host;
        double rowWidth = Math.Max(1d, row.ActualWidth);
        double rowHeight = Math.Max(1d, row.ActualHeight);
        this.pointerOffsetY = double.IsNaN(pointerOffsetY)
            ? rowHeight * 0.5d
            : Math.Clamp(pointerOffsetY, 0d, rowHeight);

        Grid ghost = new() {
            Width = rowWidth,
            Height = rowHeight,
            IsHitTestVisible = false,
            Focusable = false
        };
        ghost.Children.Add(new Image {
            Source = CaptureRow(row, rowWidth, rowHeight),
            Stretch = Stretch.Fill,
            Opacity = 0.68d,
            IsHitTestVisible = false
        });
        invalidOverlay = new Border {
            Background = Brushes.Transparent,
            BorderBrush = NormalBorderBrush,
            BorderThickness = new Thickness(1d),
            IsHitTestVisible = false
        };
        ghost.Children.Add(invalidOverlay);

        popup = new Popup {
            AllowsTransparency = true,
            StaysOpen = true,
            IsHitTestVisible = false,
            Focusable = false,
            PopupAnimation = PopupAnimation.None,
            Placement = PlacementMode.RelativePoint,
            PlacementTarget = host,
            Child = ghost
        };
        popup.Opened += OnPopupOpened;

        followTimer = new DispatcherTimer(DispatcherPriority.Input, host.Dispatcher) {
            Interval = TimeSpan.FromMilliseconds(FollowIntervalMilliseconds)
        };
        followTimer.Tick += OnFollowTimerTick;
    }

    public static DragRowGhostPopup TryCreate(FrameworkElement host, FrameworkElement row,
        double pointerOffsetY = double.NaN) {
        if (host == null || row == null || row.ActualWidth <= 0d || row.ActualHeight <= 0d ||
            !host.IsVisible)
            return null;

        DragRowGhostPopup ghost = new(host, row, pointerOffsetY);
        ghost.UpdatePosition();
        ghost.popup.IsOpen = true;
        ghost.followTimer.Start();
        return ghost;
    }

    public void SetInvalid(bool invalid) {
        if (disposed || isInvalid == invalid)
            return;
        isInvalid = invalid;
        invalidOverlay.Background = invalid ? InvalidOverlayBrush : Brushes.Transparent;
        invalidOverlay.BorderBrush = invalid ? InvalidBorderBrush : NormalBorderBrush;
        invalidOverlay.BorderThickness = new Thickness(invalid ? 2d : 1d);
    }

    private void OnFollowTimerTick(object sender, EventArgs e) => UpdatePosition();

    private void OnPopupOpened(object sender, EventArgs e) {
        popupSource = PresentationSource.FromVisual(popup.Child) as HwndSource;
        if (popupSource == null)
            return;
        popupSource.AddHook(PopupWndProc);

        IntPtr handle = popupSource.Handle;
        IntPtr style = GetWindowLongPtr(handle, GwlExStyle);
        long transparentStyle = style.ToInt64() | WsExTransparent | WsExToolWindow | WsExNoActivate;
        SetWindowLongPtr(handle, GwlExStyle, new IntPtr(transparentStyle));
    }

    private IntPtr PopupWndProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled) {
        if (message != WmNcHitTest)
            return IntPtr.Zero;
        handled = true;
        return new IntPtr(HtTransparent);
    }

    private void UpdatePosition() {
        if (disposed || !GetCursorPos(out NativePoint point))
            return;
        try {
            Point relativePoint = host.PointFromScreen(new Point(point.X, point.Y));
            double verticalOffset = relativePoint.Y - pointerOffsetY;
            if (!double.IsNaN(lastVerticalOffset) && Math.Abs(lastVerticalOffset - verticalOffset) < 0.5d)
                return;
            lastVerticalOffset = verticalOffset;
            popup.VerticalOffset = verticalOffset;
        } catch (InvalidOperationException) {
            // The host may briefly lose its presentation source during docking.
        }
    }

    public void Dispose() {
        if (disposed)
            return;
        disposed = true;
        followTimer.Stop();
        followTimer.Tick -= OnFollowTimerTick;
        popup.Opened -= OnPopupOpened;
        popupSource?.RemoveHook(PopupWndProc);
        popupSource = null;
        popup.IsOpen = false;
        popup.Child = null;
    }

    private static ImageSource CaptureRow(FrameworkElement row, double width, double height) {
        try {
            DpiScale dpi = VisualTreeHelper.GetDpi(row);
            int pixelWidth = Math.Max(1, (int)Math.Ceiling(width * dpi.DpiScaleX));
            int pixelHeight = Math.Max(1, (int)Math.Ceiling(height * dpi.DpiScaleY));
            RenderTargetBitmap bitmap = new(pixelWidth, pixelHeight,
                96d * dpi.DpiScaleX, 96d * dpi.DpiScaleY, PixelFormats.Pbgra32);
            bitmap.Render(row);
            bitmap.Freeze();
            return bitmap;
        } catch (InvalidOperationException) {
            return null;
        }
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out NativePoint point);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint {
        public int X;
        public int Y;
    }

    private static IntPtr GetWindowLongPtr(IntPtr window, int index) => IntPtr.Size == 8
        ? GetWindowLongPtr64(window, index)
        : new IntPtr(GetWindowLong32(window, index));

    private static IntPtr SetWindowLongPtr(IntPtr window, int index, IntPtr value) => IntPtr.Size == 8
        ? SetWindowLongPtr64(window, index, value)
        : new IntPtr(SetWindowLong32(window, index, value.ToInt32()));

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    private static extern int GetWindowLong32(IntPtr window, int index);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    private static extern IntPtr GetWindowLongPtr64(IntPtr window, int index);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(IntPtr window, int index, int value);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(IntPtr window, int index, IntPtr value);

    private static Brush CreateFrozenBrush(Color color) {
        SolidColorBrush brush = new(color);
        brush.Freeze();
        return brush;
    }
}
