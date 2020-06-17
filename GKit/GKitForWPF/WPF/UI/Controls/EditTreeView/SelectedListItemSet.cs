using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GKitForWPF.UI.Controls {
	public class SelectedItemSet : IEnumerable<ITreeItem> {
		public int Count => itemSet.Count;

		private HashSet<ITreeItem> itemSet;

		public ITreeItem First => itemSet.First();
		public ITreeItem Last => itemSet.Last();

		public event ListItemDelegate SelectionAdded;
		public event ListItemDelegate SelectionRemoved;


		public SelectedItemSet() {
			itemSet = new HashSet<ITreeItem>();
		}

		//Control
		public void AddSelectedItem(ITreeItem item) {
			itemSet.Add(item);
			item.SetDisplaySelected(true);

			SelectionAdded?.Invoke(item);
		}
		public void RemoveSelectedItem(ITreeItem item) {
			itemSet.Remove(item);
			item.SetDisplaySelected(false);

			SelectionRemoved?.Invoke(item);
		}
		public void SetSelectedItem(ITreeItem item) {
			UnselectItems();
			AddSelectedItem(item);
		}
		public void UnselectItems() {
			foreach (ITreeItem item in itemSet.ToArray()) {
				RemoveSelectedItem(item);
			}
			itemSet.Clear();
		}

		public bool Contains(ITreeItem item) {
			return itemSet.Contains(item);
		}
		public IEnumerable<ITreeItem> Where(Func<ITreeItem, bool> predicate) {
			return itemSet.Where(predicate);
		}
		public IEnumerable<TResult> Select<TResult>(Func<ITreeItem, TResult> selector) {
			return itemSet.Select(selector);
		}

		public IEnumerator<ITreeItem> GetEnumerator() {
			return itemSet.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return itemSet.GetEnumerator();
		}
	}
}
