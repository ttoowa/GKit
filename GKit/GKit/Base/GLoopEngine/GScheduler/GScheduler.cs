using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if OnUnity
using UnityEngine;
using GKitForUnity.Core.Scheduler;
#elif OnWPF
using GKitForWPF.Core.Scheduler;
#else
using GKit.Core.Scheduler;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public class GScheduler {
		public GLoopEngine ownerCore;
		public bool IsRunning => routine != null;
		public bool IsExecuting => JobCount > 0 || executionFlag;
		public int JobCount => jobQueue.Count;
		private bool executionFlag;

		private GRoutine routine;
		private Queue<GScheduleTask> jobQueue;


		public GScheduler(GLoopEngine ownerCore) {
			this.ownerCore = ownerCore;

			jobQueue = new Queue<GScheduleTask>();
		}
		public void Start() {
			if (IsRunning)
				return;

			routine = ownerCore.AddGRoutine(ScheduleUpdate());
		}
		public void Stop() {
			if (!IsRunning)
				return;

			ownerCore.RemoveGRoutine(routine);
			routine = null;
		}

		public void AddTask(Action action) {
			jobQueue.Enqueue(new GScheduleTask(action));
		}
		public void AddRoutine(IEnumerator routine) {
			jobQueue.Enqueue(new GScheduleTask(routine));
		}
		public void AddWaitSeconds(float seconds) {
			AddRoutine(WaitSeconds(seconds));
		}

		private IEnumerator ScheduleUpdate() {
			for (; ; ) {
				while (jobQueue.Count > 0) {
					executionFlag = true;
					GScheduleTask task = jobQueue.Dequeue();
					switch (task.type) {
						case GScheduleTaskType.CoreTask:
							((Action)task.action).TryInvoke();
							break;
						case GScheduleTaskType.CoreRoutine:
							yield return ((IEnumerator)task.action).Invoke(ownerCore);
							break;
					}
				}
				yield return null;
				executionFlag = false;
			}
		}
		public static IEnumerator WaitSeconds(float seconds) {
			yield return new GWait(GTimeUnit.Second, seconds);
		}
		public IEnumerator WaitJob() {
			for (; ; ) {
				if (IsExecuting) {
					yield return null;
				} else {
					yield break;
				}
			}
		}
	}
}
