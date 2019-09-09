using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKit.WPF.UI.Controls {
	public class SelectedListItemSet : IEnumerable<IListItem> {
		public int Count => itemSet.Count;

		private HashSet<IListItem> itemSet;

		public IListItem First => itemSet.First();
		public IListItem Last => itemSet.Last();

		public event ListItemDelegate SelectionAdded;
		public event ListItemDelegate SelectionRemoved;


		public SelectedListItemSet() {
			itemSet = new HashSet<IListItem>();
		}

		//Control
		public void AddSelectedItem(IListItem item) {
			itemSet.Add(item);
			item.SetDisplaySelected(true);

			SelectionAdded?.Invoke(item);
		}
		public void RemoveSelectedItem(IListItem item) {
			itemSet.Remove(item);
			item.SetDisplaySelected(false);

			SelectionRemoved?.Invoke(item);
		}
		public void SetSelectedItem(IListItem item) {
			UnselectItems();
			AddSelectedItem(item);
		}
		public void UnselectItems() {
			foreach (IListItem item in itemSet.ToArray()) {
				RemoveSelectedItem(item);
			}
			itemSet.Clear();
		}

		public bool Contains(IListItem item) {
			return itemSet.Contains(item);
		}
		public IEnumerable<IListItem> Where(Func<IListItem, bool> predicate) {
			return itemSet.Where(predicate);
		}
		public IEnumerable<TResult> Select<TResult>(Func<IListItem, TResult> selector) {
			return itemSet.Select(selector);
		}

		public IEnumerator<IListItem> GetEnumerator() {
			return itemSet.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return itemSet.GetEnumerator();
		}
	}
}
