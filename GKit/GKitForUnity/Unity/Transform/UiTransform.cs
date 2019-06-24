extern alias CoreModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GKit;
using RectTransform = CoreModule::UnityEngine.RectTransform;
using CVector2 = CoreModule::UnityEngine.Vector2;
using CRect = CoreModule::UnityEngine.Rect;
using Vector2 = global::UnityEngine.Vector2;
using Rect = global::UnityEngine.Rect;

namespace GKit.Unity {
	[RequireComponent(typeof(RectTransform))]
	public class UiTransform : MonoBehaviour {
		private HorizontalAlignment horizontalAlignment;
		public HorizontalAlignment HorizontalAlignment {
			get {
				return horizontalAlignment;
			} set {
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
		public CVector2 Position {
			get {
				return RectTransform.anchoredPosition;
			} set {
				RectTransform.anchoredPosition = value;
			}
		}
		public CVector2 SizeDelta {
			get {
				return RectTransform.sizeDelta;
			} set {
				RectTransform.sizeDelta = value;
			}
		}
		public CVector2 Pivot {
			get {
				return RectTransform.pivot;
			} set {
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
				Pivot =  new CVector2(UiUtility.GetPivotPosition(alignment), Pivot.y);
			}
		}
		public void SetVerticalAlignment(VerticalAlignment alignment, bool setPivotAuto = true) {
			VerticalAlignment = alignment;

			if (setPivotAuto) {
				Pivot = new CVector2(Pivot.x, UiUtility.GetPivotPosition(alignment));
			}
		}
	}
}
