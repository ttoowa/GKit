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
		private const int
			Left = 0,
			Right = 1,
			Middle = 2;
		
		public static bool LeftHold {
			get; private set;
		}
		public static bool LeftDown {
			get; private set;
		}
		public static bool LeftUp {
			get; private set;
		}
		public static bool RightHold {
			get; private set;
		}
		public static bool RightDown {
			get; private set;
		}
		public static bool RightUp {
			get; private set;
		}
		public static bool MiddleHold {
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
			current = Input.GetMouseButton(Left);
#else
			current = KeyInput.GetKeyHold(WinKey.MouseLeft); //Mouse.LeftButton == MouseButtonState.Pressed;
#endif
			if (LeftUp) {
				LeftUp = false;
			}
			if(current) {
				if(!LeftHold) {
					LeftDown = true;
					OnLeftDownOnce.SafeInvoke();
					OnLeftDownOnce = null;
					OnLeftDown.SafeInvoke();
				} else {
					LeftDown = false;
				}
			} else {
				if(LeftHold) {
					LeftDown = false;
					LeftUp = true;

					OnLeftUpOnce.SafeInvoke();
					OnLeftUpOnce = null;
					OnLeftUp.SafeInvoke();
				}
			}
			LeftHold = current;

			//Right
#if OnUnity
			current = Input.GetMouseButton(Right);
#else
			current = KeyInput.GetKeyHold(WinKey.MouseRight); //Mouse.RightButton == MouseButtonState.Pressed;
#endif
			if (RightUp) {
				RightUp = false;
			}
			if(current) {
				if(!RightHold) {
					RightDown = true;

					OnRightDownOnce.SafeInvoke();
					OnRightDownOnce = null;
					OnRightDown.SafeInvoke();
				} else {
					RightDown = false;
				}
			} else {
				if(RightHold) {
					RightDown = false;
					RightUp = true;

					OnRightUpOnce.SafeInvoke();
					OnRightUpOnce = null;
					OnRightUp.SafeInvoke();
				}
			}
			RightHold = current;

			//Middle
#if OnUnity
			current = Input.GetMouseButton(Middle);
#else
			current = KeyInput.GetKeyHold(WinKey.MouseMiddle); //Mouse.MiddleButton == MouseButtonState.Pressed;
#endif
			if (MiddleUp) {
				MiddleUp = false;
			}
			if (current) {
				if (!MiddleHold) {
					MiddleDown = true;

					OnMiddleDownOnce.SafeInvoke();
					OnMiddleDownOnce = null;
					OnMiddleDown.SafeInvoke();
				} else {
					MiddleDown = false;
				}
			} else {
				if (MiddleHold) {
					MiddleDown = false;
					MiddleUp = true;

					OnMiddleUpOnce.SafeInvoke();
					OnMiddleUpOnce = null;
					OnMiddleUp.SafeInvoke();
				}
			}
			MiddleHold = current;
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