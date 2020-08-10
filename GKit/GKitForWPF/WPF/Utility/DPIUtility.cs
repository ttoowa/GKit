using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GKitForWPF.WPF.Utility {
	public static class DPIUtility {
		public const int WindowsDefaultDPI = 96;
		public const int MacDefaultDPI = 72;

		public static Vector2 GetDPI(this Visual visual) {
			return GetDPIScale(visual) * WindowsDefaultDPI;
		}
		public static Vector2 GetDPIScale(this Visual visual) {
			PresentationSource source = PresentationSource.FromVisual(visual);
			Vector2 dpiScale = new Vector2(1f, 1f);
			if (source != null) {
				dpiScale.x = (float)source.CompositionTarget.TransformToDevice.M11;
				dpiScale.y = (float)source.CompositionTarget.TransformToDevice.M22;
				return dpiScale;
			}
			return dpiScale;
		}
	}
}
