using System;
using System.Collections.Generic;

namespace GKit.Core.Component {
	public class GLoopGroup {
		public int TaskCount => TaskList.Count;
		public GLoopEngine OwnerCore {
			get; private set;
		}
		public List<GLoopAction> TaskList {
			get; private set;
		}
		public GWhen RemoveCondition {
			get; private set;
		}

		public GLoopGroup(GLoopEngine ownerCore, GWhen removeCondition) {
			this.OwnerCore = ownerCore;
			TaskList = new List<GLoopAction>();
			RemoveCondition = removeCondition;
		}
		public void SyncFrame() {
			int loopCount = TaskList.Count;
			GLoopAction task;
			for (int i = 0; i < loopCount; i++) {
				task = TaskList[i];
				task.currentFrame = OwnerCore.currentFrame;
			}
		}
		public void RunWithTimer() {
			int loopCount = TaskList.Count;
			GLoopAction task;
			for (int i = 0; i < loopCount; i++) {
				task = TaskList[i];
				if (task.currentFrame == OwnerCore.currentFrame) {
					continue;
				}
				task.RunTimer();
			}
		}
		public void RunImmediately() {
			int loopCount = TaskList.Count;
			GLoopAction task;
			for (int i = 0; i < loopCount; i++) {
				task = TaskList[i];
				if (task.currentFrame == OwnerCore.currentFrame) {
					continue;
				}
				task.RunImmediately();
			}
		}
		public void Clear() {
			TaskList.Clear();
		}
	}
}
