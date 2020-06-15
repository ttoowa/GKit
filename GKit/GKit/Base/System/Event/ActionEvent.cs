using System;
using System.Collections.Generic;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public class ActionEvent {
		private Queue<Action> actionQueue;
		public List<Action> actionList;

		public ActionEvent() {
			actionQueue = new Queue<Action>();
			actionList = new List<Action>();
		}
		public void Invoke() {
			while (actionQueue.Count > 0) {
				actionQueue.Dequeue().Invoke();
			}
			for (int i = 0; i < actionList.Count; ++i) {
				actionList[i].Invoke();
			}
		}
		public void Add(Action action, bool executeOnce = false) {
			if (executeOnce) {
				actionQueue.Enqueue(action);
			} else {
				actionList.Add(action);
			}
		}
		public bool Remove(Action action) {
			return actionList.Remove(action);
		}

		public static ActionEvent operator +(ActionEvent left, Action right) {
			left.Add(right);
			return left;
		}
		public static ActionEvent operator -(ActionEvent left, Action right) {
			left.Remove(right);
			return left;
		}
	}
}
