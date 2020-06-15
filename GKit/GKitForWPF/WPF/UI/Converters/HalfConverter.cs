using System;
using System.Globalization;
using System.Windows.Data;

namespace GKitForWPF.UI.Converters {
	public class HalfConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (double)value * 0.5d;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return (double)value * 2d;
		}
	}
}
