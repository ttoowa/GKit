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
    public class MultiTypeObjectPool {
        public delegate IPoolable CreateInstanceDelegate(Type type);

        private readonly Dictionary<Type, ObjectPool<IPoolable>> poolDict;

        private readonly CreateInstanceDelegate CreateInstanceMethod;
        private readonly Arg1Delegate<IPoolable> DisposeInstanceMethod;
        private readonly Arg1Delegate<IPoolable> GetInstanceMethod;
        private readonly Arg1Delegate<IPoolable> ReturnInstanceMethod;


        public MultiTypeObjectPool(CreateInstanceDelegate createInstanceMethod = null, Arg1Delegate<IPoolable> disposeInstanceMethod = null,
         Arg1Delegate<IPoolable> getInstanceMethod = null, Arg1Delegate<IPoolable> returnInstanceMethod = null) {
            poolDict = new Dictionary<Type, ObjectPool<IPoolable>>();

            this.CreateInstanceMethod = createInstanceMethod;
            this.DisposeInstanceMethod = disposeInstanceMethod;
            this.GetInstanceMethod = getInstanceMethod;
            this.ReturnInstanceMethod = returnInstanceMethod;
        }

        public IPoolable GetInstance(Type type) {
            ObjectPool<IPoolable> pool = GetOrCreatePool(type);

            return pool.GetInstance();
        }
        public void ReturnInstance(IPoolable instance) {
            Type type = instance.GetType();
            ObjectPool<IPoolable> pool = GetOrCreatePool(type);
            pool.ReturnInstance(instance);
        }

        private ObjectPool<IPoolable> GetOrCreatePool(Type type) {
            ObjectPool<IPoolable> pool;
            if (poolDict.ContainsKey(type)) {
                pool = poolDict[type];
            } else {
                pool = new ObjectPool<IPoolable>();
                poolDict.Add(type, pool);

                pool.CreateInstanceMethod = CreateInstanceMethod;
                pool.DisposeInstanceMethod += (IPoolable instance) => {
                    DisposeInstanceMethod?.Invoke(instance);
                };
                pool.GetInstanceMethod += (IPoolable instance) => {
                    GetInstanceMethod?.Invoke(instance);
                };
                pool.ReturnInstanceMethod += (IPoolable instance) => {
                    ReturnInstanceMethod?.Invoke(instance);
                };
            }

            return pool;
        }
    }
}
