using System;
using UnityEngine;

namespace GKitForUnity {
	/// <summary>
	/// 유니티 오브젝트에 추가되는 입력 이벤트 클래스입니다.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class InputHandler : MonoBehaviour {
		private const float DragThreshold = 20f;

		public bool IsFocused {
			get; private set;
		}
		public bool IsMouseOver {
			get; private set;
		}
		public bool IsMousePreviewOver {
			get; private set;
		}
		public bool IsMousePressed {
			get; private set;
		}
		public bool IsMouseRightPressed {
			get; private set;
		}
		public bool IsMouseMiddlePressed {
			get; private set;
		}

		public GLoopEngine OwnerLoopEngine {
			get; private set;
		}
		public int readMask;
		public int writeMask;
		public Vector2 Size {
			get {
				Collider collider = this.Collider;
				if (collider is BoxCollider) {
					return ((BoxCollider)collider).size;
				}
				return collider.bounds.size;
			}
		}
		public Vector2 ActualSize {
			get {
				Vector2 size = Size;
				Vector2 scale = (Vector2)transform.lossyScale;
				return new Vector2(size.x * scale.x, size.y * scale.y);
			}
		}
		public int Layer {
			get {
				return gameObject.layer;
			}
			set {
				gameObject.layer = value;
				OnLayerChanged?.Invoke(value);
			}
		}
		public CursorInfo Cursor {
			get {
				return cursor;
			}
			set {
				cursor = value;
				if (IsMouseOver) {
					CursorManager.SetCursor(value);
				}
			}
		}
		public event Action
			//Mouse
			OnMouseEnter,
			OnMouseExit,
			OnMousePreviewEnter,
			OnMousePreviewExit,
			OnMouseDown,
			OnMouseRightDown,
			OnMouseMiddleDown,
			OnAnyMouseDown,
			OnMouseDragStart,
			OnMouseUp,
			OnMouseRightUp,
			OnMouseMiddleUp,
			OnClick,
			OnRightClick,
			OnMiddleClick,
			//Focus
			OnFocusOn,
			OnFocusOut;
		public event Arg1Delegate<int> OnLayerChanged;
		public event Arg1Delegate<Vector2> OnScrolled;

		public Collider Collider {
			get {
				FindCollider();
				return collider;
			}
		}
		private new Collider collider;
		private CursorInfo cursor;

		private Vector2 mouseDownPos;
		private bool calledDragStart;

		public void SetOwnerLoopEngine(GLoopEngine loopEngine) {
			this.OwnerLoopEngine = loopEngine;
		}
		private void FindCollider() {
			if (collider != null)
				return;

			collider = GetComponent<Collider>();
		}
		//Mouse
		internal void CallMouseEnter() {
			IsMouseOver = true;
			if (cursor != null) {
				CursorManager.SetCursor(cursor);
			}
			OnMouseEnter?.Invoke();
		}
		internal void CallMouseExit() {
			IsMouseOver = false;
			if (cursor != null) {
				CursorManager.SetCursor(null);
			}
			OnMouseExit?.Invoke();
		}
		internal void CallMousePreviewEnter() {
			IsMousePreviewOver = true;
			OnMousePreviewEnter?.Invoke();
		}
		internal void CallMousePreviewExit() {
			IsMousePreviewOver = false;
			OnMousePreviewExit?.Invoke();
		}
		internal void CallMouseDown() {
			IsMousePressed = true;
			OnMouseDown?.Invoke();
			OnAnyMouseDown?.Invoke();

			if (OwnerLoopEngine != null) {
				calledDragStart = false;
				mouseDownPos = MouseInput.ScreenPos;
				OwnerLoopEngine.AddLoopAction(OnMouseDragging, GLoopCycle.EveryFrame, GWhen.MouseUpRemove);
			}
		}
		internal void CallMouseRightDown() {
			IsMouseRightPressed = true;
			OnMouseRightDown?.Invoke();
			OnAnyMouseDown?.Invoke();
		}
		internal void CallMouseMiddleDown() {
			IsMouseMiddlePressed = true;
			OnMouseMiddleDown?.Invoke();
			OnAnyMouseDown?.Invoke();
		}
		internal void CallMouseDragStart() {
			OnMouseDragStart?.Invoke();
		}
		internal void CallMouseUp() {
			IsMousePressed = false;
			OnMouseUp?.Invoke();
		}
		internal void CallMouseRightUp() {
			IsMouseRightPressed = false;
			OnMouseRightUp?.Invoke();
		}
		internal void CallMouseMiddleUp() {
			IsMouseMiddlePressed = false;
			OnMouseMiddleUp?.Invoke();
		}
		internal void CallClick() {
			OnClick?.Invoke();
		}
		internal void CallRightClick() {
			OnRightClick?.Invoke();
		}
		internal void CallMiddleClick() {
			OnMiddleClick?.Invoke();
		}
		//Focus
		internal void CallFocusOn() {
			IsFocused = true;
			OnFocusOn?.Invoke();
		}
		internal void CallFocusOut() {
			IsFocused = false;
			OnFocusOut?.Invoke();
		}
		//Scroll
		internal void CallScrolled(Vector2 scrollDelta) {
			OnScrolled?.Invoke(scrollDelta);
		}

		private void OnMouseDragging() {
			if (calledDragStart)
				return;

			float dragLength = (MouseInput.ScreenPos - mouseDownPos).magnitude;
			if (dragLength >= DragThreshold) {
				calledDragStart = true;

				CallMouseDragStart();
			}
		}
		public void SetEnable(bool enable) {
			Collider.enabled = enable;
		}

		[Obsolete("enabled 대신 SetEnable(bool)을 사용하세요.")]
		public new bool enabled;
	}
}