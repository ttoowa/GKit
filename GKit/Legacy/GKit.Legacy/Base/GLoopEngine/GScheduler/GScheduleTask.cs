using System;
using System.Collections;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Core.Scheduler {
	public class GScheduleTask {
		public GScheduleTaskType type;
		public object action;

		public GScheduleTask(Action action) {
			this.action = action;
			type = GScheduleTaskType.CoreTask;
		}
		public GScheduleTask(IEnumerator routine) {
			this.action = routine;
			type = GScheduleTaskType.CoreRoutine;
		}
	}
}
