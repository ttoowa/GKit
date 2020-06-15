using UnityEngine;

namespace GKitForUnity {
	[RequireComponent(typeof(RectTransform))]
	public class UiTransform : MonoBehaviour {
		private HorizontalAlignment horizontalAlignment;
		public HorizontalAlignment HorizontalAlignment {
			get {
				return horizontalAlignment;
			}
			set {
				horizontalAlignment = value;
				RectTransform.SetHorizontalAlignment(value);
			}
		}
		private VerticalAlignment verticalAlignment;
		public VerticalAlignment VerticalAlignment {
			get {
				return verticalAlignment;
			}
			set {
				verticalAlignment = value;
				RectTransform.SetVerticalAlignment(value);
			}
		}
		public Vector2 Position {
			get {
				return RectTransform.anchoredPosition;
			}
			set {
				RectTransform.anchoredPosition = value;
			}
		}
		public Vector2 SizeDelta {
			get {
				return RectTransform.sizeDelta;
			}
			set {
				RectTransform.sizeDelta = value;
			}
		}
		public Vector2 Pivot {
			get {
				return RectTransform.pivot;
			}
			set {
				RectTransform.pivot = value;
			}
		}

		public RectTransform RectTransform {
			get; private set;
		}

		private void Awake() {
			RectTransform = GetComponent<RectTransform>();
		}
		public void SetHorizontalAlignment(HorizontalAlignment alignment, bool setPivotAuto = true) {
			HorizontalAlignment = alignment;

			if (setPivotAuto) {
				Pivot = new Vector2(UiUtility.GetPivotPosition(alignment), Pivot.y);
			}
		}
		public void SetVerticalAlignment(VerticalAlignment alignment, bool setPivotAuto = true) {
			VerticalAlignment = alignment;

			if (setPivotAuto) {
				Pivot = new Vector2(Pivot.x, UiUtility.GetPivotPosition(alignment));
			}
		}
	}
}
