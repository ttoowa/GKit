using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
#if UNITY
using UnityEngine;
#endif

namespace GKit {
	/// <summary>
	/// Unity API를 사용해 마우스 입력을 감지하는 클래스입니다.
	/// </summary>
	public static class MouseInput {
		private const int
			Left = 0,
			Right = 1,
			Middle = 2;
		
		public static bool LeftAuto {
			get; private set;
		}
		public static bool LeftDown {
			get; private set;
		}
		public static bool LeftUp {
			get; private set;
		}
		public static bool RightAuto {
			get; private set;
		}
		public static bool RightDown {
			get; private set;
		}
		public static bool RightUp {
			get; private set;
		}
		public static bool MiddleAuto {
			get; private set;
		}
		public static bool MiddleDown {
			get; private set;
		}
		public static bool MiddleUp {
			get; private set;
		}
		

		public static event Action
			OnLeftDownOnce,
			OnLeftUpOnce,
			OnRightDownOnce,
			OnRightUpOnce,
			OnMiddleDownOnce,
			OnMiddleUpOnce,

			OnLeftDown,
			OnLeftUp,
			OnRightDown,
			OnRightUp,
			OnMiddleDown,
			OnMiddleUp;

#if WPF
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT {
			public int X;
			public int Y;

			public static implicit operator Point(POINT point) {
				return new Point(point.X, point.Y);
			}
		}
		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out POINT lpPoint);
#endif

#if UNITY
		public static Vector2 ScreenPos {
			get; private set;
		}
		public static Vector2 ScrollDelta {
			get; private set;
		}
#elif WPF
		public static Vector2 AbsolutePosition {
			get; private set;
		}
#endif

		internal static void Update() {
#if UNITY
			ScreenPos = Input.mousePosition;
			ScrollDelta = Input.mouseScrollDelta;
#elif WPF
			POINT nativePos;
			GetCursorPos(out nativePos);

			AbsolutePosition = new Vector2(nativePos.X, nativePos.Y);
#endif

			bool current;
			//Left
#if UNITY
			current = Input.GetMouseButton(Left);
#elif WPF
			current = KeyInput.GetKey(WinKey.MouseLeft); //Mouse.LeftButton == MouseButtonState.Pressed;
#endif
			if (LeftUp) {
				LeftUp = false;
			}
			if(current) {
				if(!LeftAuto) {
					LeftDown = true;
					OnLeftDownOnce.SafeInvoke();
					OnLeftDownOnce = null;
					OnLeftDown.SafeInvoke();
				} else {
					LeftDown = false;
				}
			} else {
				if(LeftAuto) {
					LeftDown = false;
					LeftUp = true;

					OnLeftUpOnce.SafeInvoke();
					OnLeftUpOnce = null;
					OnLeftUp.SafeInvoke();
				}
			}
			LeftAuto = current;

			//Right
#if UNITY
			current = Input.GetMouseButton(Right);
#elif WPF
			current = KeyInput.GetKey(WinKey.MouseRight); //Mouse.RightButton == MouseButtonState.Pressed;
#endif
			if (RightUp) {
				RightUp = false;
			}
			if(current) {
				if(!RightAuto) {
					RightDown = true;

					OnRightDownOnce.SafeInvoke();
					OnRightDownOnce = null;
					OnRightDown.SafeInvoke();
				} else {
					RightDown = false;
				}
			} else {
				if(RightAuto) {
					RightDown = false;
					RightUp = true;

					OnRightUpOnce.SafeInvoke();
					OnRightUpOnce = null;
					OnRightUp.SafeInvoke();
				}
			}
			RightAuto = current;

			//Middle
#if UNITY
			current = Input.GetMouseButton(Middle);
#elif WPF
			current = KeyInput.GetKey(WinKey.MouseMiddle); //Mouse.MiddleButton == MouseButtonState.Pressed;
#endif
			if (MiddleUp) {
				MiddleUp = false;
			}
			if (current) {
				if (!MiddleAuto) {
					MiddleDown = true;

					OnMiddleDownOnce.SafeInvoke();
					OnMiddleDownOnce = null;
					OnMiddleDown.SafeInvoke();
				} else {
					MiddleDown = false;
				}
			} else {
				if (MiddleAuto) {
					MiddleDown = false;
					MiddleUp = true;

					OnMiddleUpOnce.SafeInvoke();
					OnMiddleUpOnce = null;
					OnMiddleUp.SafeInvoke();
				}
			}
			MiddleAuto = current;
		}
#if WPF
		//private static Vector2 GetAbsolutePosition() {
		//	var pos = System.Windows.Forms.Cursor.Position;
		//	return new Vector2(pos.X, pos.Y);
		//}
		public static Vector2 GetWindowPosition(Window window) {
			return (AbsolutePosition - (Vector2)window.PointToScreen(new Point()));
		}
		public static Vector2 GetRelativePosition(Visual visual) {
			return (AbsolutePosition - (Vector2)visual.PointToScreen(new Point()));
		}
#endif
#if UNITY
		public static Vector2 GetWorldPos(Camera cam, float zDepth = 1f) {
			return cam.ScreenToWorldPoint(new Vector3(ScreenPos.x, ScreenPos.y, zDepth));
		}
#endif
	}
}