// using System;
// using System.Collections.Generic;
// #if OnUnity
// using UnityEngine;
// #endif

// #if OnUnity
// namespace GKitForUnity
// #elif OnWPF
// namespace GKitForWPF
// #else
// namespace GKit
// #endif
// {

//     public class MultiTypeObjectPool {
//         //public int Count => targetCount;

//         //public bool IsFull => objectStack.Count == targetCount;
//         //private int targetCount;
//         //private Stack<T> objectStack;
//         //public ReturnDelegate<T> CreateInstanceMethod;
//         //public event Arg1Delegate<T> DisposeInstanceTask;
//         //public event Arg1Delegate<T> GetInstanceTask;
//         //public event Arg1Delegate<T> ReturnInstanceTask;

//         private Dictionary<Type, ObjectPool<IPoolable>> poolDict = new Dictionary<Type, ObjectPool<IPoolable>>();

//         public MultiTypeObjectPool() {
//         }

//         public IPoolable GetInstance(Type type) {
//             ObjectPool<IPoolable> pool;
//             if (poolDict.ContainsKey(type)) {
//                 pool = poolDict[type];
//             } else {
//                 pool = new ObjectPool<IPoolable>();
//                 poolDict.Add(type, pool);
//             }

//             return pool.GetInstance();

//             if (objectStack.Count == 0) {
//                 CreateInstance(1);
//             }
//             T instance = objectStack.Pop();
//             GetInstanceTask?.Invoke(instance);
//             return instance;
//         }
//         public void ReturnInstance(T instance) {
//             ReturnInstanceTask?.Invoke(instance);
//             if (objectStack.Count < targetCount) {
//                 objectStack.Push(instance);
//             } else {
//                 DisposeInstanceTask?.Invoke(instance);
//             }
//         }

//         public void CreateInstance(int count) {
//             if (count == 0)
//                 return;

//             this.targetCount += count;
//             for (int i = 0; i < count; ++i) {
//                 T instance;
//                 if (CreateInstanceMethod != null) {
//                     instance = CreateInstanceMethod();
//                 } else {
//                     instance = default(T);
//                 }
//                 ReturnInstanceTask?.Invoke(instance);
//                 objectStack.Push(instance);
//             }
//         }
//         public void DeleteInstance(int deleteCount) {
//             deleteCount = Mathf.Min(deleteCount, targetCount);
//             targetCount = Mathf.Max(targetCount - deleteCount, 0);

//             int immediateDeleteCount = Mathf.Min(deleteCount, objectStack.Count);
//             for (int i = 0; i < immediateDeleteCount; ++i) {
//                 OnDisposeInstance(objectStack.Pop());
//             }
//         }

//         private void OnDisposeInstance(T obj) {
//             DisposeInstanceTask?.Invoke(obj);
//         }
//     }
// }
