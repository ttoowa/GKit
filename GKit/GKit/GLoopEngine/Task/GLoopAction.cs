using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKit {
	/// <summary>
	/// 코어에서 실행하는 작업 단위입니다.
	/// </summary>
	public class GLoopAction {
		public int Timer => timer;
		public Action Action => action;
		public int DelayTime => delayTime;
		public GWhen TaskEvent => taskEvent;

		private GLoopEngine ownerCore;
		private int timer;
		private Action action;
		private int delayTime;
		private GWhen taskEvent;
		internal int currentFrame;
		
		internal GLoopAction(GLoopEngine ownerCore, Action action, int delayTime, GWhen taskEvent) {
			this.ownerCore = ownerCore;
			this.action = action;
			this.timer = delayTime;
			this.delayTime = delayTime;
			this.taskEvent = taskEvent;
		}
		public void Stop() {
			ownerCore.RemoveTask(this);
		}
		internal void RunTimer() {
			if (UpdateTimer()) {
				action();
			}
		}
		internal void RunImmediately() {
			action();
		}
		private bool UpdateTimer() {
			if (delayTime <= 0)
				return false;
			if (++timer >= delayTime) {
				timer = 0;
				return true;
			}
			return false;
		}
	}
}
