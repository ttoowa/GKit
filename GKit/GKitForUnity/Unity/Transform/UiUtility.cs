using UnityEngine;

namespace GKitForUnity;

public static class UIUtility {
    public static void SetHorizontalAlignment(this RectTransform rectTransform, HorizontalAlignment alignment) {
        GRange anchorRange = GetAnchorRange(alignment);
        rectTransform.anchorMin = new Vector2(anchorRange.min, rectTransform.anchorMin.y);
        rectTransform.anchorMax = new Vector2(anchorRange.max, rectTransform.anchorMax.y);
    }

    public static void SetVerticalAlignment(this RectTransform rectTransform, VerticalAlignment alignment) {
        GRange anchorRange = GetAnchorRange(alignment);
        rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, anchorRange.min);
        rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, anchorRange.max);
    }

    public static void SetHorizontalPivot(this RectTransform rectTransform, HorizontalAlignment alignment) {
        rectTransform.pivot = new Vector2(GetPivotPosition(alignment), rectTransform.pivot.y);
    }

    public static void SetVerticalPivot(this RectTransform rectTransform, VerticalAlignment alignment) {
        rectTransform.pivot = new Vector2(rectTransform.pivot.x, GetPivotPosition(alignment));
    }

    public static GRange GetAnchorRange(HorizontalAlignment alignment) {
        float anchorMin;
        float anchorMax;
        switch (alignment) {
            default:
            case HorizontalAlignment.Stretch:
                anchorMin = 0f;
                anchorMax = 1f;
                break;
            case HorizontalAlignment.Left:
                anchorMin = anchorMax = 0f;
                break;
            case HorizontalAlignment.Center:
                anchorMin = anchorMax = 0.5f;
                break;
            case HorizontalAlignment.Right:
                anchorMin = anchorMax = 1f;
                break;
        }

        return new GRange(anchorMin, anchorMax);
    }

    public static GRange GetAnchorRange(VerticalAlignment alignment) {
        float anchorMin;
        float anchorMax;
        switch (alignment) {
            default:
            case VerticalAlignment.Stretch:
                anchorMin = 0f;
                anchorMax = 1f;
                break;
            case VerticalAlignment.Bottom:
                anchorMin = anchorMax = 0f;
                break;
            case VerticalAlignment.Center:
                anchorMin = anchorMax = 0.5f;
                break;
            case VerticalAlignment.Top:
                anchorMin = anchorMax = 1f;
                break;
        }

        return new GRange(anchorMin, anchorMax);
    }

    public static float GetPivotPosition(HorizontalAlignment alignment) {
        switch (alignment) {
            case HorizontalAlignment.Left:
                return 0f;
            default:
            case HorizontalAlignment.Stretch:
            case HorizontalAlignment.Center:
                return 0.5f;
            case HorizontalAlignment.Right:
                return 1f;
        }
    }

    public static float GetPivotPosition(VerticalAlignment alignment) {
        switch (alignment) {
            case VerticalAlignment.Bottom:
                return 0f;
            default:
            case VerticalAlignment.Stretch:
            case VerticalAlignment.Center:
                return 0.5f;
            case VerticalAlignment.Top:
                return 1f;
        }
    }

    //public static Vector2 GetPreferredSize(this RectTransform rectTransform) {
    //	return new Vector2(
    //		LayoutUtility.GetPreferredSize(rectTransform, 0),
    //		LayoutUtility.GetPreferredSize(rectTransform, 1));
    //}
}