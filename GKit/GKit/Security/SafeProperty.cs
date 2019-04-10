using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKit.Security {
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
				OnHackedInstance.SafeInvoke();

			}
			return value;
		}
		private void UpdateChecksum() {
			hashBuffer = GetChecksum();
		}
		private int GetChecksum() {
			if(value == null) {
				return 0;
			} else {
				return value.GetHashCode();
			}
		}

		public void RunEvent() {
			OnValueChanged.SafeInvoke();
		}
		public void ClearEvent() {
			OnValueChanged = null;
		}
	}
}
