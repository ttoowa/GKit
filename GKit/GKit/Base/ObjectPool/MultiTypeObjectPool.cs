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

        private readonly Dictionary<Type, ObjectPool<IPoolable>> poolDict;

        public MultiTypeObjectPool() {
            poolDict = new Dictionary<Type, ObjectPool<IPoolable>>();
        }

        public IPoolable GetInstance(Type type) {
            ObjectPool<IPoolable> pool;
            if (poolDict.ContainsKey(type)) {
                pool = poolDict[type];
            } else {
                pool = new ObjectPool<IPoolable>();
                poolDict.Add(type, pool);
            }

            return pool.GetInstance();
        }
        public void ReturnInstance(IPoolable instance) {
            Type type = instance.GetType();
            ObjectPool<IPoolable> pool;
            if (poolDict.ContainsKey(type)) {
                pool = poolDict[type];
            } else {
                pool = new ObjectPool<IPoolable>();
                poolDict.Add(type, pool);
            }

            pool.ReturnInstance(instance);
        }

        private ObjectPool<IPoolable> GetOrCreatePool<T>() {
            ObjectPool<IPoolable> pool;
            if (poolDict.ContainsKey(type)) {
                pool = poolDict[type];
            } else {
                pool = new ObjectPool<IPoolable>();
                poolDict.Add(type, pool);

                pool.CreateInstanceMethod = () => {

                };
            }

            return pool;
        }
    }
}
