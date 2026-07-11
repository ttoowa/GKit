using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GKitForWPF.UI.Controls;

public enum DropTargetIndicatorMode { Before, Inside, After }

/// <summary>
/// Draws a drop target in one persistent adorner. Updating the target only
/// invalidates this tiny drawing layer; it never moves a Popup HWND and never
/// changes a row's layout properties.
/// </summary>
public sealed class DropTargetIndicatorPopup : IDisposable {
    private AdornerLayer adornerLayer;
    private DropTargetIndicatorAdorner adorner;
    private FrameworkElement host;
    private bool disposed;

    public void Show(FrameworkElement target, DropTargetIndicatorMode mode) {
        if (disposed || target == null || target.ActualWidth <= 0d || target.ActualHeight <= 0d)
            return;

        FrameworkElement targetHost = FindListHost(target);
        if (targetHost == null)
            return;
        if (!ReferenceEquals(host, targetHost)) {
            DetachAdorner();
            host = targetHost;
        }
        AdornerLayer currentLayer = AdornerLayer.GetAdornerLayer(host);
        if (adorner != null && !ReferenceEquals(adornerLayer, currentLayer)) {
            DetachAdorner();
            host = targetHost;
        }
        if (adorner == null) {
            adornerLayer = currentLayer ?? AdornerLayer.GetAdornerLayer(host);
            if (adornerLayer == null)
                return;
            adorner = new DropTargetIndicatorAdorner(host);
            adornerLayer.Add(adorner);
        }

        Point origin;
        try {
            origin = target.TranslatePoint(new Point(), host);
        } catch (InvalidOperationException) {
            Hide();
            return;
        }
        adorner.Update(new Rect(origin, target.RenderSize), mode);
    }

    public void Hide() {
        if (!disposed)
            adorner?.Clear();
    }

    public void Dispose() {
        if (disposed)
            return;
        disposed = true;
        DetachAdorner();
        host = null;
    }

    private void DetachAdorner() {
        if (adornerLayer != null && adorner != null &&
            ReferenceEquals(VisualTreeHelper.GetParent(adorner), adornerLayer))
            adornerLayer.Remove(adorner);
        adorner = null;
        adornerLayer = null;
    }

    private static FrameworkElement FindListHost(DependencyObject child) {
        for (DependencyObject current = child; current != null; current = VisualTreeHelper.GetParent(current)) {
            if (current is ListBox listBox)
                return listBox;
        }
        return null;
    }

    private sealed class DropTargetIndicatorAdorner : Adorner {
        private static readonly Brush IndicatorBrush = CreateFrozenBrush(Color.FromRgb(219, 181, 94));
        private static readonly Brush InsideBrush = CreateFrozenBrush(Color.FromArgb(38, 219, 181, 94));
        private static readonly Pen IndicatorPen = CreateFrozenPen(IndicatorBrush, 2d);

        private Rect targetBounds = Rect.Empty;
        private DropTargetIndicatorMode mode;

        public DropTargetIndicatorAdorner(UIElement adornedElement) : base(adornedElement) {
            IsHitTestVisible = false;
            Focusable = false;
            ClipToBounds = true;
            SnapsToDevicePixels = true;
            UseLayoutRounding = true;
        }

        public void Update(Rect bounds, DropTargetIndicatorMode newMode) {
            if (targetBounds == bounds && mode == newMode)
                return;
            targetBounds = bounds;
            mode = newMode;
            InvalidateVisual();
        }

        public void Clear() {
            if (targetBounds.IsEmpty)
                return;
            targetBounds = Rect.Empty;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
            if (targetBounds.IsEmpty)
                return;

            switch (mode) {
                case DropTargetIndicatorMode.Before: {
                    double y = targetBounds.Top + 1d;
                    drawingContext.DrawLine(IndicatorPen,
                        new Point(targetBounds.Left, y), new Point(targetBounds.Right, y));
                    break;
                }
                case DropTargetIndicatorMode.After: {
                    double y = targetBounds.Bottom - 1d;
                    drawingContext.DrawLine(IndicatorPen,
                        new Point(targetBounds.Left, y), new Point(targetBounds.Right, y));
                    break;
                }
                default: {
                    Rect insideBounds = targetBounds;
                    insideBounds.Inflate(-1d, -1d);
                    if (insideBounds.Width > 0d && insideBounds.Height > 0d)
                        drawingContext.DrawRectangle(InsideBrush, IndicatorPen, insideBounds);
                    break;
                }
            }
        }

        private static Brush CreateFrozenBrush(Color color) {
            SolidColorBrush brush = new(color);
            brush.Freeze();
            return brush;
        }

        private static Pen CreateFrozenPen(Brush brush, double thickness) {
            Pen pen = new(brush, thickness);
            pen.Freeze();
            return pen;
        }
    }
}
