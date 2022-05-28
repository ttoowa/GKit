using System;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public class InputButton {
		public bool IsDown {
			get; internal set;
		}
		public bool IsHold {
			get; internal set;
		}
		public bool IsUp {
			get; internal set;
		}

		public event Action Down;
		public event Action DownOnce;

		public event Action Up;
		public event Action UpOnce;

		public event Action Hold;
		public event Action HoldOnce;

		internal InputButton() {

		}
		internal void ResetState() {
			IsDown = false;
			IsHold = false;
			IsUp = false;
		}
		internal void UpdateState(bool onHold) {
			IsDown = false;
			IsUp = false;

			if (IsHold) {
				if (!onHold) {
					IsUp = true;

					UpOnce?.Invoke();
					UpOnce = null;
					Up?.Invoke();
				}
			} else {
				if (onHold) {
					IsDown = true;
					DownOnce?.Invoke();
					DownOnce = null;
					Down?.Invoke();
				}
			}

			IsHold = onHold;
			if(onHold) {
				HoldOnce?.Invoke();
				HoldOnce = null;
				Hold?.Invoke();
			}
		}
	}
}
