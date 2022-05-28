﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GKitForWPF.UI.Converters {
	public class DoubleToHalfRadiusConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double radius = (double)value * 0.5d;
			return new CornerRadius(radius);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
