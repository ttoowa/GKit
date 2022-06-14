using System;
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
	public class ObjectPool {
		public int Count => targetCount;

		public bool IsFull => objectStack.Count == targetCount;
		private int targetCount;
		private Stack<object> objectStack;
		public ReturnDelegate<object> CreateInstanceMethod;
		public event Arg1Delegate<object> DisposeInstanceMethod;
		public event Arg1Delegate<object> GetInstanceMethod;
		public event Arg1Delegate<object> ReturnInstanceMethod;

		public ObjectPool(ReturnDelegate<object> createObjectMethod, int count = 32) {
			CreateInstanceMethod = createObjectMethod;
			if (createObjectMethod == null) {
				throw new ArgumentNullException("createObjectMethod cannot be null.");
			}

			Init(count);
		}
		private void Init(int count) {
			targetCount = count;
			objectStack = new Stack<object>(count);
			CreateInstance(count);
		}

		public object GetInstance() {
			if (objectStack.Count == 0) {
				CreateInstance(1);
			}
			object instance = objectStack.Pop();
			GetInstanceMethod?.Invoke(instance);
			return instance;
		}
		public void ReturnInstance(object instance) {
			ReturnInstanceMethod?.Invoke(instance);
			if (objectStack.Count < targetCount) {
				objectStack.Push(instance);
			} else {
				DisposeInstanceMethod?.Invoke(instance);
			}
		}

		public void CreateInstance(int count) {
			if (count == 0)
				return;

			this.targetCount += count;
			for (int i = 0; i < count; ++i) {
				object instance = CreateInstanceMethod();
				ReturnInstanceMethod?.Invoke(instance);
				objectStack.Push(instance);
			}
		}
		public void DeleteInstance(int deleteCount) {
			deleteCount = Mathf.Min(deleteCount, targetCount);
			targetCount = Mathf.Max(targetCount - deleteCount, 0);

			int immediateDeleteCount = Mathf.Min(deleteCount, objectStack.Count);
			for (int i = 0; i < immediateDeleteCount; ++i) {
				InvokeDisposeInstanceMethod(objectStack.Pop());
			}
		}

		private void InvokeDisposeInstanceMethod(object obj) {
			DisposeInstanceMethod?.Invoke(obj);
		}
	}

	public class ObjectPool<T> {
		public int Count => targetCount;

		public bool IsFull => objectStack.Count == targetCount;
		private int targetCount;
		private Stack<T> objectStack;
		public readonly ReturnDelegate<T> CreateInstanceMethod;
		public event Arg1Delegate<T> DisposeInstanceMethod;
		public event Arg1Delegate<T> GetInstanceMethod;
		public event Arg1Delegate<T> ReturnInstanceMethod;

		public ObjectPool() {
			Init();
		}
		public ObjectPool(ReturnDelegate<T> createObjectMethod) {
			CreateInstanceMethod = createObjectMethod;

			Init();
		}
		private void Init() {
			targetCount = 0;
			objectStack = new Stack<T>();
		}

		public T GetInstance() {
			if (objectStack.Count == 0) {
				CreateInstance(1);
			}
			T instance = objectStack.Pop();
			GetInstanceMethod?.Invoke(instance);
			return instance;
		}
		public void ReturnInstance(T instance) {
			ReturnInstanceMethod?.Invoke(instance);
			if (objectStack.Count < targetCount) {
				objectStack.Push(instance);
			} else {
				DisposeInstanceMethod?.Invoke(instance);
			}
		}

		public void CreateInstance(int count) {
			if (count == 0)
				return;

			this.targetCount += count;
			for (int i = 0; i < count; ++i) {
				T instance;
				if (CreateInstanceMethod != null) {
					instance = CreateInstanceMethod();
				} else {
					instance = default(T);
				}
				ReturnInstanceMethod?.Invoke(instance);
				objectStack.Push(instance);
			}
		}
		public void DeleteInstance(int deleteCount) {
			deleteCount = Mathf.Min(deleteCount, targetCount);
			targetCount = Mathf.Max(targetCount - deleteCount, 0);

			int immediateDeleteCount = Mathf.Min(deleteCount, objectStack.Count);
			for (int i = 0; i < immediateDeleteCount; ++i) {
				InvokeDisposeInstanceMethod(objectStack.Pop());
			}
		}

		private void InvokeDisposeInstanceMethod(T obj) {
			DisposeInstanceMethod?.Invoke(obj);
		}
	}
}
