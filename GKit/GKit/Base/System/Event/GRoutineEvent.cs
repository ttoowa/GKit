using System;
using System.Collections;
using System.Collections.Generic;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public class GRoutineEvent {
		private GLoopEngine core;
		private Queue<Func<IEnumerator>> routineQueue;
		private List<Func<IEnumerator>> routineList;

		public GRoutineEvent(GLoopEngine core) {
			this.core = core;
			routineQueue = new Queue<Func<IEnumerator>>();
			routineList = new List<Func<IEnumerator>>();
		}
		public GRoutine Invoke() {
			return CallTask().Invoke(core);
		}
		public void Add(Func<IEnumerator> routine, bool executeOnce = false) {
			if (executeOnce) {
				routineQueue.Enqueue(routine);
			} else {
				routineList.Add(routine);
			}
		}
		public bool Remove(Func<IEnumerator> routine) {
			return routineList.Remove(routine);
		}

		private IEnumerator CallTask() {
			while (routineQueue.Count > 0) {
				yield return routineQueue.Dequeue()().Invoke(core);
			}
			for (int i = 0; i < routineList.Count; ++i) {
				Func<IEnumerator> routine = routineList[i];
				yield return routine().Invoke(core);
			}
		}

		public static GRoutineEvent operator +(GRoutineEvent left, Func<IEnumerator> right) {
			left.Add(right);
			return left;
		}
		public static GRoutineEvent operator -(GRoutineEvent left, Func<IEnumerator> right) {
			left.Remove(right);
			return left;
		}
	}
}
