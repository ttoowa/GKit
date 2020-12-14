using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace GKitForWPF.UI.Windows {
	public partial class ToastMessage : Window {
		private const int WS_EX_TRANSPARENT = 0x00000020;
		private const int GWL_EXSTYLE = (-20);
		private const int WM_NCHITTEST = 0x0084;
		[DllImport("user32.dll")]
		public static extern int GetWindowLong(IntPtr hwnd, int index);
		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

		// [ Static function ]
		public static void Show(string text) {
			ToastMessage toastMessage = new ToastMessage(text);
			toastMessage.Show();
		}
		// [ Constructor ]
		private ToastMessage() {
			InitializeComponent();
			//for WPFDesigner
		}
		private ToastMessage(string text) {
			InitializeComponent();
			this.MessageText.Text = text;
			Loaded += OnLoaded;
		}

		// [ Event ]
		private void OnLoaded(object sender, RoutedEventArgs e) {
			PlayAnimation();
		}
		protected override void OnSourceInitialized(EventArgs e) {
			base.OnSourceInitialized(e);

			// Get this window's handle
			IntPtr hwnd = new WindowInteropHelper(this).Handle;

			// Change the extended window style to include WS_EX_TRANSPARENT
			int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
			SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
		}
		private void OpacityAnim_Completed(object sender, EventArgs e) {
			Close();
		}

		
		private void PlayAnimation() {
			DoubleAnimation opacityAnim = new DoubleAnimation() {
				BeginTime = TimeSpan.FromSeconds(0.8d),
				From = 1d,
				To = 0d,
				Duration = new Duration(TimeSpan.FromSeconds(1d)),
			};
			opacityAnim.Completed += OpacityAnim_Completed;
			this.BeginAnimation(Window.OpacityProperty, opacityAnim);
		}

	}
}
