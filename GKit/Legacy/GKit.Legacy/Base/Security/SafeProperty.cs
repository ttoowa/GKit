﻿using System;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Security {
	public class SafeProperty<T> where T : IConvertible {
		public event Action OnHackedInstance;
		public T Value {
			get {
				return GetValue();
			}
			set {
				SetValue(value);
			}
		}
		public event Action OnValueChanged;
		private T value;
		private int hashBuffer; //CheckSum

		public SafeProperty() {
			UpdateChecksum();
		}
		public SafeProperty(T value) {
			SetValue(value);
		}
		public void SetValue(T value) {
			SetValueNoEvent(value);
			OnValueChanged?.Invoke();
		}
		public void SetValueNoEvent(T value) {
			this.value = value;
			UpdateChecksum();
		}
		private T GetValue() {
			if (hashBuffer != GetChecksum()) {
				SecurityEvent.CallMemoryHacked();
				OnHackedInstance.TryInvoke();

			}
			return value;
		}
		private void UpdateChecksum() {
			hashBuffer = GetChecksum();
		}
		private int GetChecksum() {
			if (value == null) {
				return 0;
			} else {
				return value.GetHashCode();
			}
		}

		public void RunEvent() {
			OnValueChanged.TryInvoke();
		}
		public void ClearEvent() {
			OnValueChanged = null;
		}
	}
}
