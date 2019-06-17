using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKit {
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

		public static ActionEvent operator + (ActionEvent left, Action right) {
			left.Add(right);
			return left;
		}
		public static ActionEvent operator -(ActionEvent left, Action right) {
			left.Remove(right);
			return left;
		}
	}
}
