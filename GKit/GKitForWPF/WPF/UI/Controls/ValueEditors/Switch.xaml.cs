using GKitForWPF.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GKitForWPF.UI.Controls {
	public partial class Switch : UserControl {
		public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(nameof(Value), typeof(bool), typeof(Switch), new PropertyMetadata(false));

		private static SolidColorBrush DeactiveBackBrush = "737373".ToBrush();
		private static SolidColorBrush ActiveBackBrush = "408DC7".ToBrush();

		public event Action ValueChanged;

		public bool Value {
			get {
				return (bool)GetValue(ValueProperty);
			}
			set {
				SetValue(ValueProperty, value);
				ValueChanged?.Invoke();
			}
		}

		public Switch() {
			InitializeComponent();
			RegisterEvents();

			this.Loaded += Switch_Loaded;
		}

		private void Switch_Loaded(object sender, RoutedEventArgs e) {
			UpdateUI();
		}

		private void RegisterEvents() {
			ValueChanged += OnValueChanged;
		}

		private void OnValueChanged() {
			UpdateUI();
		}
		private void Button_Click(object sender, RoutedEventArgs e) {
			Value = !Value;
		}

		private void UpdateUI() {
			if (Value) {
				BtnBack.Fill = ActiveBackBrush;
				Button.HorizontalAlignment = HorizontalAlignment.Right;
			} else {
				BtnBack.Fill = DeactiveBackBrush;
				Button.HorizontalAlignment = HorizontalAlignment.Left;
			}
		}
	}
}

