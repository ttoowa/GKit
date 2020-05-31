#if OnWPF
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace GKit.WPF {
	public static class IgnoreWindowUtility {
		private const int WS_EX_TRANSPARENT = 0x00000020;
		private const int GWL_EXSTYLE = (-20);
		private const int WM_NCHITTEST = 0x0084;
		[DllImport("user32.dll")]
		public static extern int GetWindowLong(IntPtr hwnd, int index);
		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

		/// <summary>
		/// OnSourceInitialized 함수를 오버라이드하여 base함수호출 이후 호출하세요.
		/// </summary>
		/// <param name="window"></param>
		public static void SetIgnoreWindow(this Window window) {
			IntPtr hwnd = new WindowInteropHelper(window).Handle;

			// Change the extended window style to include WS_EX_TRANSPARENT
			int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
			SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);

			window.Loaded += (object sender, RoutedEventArgs e) => {
				window.IsHitTestVisible = false;
				window.IsEnabled = false;
				window.Focusable = false;
				window.ShowInTaskbar = false;
			};
		}
	}
}
#endif
