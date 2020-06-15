using System.Collections.Generic;
#if OnUnity
using UnityEngine;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public class ObjectPool<T> {
		public int Count => targetCount;

		public bool IsFull => objectStack.Count == targetCount;
		private int targetCount;
		private Stack<T> objectStack;
		public ReturnDelegate<T> CreateMethod;
		public event Arg1Delegate<T> DisposeTask;
		public event Arg1Delegate<T> GetTask;
		public event Arg1Delegate<T> ReleaseTask;

		public ObjectPool(int count = 32) {
			Init(count);
		}
		public ObjectPool(ReturnDelegate<T> createObjectMethod, int count = 32) {
			CreateMethod = createObjectMethod;

			Init(count);
		}
		private void Init(int count) {
			targetCount = count;
			objectStack = new Stack<T>(count);
			CreateObject(count);
		}

		public T GetObject() {
			if (objectStack.Count == 0) {
				CreateObject(1);
			}
			T instance = objectStack.Pop();
			GetTask?.Invoke(instance);
			return instance;
		}
		public void ReleaseObject(T obj) {
			ReleaseTask?.Invoke(obj);
			if (objectStack.Count < targetCount) {
				objectStack.Push(obj);
			} else {
				DisposeTask?.Invoke(obj);
			}
		}

		public void CreateObject(int count) {
			this.targetCount += count;
			T item;
			if (CreateMethod != null) {
				item = CreateMethod();
			} else {
				item = default(T);
			}
			objectStack.Push(item);
		}
		public void DeleteObject(int deleteCount) {
			deleteCount = Mathf.Min(deleteCount, targetCount);
			targetCount = Mathf.Max(targetCount - deleteCount, 0);

			int immediateDeleteCount = Mathf.Min(deleteCount, objectStack.Count);
			for (int i = 0; i < immediateDeleteCount; ++i) {
				OnDisposeObject(objectStack.Pop());
			}
		}

		private void OnDisposeObject(T obj) {
			DisposeTask?.Invoke(obj);
		}
	}
}
