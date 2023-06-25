using System.Collections.Generic;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
    public class TwoWayDictionary<T1, T2> {
        private Dictionary<T1, T2> forward = new();
        private Dictionary<T2, T1> reverse = new();

        public TwoWayDictionary() {
            Forward = new Indexer<T1, T2>(forward);
            Reverse = new Indexer<T2, T1>(reverse);
        }

        public class Indexer<T3, T4> {
            private Dictionary<T3, T4> _dictionary;

            public Indexer(Dictionary<T3, T4> dictionary) {
                _dictionary = dictionary;
            }

            public T4 this[T3 index] {
                get => _dictionary[index];
                set => _dictionary[index] = value;
            }

            public bool ContainsKey(T3 index) {
                return _dictionary.ContainsKey(index);
            }
        }

        public void Add(T1 t1, T2 t2) {
            forward.Add(t1, t2);
            reverse.Add(t2, t1);
        }

        public bool Remove(T1 t1, T2 t2) {
            bool result = forward.Remove(t1);
            result &= reverse.Remove(t2);
            return result;
        }

        public bool Contains(T1 t1, T2 t2) {
            bool result = forward.ContainsKey(t1);
            result &= reverse.ContainsKey(t2);
            return result;
        }

        public Indexer<T1, T2> Forward { get; private set; }
        public Indexer<T2, T1> Reverse { get; private set; }
    }
}