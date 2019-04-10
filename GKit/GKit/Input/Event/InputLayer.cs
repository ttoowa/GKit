#if UNITY
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GKit {
	/// <summary>
	/// 입력 레이어 클래스입니다. 유니티 레이어를 참조합니다.
	/// </summary>
	public class InputLayer {
		public Camera camera;
		public int layer;

		public bool OnHit {
			get {
				return onHit;
			}
		}

		private bool onHit;
		private InputHandler cursorFocus;
		private InputHandler previewCursorFocus;
		private InputHandler keyFocus;

		public InputLayer(Camera camera, int layer) {
			this.camera = camera;
			this.layer = layer;
		}

		internal void StartUpdate() {
			onHit = false;
		}
		internal void EndUpdate() {
			if (!onHit) {
				Update(null);
			}
		}
		internal void Update(InputHandler handler) {
			if (handler != null) {
				onHit = true;

				if (previewCursorFocus != null) {
					if (MouseInput.ScrollDelta != Vector2.zero) {
						previewCursorFocus.CallScrolled(MouseInput.ScrollDelta);
					}
				}
				if (previewCursorFocus != handler) {
					PreviewCursorFocusOut();
					(previewCursorFocus = handler).CallMousePreviewEnter();
				}

				if (cursorFocus != handler) {
					if (!MouseInput.LeftAuto) {
						CursorFocusOut();

						//새 포커스
						cursorFocus = handler;
						cursorFocus.CallMouseEnter();
					}
				}
				if (keyFocus != handler) {
					if (MouseInput.LeftDown) {
						KeyFocusOut();
						handler.CallFocusOn();
						keyFocus = handler;
					}
				}
				if (cursorFocus != null) {
					//마우스다운
					if (MouseInput.LeftDown) {
						cursorFocus.CallMouseDown();
					}
					if (MouseInput.RightDown) {
						cursorFocus.CallMouseRightDown();
					}
					if(MouseInput.MiddleDown) {
						cursorFocus.CallMouseMiddleDown();
					}
					//마우스업
					if (MouseInput.LeftUp) {
						if (cursorFocus.IsMousePressed) {
							cursorFocus.CallMouseUp();
							cursorFocus.CallClick();
						}
					}
					if(MouseInput.RightUp) {
						if(cursorFocus.IsMouseRightPressed) {
							cursorFocus.CallMouseRightUp();
							cursorFocus.CallRightClick();
						}
					}
					if(MouseInput.MiddleUp) {
						if(cursorFocus.IsMouseMiddlePressed) {
							cursorFocus.CallMouseMiddleUp();
							cursorFocus.CallMiddleClick();
						}
					}
				}
			} else {
				PreviewCursorFocusOut();
				if (MouseInput.LeftDown || MouseInput.RightDown || MouseInput.MiddleDown) {
					KeyFocusOut();
				} else if (!MouseInput.LeftAuto && !MouseInput.RightAuto && !MouseInput.MiddleAuto) {
					CursorFocusOut();
				}
			}
		}

		private void KeyFocusOut() {
			if (keyFocus != null) {
				if (keyFocus.IsFocused) {
					keyFocus.CallFocusOut();
					keyFocus = null;
				}
			}
		}
		private void CursorFocusOut() {
			if (cursorFocus != null) {
				if (cursorFocus.IsMousePressed) {
					cursorFocus.CallMouseUp();
				}
				if(cursorFocus.IsMouseRightPressed) {
					cursorFocus.CallMouseRightUp();
				}
				if(cursorFocus.IsMouseMiddlePressed) {
					cursorFocus.CallMouseMiddleUp();
				}
				if (cursorFocus.IsMouseOver) {
					cursorFocus.CallMouseExit();
				}
				cursorFocus = null;
			}
		}
		private void PreviewCursorFocusOut() {
			if (previewCursorFocus != null) {
				if (previewCursorFocus.IsMousePreviewOver) {
					previewCursorFocus.CallMousePreviewExit();
				}
				previewCursorFocus = null;
			}
		}
	}
}
#endif