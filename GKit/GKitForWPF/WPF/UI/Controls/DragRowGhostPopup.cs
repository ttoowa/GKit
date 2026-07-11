using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace GKitForWPF.UI.Controls;

/// <summary>
/// A non-activating transparent popup used as a drag preview. The popup owns a
/// separate HWND, so it is not clipped or laid out by the source list/AvalonDock.
/// Only its window offset is updated while dragging; the row visual is reused.
/// </summary>
public sealed class DragRowGhostPopup : IDisposable {
    private const int FollowIntervalMilliseconds = 16;
    private static readonly Brush InvalidOverlayBrush = CreateFrozenBrush(Color.FromArgb(105, 220, 45, 45));
    private static readonly Brush InvalidBorderBrush = CreateFrozenBrush(Color.FromRgb(245, 70, 70));
    private static readonly Brush NormalBorderBrush = CreateFrozenBrush(Color.FromArgb(145, 219, 181, 94));

    private readonly FrameworkElement host;
    private readonly Popup popup;
    private readonly Border invalidOverlay;
    private readonly VisualBrush rowBrush;
    private readonly DispatcherTimer followTimer;
    private readonly double rowHeight;
    private bool isInvalid;
    private bool disposed;

    private DragRowGhostPopup(FrameworkElement host, FrameworkElement row) {
        this.host = host;
        double rowWidth = Math.Max(1d, row.ActualWidth);
        rowHeight = Math.Max(1d, row.ActualHeight);
        rowBrush = new VisualBrush(row) {
            AlignmentX = AlignmentX.Left,
            AlignmentY = AlignmentY.Top,
            Stretch = Stretch.Fill
        };

        Grid ghost = new() {
            Width = rowWidth,
            Height = rowHeight,
            IsHitTestVisible = false,
            Focusable = false
        };
        ghost.Children.Add(new Border {
            Background = rowBrush,
            Opacity = 0.68d
        });
        invalidOverlay = new Border {
            Background = Brushes.Transparent,
            BorderBrush = NormalBorderBrush,
            BorderThickness = new Thickness(1d)
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
        Point relativePoint = host.PointFromScreen(new Point(point.X, point.Y));
        // Keep X aligned with the list; only Y follows the OS cursor.
        popup.HorizontalOffset = 0d;
        popup.VerticalOffset = relativePoint.Y - rowHeight * 0.5d;
    }

    public void Dispose() {
        if (disposed)
            return;
        disposed = true;
        followTimer.Stop();
        followTimer.Tick -= OnFollowTimerTick;
        popup.IsOpen = false;
        popup.Child = null;
        rowBrush.Visual = null;
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
