using System.Collections.Generic;
using System.Runtime.InteropServices;
#if OnUnity
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
#endif

namespace GKit {

	//TODO : 키 검사를 안 하다가 최초로 검사할 시 누른 상태일 확률 큼 -> 해결하기
	//첫 번째 검사는 무조건 false를 반환하고, 한 번이라도 검사한 키는 계속 검사해주는 건 어떨까?

	/// <summary>
	/// Windows API를 사용해 키보드 입력을 감지하는 클래스입니다.
	/// </summary>
	public static class KeyInput {
		/// <summary>
		/// 텍스트 키를 인식할 타이밍 (60fps 기준 : 20)
		/// </summary>
		private const float TextKeyDelayMillisec = 266f;
		private const float TextKeyFireDelayMillisec = 20f;

		private static GLoopEngine ownerCore;

		private static float textKeyTimer;
		private static WinKey currentTextKey;
		private static Stack<WinKey> keyDownStack = new Stack<WinKey>();
		private static List<WinKey> keyHoldList = new List<WinKey>();
		private static Stack<WinKey> keyUpStack = new Stack<WinKey>();
		private static List<WinKey> activeKeyList = new List<WinKey>();

		public delegate bool ConditionDelegate();
		public static event ConditionDelegate Condition;

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
		private static extern short GetAsyncKeyState(int keyCode);

		static KeyInput() {
#if OnUnity
			Condition += () => { return Application.isFocused; };
#endif
		}

		internal static void SetCore(GLoopEngine core) {
			ownerCore = core;

			core.AddLoopAction(UpdateFrame);
		}

		public static bool GetKeyHold(WinKey key) {
			RegistActiveKey(key);
			if (!CheckCondition())
				return false;

			return keyHoldList.Contains(key);
		}
		public static bool GetKeyDown(WinKey key) {
			RegistActiveKey(key);
			if (!CheckCondition())
				return false;

			return keyDownStack.Contains(key);
		}
		public static bool GetKeyText(WinKey key) {
			RegistActiveKey(key);
			if (!CheckCondition())
				return false;

			if (keyDownStack.Contains(key)) {
				currentTextKey = key;
				textKeyTimer = TextKeyDelayMillisec;
				return true;
			} else {
				if (key == currentTextKey && keyHoldList.Contains(key)) {
					if (textKeyTimer <= 0) {
						textKeyTimer = TextKeyFireDelayMillisec;
						return true;
					} else {
						textKeyTimer -= ownerCore.DeltaMilliseconds;
						return false;
					}
				}
			}
			return false;
		}
		public static bool GetKeyUp(WinKey key) {
			RegistActiveKey(key);
			if (!CheckCondition())
				return false;

			if (keyUpStack.Contains(key)) {
				return true;
			} else if (!keyHoldList.Contains(key)) {
				if (GetAsyncKeyState(GetVKCode(key)) != 0) {
					keyHoldList.Add(key);
					keyDownStack.Push(key);
				}
			}
			return false;
		}

		public static Vector2 GetInputWASD() {
			Vector2 inputVector = new Vector2();
			if (GetKeyHold(WinKey.W)) {
				inputVector.y += 1f;
			}
			if (GetKeyHold(WinKey.S)) {
				inputVector.y -= 1f;
			}
			if (GetKeyHold(WinKey.A)) {
				inputVector.x -= 1f;
			}
			if (GetKeyHold(WinKey.D)) {
				inputVector.x += 1f;
			}
			return inputVector;
		}
		public static Vector2 GetInputArrow() {
			Vector2 inputVector = new Vector2();
			if (GetKeyHold(WinKey.UpArrow)) {
				inputVector.y += 1f;
			}
			if (GetKeyHold(WinKey.DownArrow)) {
				inputVector.y -= 1f;
			}
			if (GetKeyHold(WinKey.LeftArrow)) {
				inputVector.x -= 1f;
			}
			if (GetKeyHold(WinKey.RightArrow)) {
				inputVector.x += 1f;
			}
			return inputVector;
		}

