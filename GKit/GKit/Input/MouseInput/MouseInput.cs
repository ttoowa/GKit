using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
#if OnUnity
using UnityEngine;
#else
using System.Windows.Media;
#endif

namespace GKit {
	/// <summary>
	/// 마우스 입력 정보를 제공하는 클래스입니다.
	/// </summary>
	public static class MouseInput {
		private enum MouseButton {
			Left = 0,
			Right = 1,
			Middle = 2,
		}

		public static InputButton Left {
			get; private set;
		}
		public static InputButton Right {
			get; private set;
		}
		public static InputButton Middle {
			get; private set;
		}

#if !OnUnity
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT {
			public int X;
			public int Y;

			public static implicit operator System.Drawing.Point(POINT point) {
				return new System.Drawing.Point(point.X, point.Y);
			}
		}
		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out POINT lpPoint);
#endif

#if OnUnity
		public static Vector2 ScreenPos {
			get; private set;
		}
		public static Vector2 ScrollDelta {
			get; private set;
		}
#else
		public static Vector2 AbsolutePosition {
			get; private set;
		}
#endif

		static MouseInput() {
			Left = new InputButton();
			Right = new InputButton();
			Middle = new InputButton();
		}
		internal static void Update() {
#if OnUnity
			ScreenPos = Input.mousePosition;
			ScrollDelta = Input.mouseScrollDelta;
#else
			POINT nativePos;
			GetCursorPos(out nativePos);

			AbsolutePosition = new Vector2(nativePos.X, nativePos.Y);
#endif

			bool current;
			//Left
#if OnUnity
			current = Input.GetMouseButton((int)MouseButton.Left);
#elif OnWPF
			current = Mouse.LeftButton == MouseButtonState.Pressed;
#else
			current = KeyInput.GetKeyHold(WinKey.MouseLeft);
#endif
			Left.UpdateState(current);


			//Right
#if OnUnity
			current = Input.GetMouseButton((int)MouseButton.Right);
#elif OnWPF
			current = Mouse.RightButton == MouseButtonState.Pressed;
#else
			current = KeyInput.GetKeyHold(WinKey.MouseRight);
#endif
			Right.UpdateState(current);

			//Middle
#if OnUnity
			current = Input.GetMouseButton((int)MouseButton.Middle);
#elif OnWPF
			current = Mouse.MiddleButton == MouseButtonState.Pressed;
#else
			current = KeyInput.GetKeyHold(WinKey.MouseMiddle);
#endif
			Middle.UpdateState(current);
		}
#if OnWPF
		public static Vector2 GetWindowPosition(Window window) {
			return (AbsolutePosition - (Vector2)window.PointToScreen(new Point()));
		}
		public static Vector2 GetRelativePosition(Visual visual) {
			return (AbsolutePosition - (Vector2)visual.PointToScreen(new Point()));
		}
#endif
#if OnUnity
		public static Vector2 GetWorldPos(Camera cam, float zDepth = 1f) {
			return cam.ScreenToWorldPoint(new Vector3(ScreenPos.x, ScreenPos.y, zDepth));
		}
#endif
	}
}