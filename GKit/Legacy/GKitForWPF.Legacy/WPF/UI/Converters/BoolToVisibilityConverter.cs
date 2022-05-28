﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GKitForWPF.UI.Converters {
	public class BoolToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			bool isVisible = (bool)value;
			return isVisible ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			Visibility visibility = (Visibility)value;
			return visibility == Visibility.Visible ? true : false;
		}
	}
}
