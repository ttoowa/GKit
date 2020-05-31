using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKit {
	public class InputButton {
		public bool Down {
			get; internal set;
		}
		public bool Hold {
			get; internal set;
		}
		public bool Up {
			get; internal set;
		}

		public event Action OnDown;
		public event Action OnUp;

		public event Action OnDownOnce;
		public event Action OnUpOnce;

		internal InputButton() {

		}
		internal void ResetState() {
			Down = false;
			Hold = false;
			Up = false;
		}
		internal void UpdateState(bool onHold) {
			Down = false;
			Up = false;

			if(Hold) {
				if(!onHold) {
					Up = true;

					OnUpOnce?.Invoke();
					OnUpOnce = null;
					OnUp?.Invoke();
				}
			} else {
				if(onHold) {
					Down = true;
					OnDownOnce?.Invoke();
					OnDownOnce = null;
					OnDown?.Invoke();
				}
			}
			
			Hold = onHold;
		}
	}
}
