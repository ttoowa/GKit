using System;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
    /// <summary>
    ///     코어에서 실행하는 작업 단위입니다.
    /// </summary>
    public class GLoopAction {
        public int Timer { get; private set; }

        public Action Action { get; }

        public int DelayTime { get; }

        public GWhen TaskEvent { get; }

        private readonly GLoopEngine ownerCore;
        internal int currentFrame;

        internal GLoopAction(GLoopEngine ownerCore, Action action, int delayTime, GWhen taskEvent) {
            this.ownerCore = ownerCore;
            Action = action;
            Timer = delayTime;
            DelayTime = delayTime;
            TaskEvent = taskEvent;
        }

        public void Stop() {
            ownerCore.RemoveLoopAction(this);
        }

        internal void RunTimer() {
            if (UpdateTimer()) {
                Action();
            }
        }

        internal void RunImmediately() {
            Action();
        }

        private bool UpdateTimer() {
            if (DelayTime <= 0) {
                return false;
            }

            if (++Timer >= DelayTime) {
                Timer = 0;
                return true;
            }

            return false;
        }
    }
}