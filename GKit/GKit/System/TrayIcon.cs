#if !OnUnity
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace GKit {
	public class TrayIcon {
		public NotifyIcon Notify {
			get; private set;
		}
		public ContextMenu Menu {
			get; private set;
		}

		public event Action OnDoubleClick;

		public TrayIcon() {
			Notify = new NotifyIcon();
			Menu = new ContextMenu();
			Notify = new NotifyIcon();
			Notify.ContextMenu = Menu;
			Notify.DoubleClick += OnDoubleClick_Notify;
		}


		public void Show() {
			Notify.Visible = true;
		}
		public void Hide() {
			Notify.Visible = false;
		}

		private void OnDoubleClick_Notify(object sender, EventArgs e) {
			OnDoubleClick?.Invoke();
		}
		
		#if OnWPF
		public void SetIcon(string icoPath) {
			Uri iconUri = GResourceUtility.GetUri(icoPath);
			Stream resourceStream = Application.GetResourceStream(iconUri).Stream;
			Icon icon = new Icon(resourceStream);
			SetIcon(icon);
		}
		#endif
		public void SetIcon(Icon icon) {
			Notify.Icon = icon;
		}

		public void ClearMenu() {
			Menu.MenuItems.Clear();
		}
		public void AddMenuSeparator() {
			Menu.MenuItems.Add("-");
		}
		public MenuItem AddMenuItem(string text, Action OnClick = null) {
			MenuItem item = new MenuItem(text, (object sender, EventArgs e) => { OnClick?.Invoke(); });
			Menu.MenuItems.Add(item);

			return item;
		}
		public MenuItem AddCheckableMenuItem(string text, Arg1Delegate<bool> OnClick = null) {
			MenuItem item = null;
			item = new MenuItem(text, (object sender, EventArgs e) => {
				item.Checked = !item.Checked;
				OnClick?.Invoke(item.Checked);
			});
			Menu.MenuItems.Add(item);

			return item;
		}
	}
}
#endif