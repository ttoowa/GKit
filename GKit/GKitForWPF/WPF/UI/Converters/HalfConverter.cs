using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GKit.WPF.UI.Converters {
	public class HalfConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (double)value * 0.5d;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return (double)value * 2d;
		}
	}
}
