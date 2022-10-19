using System;
using System.Diagnostics;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
    public class InputButton {
        public bool IsDown { get; internal set; }

        public bool IsHold { get; internal set; }

        public bool IsUp { get; internal set; }

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

                    Action currentEvent = UpOnce;
                    UpOnce = null;
                    currentEvent?.Invoke();
                    Up?.Invoke();
                }
            } else {
                if (onHold) {
                    IsDown = true;

                    Action currentEvent = DownOnce;
                    DownOnce = null;
                    currentEvent?.Invoke();
                    Down?.Invoke();
                }
            }

            IsHold = onHold;
            if (onHold) {
                Action currentEvent = HoldOnce;
                HoldOnce = null;
                currentEvent?.Invoke();
                Hold?.Invoke();
            }
        }
    }
}