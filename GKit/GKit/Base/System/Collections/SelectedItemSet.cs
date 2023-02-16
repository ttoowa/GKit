using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
    public class SelectedItemSet : IEnumerable<ISelectable> {
        public delegate void ItemDelegate(ISelectable item);

        public int Count => itemSet.Count;

        private readonly HashSet<ISelectable> itemSet;

        public ISelectable First => itemSet.First();
        public ISelectable Last => itemSet.Last();

        public event ItemDelegate SelectionAdded;
        public event ItemDelegate SelectionRemoved;


        public SelectedItemSet() {
            itemSet = new HashSet<ISelectable>();
        }

        public void Clear() {
            foreach (ISelectable item in itemSet.ToArray()) {
                Remove(item);
            }

            itemSet.Clear();
        }

        public void Add(ISelectable item) {
            itemSet.Add(item);
            item.SetSelected(true);

            SelectionAdded?.Invoke(item);
        }

        public void Remove(ISelectable item) {
            itemSet.Remove(item);
            item.SetSelected(false);

            SelectionRemoved?.Invoke(item);
        }

        public void SetSingle(ISelectable item) {
            if (itemSet.Count == 0 && item == null) {
                return;
            }

            if (itemSet.Count == 1 && itemSet.Contains(item)) {
                return;
            }

            Clear();
            Add(item);
        }

        public bool Contains(ISelectable item) {
            return itemSet.Contains(item);
        }

        public IEnumerable<ISelectable> Where(Func<ISelectable, bool> predicate) {
            return itemSet.Where(predicate);
        }

        public IEnumerable<TResult> Select<TResult>(Func<ISelectable, TResult> selector) {
            return itemSet.Select(selector);
        }

        public IEnumerator<ISelectable> GetEnumerator() {
            return itemSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return itemSet.GetEnumerator();
        }
    }
}