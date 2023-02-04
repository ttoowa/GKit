using UnityEngine;

namespace GKitForUnity;

[RequireComponent(typeof(RectTransform))]
public class UiTransform : MonoBehaviour {
    private HorizontalAlignment horizontalAlignment;

    public HorizontalAlignment HorizontalAlignment {
        get => horizontalAlignment;
        set {
            horizontalAlignment = value;
            RectTransform.SetHorizontalAlignment(value);
        }
    }

    private VerticalAlignment verticalAlignment;

    public VerticalAlignment VerticalAlignment {
        get => verticalAlignment;
        set {
            verticalAlignment = value;
            RectTransform.SetVerticalAlignment(value);
        }
    }

    public Vector2 Position {
        get => RectTransform.anchoredPosition;
        set => RectTransform.anchoredPosition = value;
    }

    public Vector2 SizeDelta {
        get => RectTransform.sizeDelta;
        set => RectTransform.sizeDelta = value;
    }

    public Vector2 Pivot {
        get => RectTransform.pivot;
        set => RectTransform.pivot = value;
    }

    public RectTransform RectTransform { get; private set; }

    private void Awake() {
        RectTransform = GetComponent<RectTransform>();
    }

    public void SetHorizontalAlignment(HorizontalAlignment alignment, bool setPivotAuto = true) {
        HorizontalAlignment = alignment;

        if (setPivotAuto) {
            Pivot = new Vector2(UIUtility.GetPivotPosition(alignment), Pivot.y);
        }
    }

    public void SetVerticalAlignment(VerticalAlignment alignment, bool setPivotAuto = true) {
        VerticalAlignment = alignment;

        if (setPivotAuto) {
            Pivot = new Vector2(Pivot.x, UIUtility.GetPivotPosition(alignment));
        }
    }
}