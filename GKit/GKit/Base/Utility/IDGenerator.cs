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
	/// <summary>
	/// 중복되지 않는 ID를 생성하는 클래스입니다.
	/// </summary>
	public class IDGenerator {
		public int StartIndex {
			get; private set;
		}
		public int Capacity {
			get; private set;
		}
		private Queue<int> IDQueue;

		public IDGenerator(int startIndex = 0) {
			this.StartIndex = startIndex;
			IDQueue = new Queue<int>();
			Expand(16);
		}
		public int GetID() {
			if (IDQueue.Count == 0) {
				Expand(1);
			}
			return IDQueue.Dequeue();
		}
		public int GetID(int count) {
			Expand(count);
			int ID = -1;
			for (int i = 0; i < count; ++i) {
				ID = IDQueue.Dequeue();
			}
			return ID;
		}
		public void ReturnID(int ID) {
			IDQueue.Enqueue(ID);
		}
		public void Clear() {
			Capacity = 0;
			IDQueue.Clear();
		}

		private void Expand(int count) {
			int i = StartIndex + Capacity;
			Capacity += count;
			for (; i < Capacity; ++i) {
				IDQueue.Enqueue(i);
			}
		}
	}
}