		private static void RegistActiveKey(WinKey key) {
			if (!activeKeyList.Contains(key)) {
				activeKeyList.Add(key);
			}
		}
		private static void UpdateFrame() {
			keyUpStack.Clear();
			keyDownStack.Clear();

			for (int i = activeKeyList.Count - 1; i >= 0; i--) {
				WinKey key = activeKeyList[i];
				if (GetAsyncKeyStateAutoWinKey(key)) {
					//키를 누른 경우
					if (!keyHoldList.Contains(key)) {
						if (!keyDownStack.Contains(key)) {
							keyDownStack.Push(key);
						}
						keyHoldList.Add(key);
					}
				} else {
					//키를 누르지 않은 경우
					if (keyHoldList.Contains(key)) {
						if (!keyUpStack.Contains(key)) {
							keyUpStack.Push(key);
						}
						keyHoldList.Remove(key);
					}
				}
			}
		}
		//Utility
		private static int GetVKCode(WinKey key) {
			switch (key) {
				case WinKey.MouseLeft:
					return 0x01;
				case WinKey.MouseRight:
					return 0x02;
				case WinKey.MouseMiddle:
					return 0x04;
				case WinKey.MouseX1:
					return 0x05;
				case WinKey.MouseX2:
					return 0x06;
				case WinKey.MouseUndefined:
					return 0x07;
				case WinKey.A:
				case WinKey.B:
				case WinKey.C:
				case WinKey.D:
				case WinKey.E:
				case WinKey.F:
				case WinKey.G:
				case WinKey.H:
				case WinKey.I:
				case WinKey.J:
				case WinKey.K:
				case WinKey.L:
				case WinKey.M:
				case WinKey.N:
				case WinKey.O:
				case WinKey.P:
				case WinKey.Q:
				case WinKey.R:
				case WinKey.S:
				case WinKey.T:
				case WinKey.U:
				case WinKey.V:
				case WinKey.W:
				case WinKey.X:
				case WinKey.Y:
				case WinKey.Z:
					return 0x41 + ((int)key - (int)WinKey.A);
				case WinKey.Backspace:
					return 0x08;
				case WinKey.Tab:
					return 0x09;
				case WinKey.Clear:
					return 0x0C;
				case WinKey.Return:
					return 0x0D;
				case WinKey.Pause:
					return 0x13;
				case WinKey.Escape:
					return 0x1B;
				case WinKey.Space:
					return 0x20;
				case WinKey.Exclaim:
					return 0x31;
				case WinKey.DoubleQuote:
					return 0xDE;
				case WinKey.Hash:
					return 0x33;
				case WinKey.Dollar:
					return 0x34;
				case WinKey.Ampersand:
					return 0x37;
				case WinKey.Quote:
					return 0xDE;
				case WinKey.LeftParen:
					return 0x39;
				case WinKey.RightParen:
					return 0x30;
				case WinKey.Asterisk:
					return 0x13;
				case WinKey.Equals:
				case WinKey.Plus:
					return 0xBB;
				case WinKey.Less:
				case WinKey.Comma:
					return 0xBC;
				case WinKey.Underscore:
				case WinKey.Minus:
					return 0xBD;
				case WinKey.Greater:
				case WinKey.Period:
					return 0xBE;
				case WinKey.Question:
				case WinKey.Slash:
					return 0xBF;
				case WinKey.Alpha0:
				case WinKey.Alpha1:
				case WinKey.Alpha2:
				case WinKey.Alpha3:
				case WinKey.Alpha4:
				case WinKey.Alpha5:
				case WinKey.Alpha6:
				case WinKey.Alpha7:
				case WinKey.Alpha8:
				case WinKey.Alpha9:
					return 0x30 + ((int)key - (int)WinKey.Alpha0);
				case WinKey.Colon:
				case WinKey.Semicolon:
					return 0xBA;
				case WinKey.At:
					return 0x32;
				case WinKey.LeftBracket:
					return 0xDB;
				case WinKey.Backslash:
					return 0xDC;
				case WinKey.RightBracket:
					return 0xDD;
				case WinKey.Caret:
					return 0x36;
				case WinKey.BackQuote:
					return 0xC0;
				case WinKey.Delete:
					return 0x2E;
				case WinKey.Keypad0:
				case WinKey.Keypad1:
				case WinKey.Keypad2:
				case WinKey.Keypad3:
				case WinKey.Keypad4:
				case WinKey.Keypad5:
				case WinKey.Keypad6:
				case WinKey.Keypad7:
				case WinKey.Keypad8:
				case WinKey.Keypad9:
					return 0x60 + ((int)key - (int)WinKey.Keypad0);
				case WinKey.KeypadPeriod:
					return 0x6E;
				case WinKey.KeypadDivide:
					return 0x6F;
				case WinKey.KeypadMultiply:
					return 0x6A;
				case WinKey.KeypadMinus:
					return 0x6D;
				case WinKey.KeypadPlus:
					return 0x6B;
				case WinKey.KeypadEnter:
					return 0x6C;
				case WinKey.UpArrow:
					return 0x26;
				case WinKey.DownArrow:
					return 0x28;
				case WinKey.RightArrow:
					return 0x27;
				case WinKey.LeftArrow:
					return 0x25;
				case WinKey.Insert:
					return 0x2D;
				case WinKey.Home:
					return 0x24;
				case WinKey.End:
					return 0x23;
				case WinKey.PageUp:
					return 0x21;
				case WinKey.PageDown:
					return 0x22;

				case WinKey.F1:
				case WinKey.F2:
				case WinKey.F3:
				case WinKey.F4:
				case WinKey.F5:
				case WinKey.F6:
				case WinKey.F7:
				case WinKey.F8:
				case WinKey.F9:
				case WinKey.F10:
				case WinKey.F11:
				case WinKey.F12:
				case WinKey.F13:
				case WinKey.F14:
				case WinKey.F15:
					return 0x70 + ((int)key - (int)WinKey.F1);

				case WinKey.Numlock:
					return 0x90;
				case WinKey.CapsLock:
					return 0x14;
				case WinKey.ScrollLock:
					return 0x91;
				case WinKey.RightShift:
					return 0xA1;
				case WinKey.LeftShift:
					return 0xA0;
				case WinKey.RightControl:
					return 0xA3;
				case WinKey.LeftControl:
					return 0xA2;
				case WinKey.RightAlt:
					return 0xA5;
				case WinKey.LeftAlt:
					return 0xA4;
				case WinKey.Help:
					return 0xE3;
				case WinKey.Print:
					return 0x2A;
				case WinKey.SysReq:
					return 0x2C;
				case WinKey.Break:
					return 0x03;
				case WinKey.Kor_Eng:
					return 0x21;
				case WinKey.LeftWindows:
				case WinKey.LeftApple:
					return 0x5b;
			}
			return 0;
		}
		private static bool GetAsyncKeyStateAutoWinKey(WinKey key) {
			return GetAsyncKeyStateAuto(GetVKCode(key));
		}
		private static bool GetAsyncKeyStateAuto(int VKey) {
			return (GetAsyncKeyState(VKey) != 0);
		}
		private static bool CheckCondition() {
			return Condition == null || Condition();
		}
	}
}