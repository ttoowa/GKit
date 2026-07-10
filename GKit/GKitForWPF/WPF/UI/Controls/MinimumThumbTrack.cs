using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace GKitForWPF.UI.Controls;

/// <summary>
/// Keeps a proportional scrollbar thumb at or above a minimum length without
/// allowing it to extend outside the Track. WPF's Track honors Thumb.MinWidth /
/// MinHeight for rendering, but still positions a clamped thumb using its
/// smaller proportional length, which clips it near the end of the range.
/// </summary>
public sealed class MinimumThumbTrack : Track {
    private bool usesCorrectedLayout;
    private double correctedThumbCenter;
    private double correctedValuePerPixel;

    public static readonly DependencyProperty MinimumThumbLengthProperty =
        DependencyProperty.Register(
            nameof(MinimumThumbLength),
            typeof(double),
            typeof(MinimumThumbTrack),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange),
            value => value is double length && double.IsFinite(length) && length >= 0d);

    public double MinimumThumbLength {
        get => (double)GetValue(MinimumThumbLengthProperty);
        set => SetValue(MinimumThumbLengthProperty, value);
    }

    protected override Size ArrangeOverride(Size arrangeSize) {
        Size result = base.ArrangeOverride(arrangeSize);
        usesCorrectedLayout = false;

        Thumb thumb = Thumb;
        if (thumb == null || thumb.Visibility == Visibility.Collapsed || MinimumThumbLength <= 0d) {
            return result;
        }

        bool isVertical = Orientation == Orientation.Vertical;
        double trackLength = isVertical ? arrangeSize.Height : arrangeSize.Width;
        double proportionalLength = isVertical ? thumb.RenderSize.Height : thumb.RenderSize.Width;
        double thumbLength = Math.Min(trackLength, Math.Max(proportionalLength, MinimumThumbLength));

        if (trackLength <= 0d || thumbLength <= proportionalLength + 0.01d) {
            return result;
        }

        double range = Math.Max(0d, Maximum - Minimum);
        double valueRatio = range > 0d
            ? Math.Clamp((Value - Minimum) / range, 0d, 1d)
            : 0d;
        double travelLength = Math.Max(0d, trackLength - thumbLength);
        bool coordinateIncreasesValue = isVertical ? IsDirectionReversed : !IsDirectionReversed;
        double thumbStart = travelLength * (coordinateIncreasesValue ? valueRatio : 1d - valueRatio);

        UIElement startButton = coordinateIncreasesValue ? DecreaseRepeatButton : IncreaseRepeatButton;
        UIElement endButton = coordinateIncreasesValue ? IncreaseRepeatButton : DecreaseRepeatButton;

        ArrangeElement(startButton, 0d, thumbStart, arrangeSize, isVertical);
        ArrangeElement(thumb, thumbStart, thumbLength, arrangeSize, isVertical);
        ArrangeElement(endButton, thumbStart + thumbLength, trackLength - thumbStart - thumbLength, arrangeSize, isVertical);

        correctedValuePerPixel = travelLength > 0d ? range / travelLength : 0d;
        correctedThumbCenter = thumbStart + thumbLength * 0.5d;
        usesCorrectedLayout = true;
        return result;
    }

    public override double ValueFromDistance(double horizontal, double vertical) {
        if (!usesCorrectedLayout) {
            return base.ValueFromDistance(horizontal, vertical);
        }

        bool isVertical = Orientation == Orientation.Vertical;
        bool coordinateIncreasesValue = isVertical ? IsDirectionReversed : !IsDirectionReversed;
        double distance = isVertical ? vertical : horizontal;
        return distance * (coordinateIncreasesValue ? 1d : -1d) * correctedValuePerPixel;
    }

    public override double ValueFromPoint(Point point) {
        if (!usesCorrectedLayout) {
            return base.ValueFromPoint(point);
        }

        bool isVertical = Orientation == Orientation.Vertical;
        double distance = (isVertical ? point.Y : point.X) - correctedThumbCenter;
        double valueDelta = isVertical
            ? ValueFromDistance(0d, distance)
            : ValueFromDistance(distance, 0d);
        return Math.Clamp(Value + valueDelta, Minimum, Maximum);
    }

    private static void ArrangeElement(UIElement element, double start, double length, Size arrangeSize, bool isVertical) {
        if (element == null) {
            return;
        }

        Rect bounds = isVertical
            ? new Rect(0d, start, arrangeSize.Width, Math.Max(0d, length))
            : new Rect(start, 0d, Math.Max(0d, length), arrangeSize.Height);
        element.Arrange(bounds);
    }
}
