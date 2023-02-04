using UnityEngine;

namespace GKitForUnity;

/// <summary>
///     입력 레이어 클래스입니다. 유니티 레이어를 참조합니다.
/// </summary>
public class InputLayer {
    public Camera camera;
    public int layer;

    public bool OnHit { get; private set; }

    private InputHandler cursorFocus;
    private InputHandler previewCursorFocus;
    private InputHandler keyFocus;

    public InputLayer(Camera camera, int layer) {
        this.camera = camera;
        this.layer = layer;
    }

    internal void StartUpdate() {
        OnHit = false;
    }

    internal void EndUpdate() {
        if (!OnHit) {
            Update(null);
        }
    }

    internal void Update(InputHandler handler) {
        if (handler != null) {
            OnHit = true;

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
                if (!MouseInput.Left.IsHold) {
                    CursorFocusOut();

                    //새 포커스
                    cursorFocus = handler;
                    cursorFocus.CallMouseEnter();
                }
            }

            if (keyFocus != handler) {
                if (MouseInput.Left.IsDown) {
                    KeyFocusOut();
                    handler.CallFocusOn();
                    keyFocus = handler;
                }
            }

            if (cursorFocus != null) {
                //마우스다운
                if (MouseInput.Left.IsDown) {
                    cursorFocus.CallMouseDown();
                }

                if (MouseInput.Right.IsDown) {
                    cursorFocus.CallMouseRightDown();
                }

                if (MouseInput.Middle.IsDown) {
                    cursorFocus.CallMouseMiddleDown();
                }

                //마우스업
                if (MouseInput.Left.IsUp) {
                    if (cursorFocus.IsMousePressed) {
                        cursorFocus.CallMouseUp();
                        cursorFocus.CallClick();
                    }
                }

                if (MouseInput.Right.IsUp) {
                    if (cursorFocus.IsMouseRightPressed) {
                        cursorFocus.CallMouseRightUp();
                        cursorFocus.CallRightClick();
                    }
                }

                if (MouseInput.Middle.IsUp) {
                    if (cursorFocus.IsMouseMiddlePressed) {
                        cursorFocus.CallMouseMiddleUp();
                        cursorFocus.CallMiddleClick();
                    }
                }
            }
        } else {
            PreviewCursorFocusOut();
            if (MouseInput.Left.IsDown || MouseInput.Right.IsDown || MouseInput.Middle.IsDown) {
                KeyFocusOut();
            } else if (!MouseInput.Left.IsHold && !MouseInput.Right.IsHold && !MouseInput.Middle.IsHold) {
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

            if (cursorFocus.IsMouseRightPressed) {
                cursorFocus.CallMouseRightUp();
            }

            if (cursorFocus.IsMouseMiddlePressed) {
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