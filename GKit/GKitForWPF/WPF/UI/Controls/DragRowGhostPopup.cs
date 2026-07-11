using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GKitForWPF.UI.Controls;

/// <summary>
/// A bitmap-backed drag preview hosted in a separate HWND. The popup stays
/// below the native cursor, so it cannot replace the real OLE drop-target HWND.
/// Moving it does not invalidate the source list or its adorner layer.
/// </summary>
public sealed class DragRowGhostPopup : IDisposable {
    private const int FollowIntervalMilliseconds = 16;
    private const double CursorGap = 12d;
    private static readonly Brush InvalidOverlayBrush = CreateFrozenBrush(Color.FromArgb(105, 220, 45, 45));
    private static readonly Brush InvalidBorderBrush = CreateFrozenBrush(Color.FromRgb(245, 70, 70));
    private static readonly Brush NormalBorderBrush = CreateFrozenBrush(Color.FromArgb(145, 219, 181, 94));

    private readonly FrameworkElement host;
    private readonly Popup popup;
    private readonly Border invalidOverlay;
    private readonly DispatcherTimer followTimer;
    private double lastVerticalOffset = double.NaN;
    private bool isInvalid;
    private bool disposed;

    private DragRowGhostPopup(FrameworkElement host, FrameworkElement row) {
        this.host = host;
        double rowWidth = Math.Max(1d, row.ActualWidth);
        double rowHeight = Math.Max(1d, row.ActualHeight);

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

        followTimer = new DispatcherTimer(DispatcherPriority.Input, host.Dispatcher) {
            Interval = TimeSpan.FromMilliseconds(FollowIntervalMilliseconds)
        };
        followTimer.Tick += OnFollowTimerTick;
    }

    public static DragRowGhostPopup TryCreate(FrameworkElement host, FrameworkElement row) {
        if (host == null || row == null || row.ActualWidth <= 0d || row.ActualHeight <= 0d ||
            !host.IsVisible)
            return null;

        DragRowGhostPopup ghost = new(host, row);
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

    private void UpdatePosition() {
        if (disposed || !GetCursorPos(out NativePoint point))
            return;
        try {
            Point relativePoint = host.PointFromScreen(new Point(point.X, point.Y));
            // Never cover the cursor: OLE resolves its target from the native
            // window directly under this point, not WPF IsHitTestVisible.
            double verticalOffset = relativePoint.Y + CursorGap;
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

    private static Brush CreateFrozenBrush(Color color) {
        SolidColorBrush brush = new(color);
        brush.Freeze();
        return brush;
    }
}
