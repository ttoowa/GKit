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

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public class TrayIcon {
		public NotifyIcon Notify {
			get; private set;
		}
		public ContextMenuStrip Menu {
			get; private set;
		}

		public event Action OnDoubleClick;

		public TrayIcon() {
			Notify = new NotifyIcon();
			Menu = new ContextMenuStrip();
			Notify = new NotifyIcon();
			Notify.ContextMenuStrip = Menu;
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
			Menu.Items.Clear();
		}
		public void AddMenuSeparator() {
			Menu.Items.Add("-");
		}
		public ToolStripMenuItem AddMenuItem(string text, Action OnClick = null) {
			ToolStripMenuItem item = new ToolStripMenuItem(text, null, (object sender, EventArgs e) => { OnClick?.Invoke(); });
			Menu.Items.Add(item);

			return item;
		}
		public ToolStripMenuItem AddCheckableMenuItem(string text, Arg1Delegate<bool> OnClick = null) {
			ToolStripMenuItem item = null;
			item = new ToolStripMenuItem(text, null, (object sender, EventArgs e) => {
				item.Checked = !item.Checked;
				OnClick?.Invoke(item.Checked);
			});
			Menu.Items.Add(item);

			return item;
		}
	}
}
#endif