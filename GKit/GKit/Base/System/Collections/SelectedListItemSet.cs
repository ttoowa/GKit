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

		private HashSet<ISelectable> itemSet;

		public ISelectable First => itemSet.First();
		public ISelectable Last => itemSet.Last();

		public event ItemDelegate SelectionAdded;
		public event ItemDelegate SelectionRemoved;


		public SelectedItemSet() {
			itemSet = new HashSet<ISelectable>();
		}

		//Control
		public void AddSelectedItem(ISelectable item) {
			itemSet.Add(item);
			item.SetSelected(true);

			SelectionAdded?.Invoke(item);
		}
		public void RemoveSelectedItem(ISelectable item) {
			itemSet.Remove(item);
			item.SetSelected(false);

			SelectionRemoved?.Invoke(item);
		}
		public void SetSelectedItem(ISelectable item) {
			UnselectItems();
			AddSelectedItem(item);
		}
		public void UnselectItems() {
			foreach (ISelectable item in itemSet.ToArray()) {
				RemoveSelectedItem(item);
			}
			itemSet.Clear();
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
