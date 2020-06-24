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
using GKitForWPF;

namespace GKitForWPF.UI.Controls {
	public partial class Vector3Editor : UserControl {

		public Vector3 Value {
			get {
				return new Vector3(ValueTextBox_X.Value, ValueTextBox_Y.Value, ValueTextBox_Z.Value);
			}
			set {
				ValueTextBox_X.Value = value.x;
				ValueTextBox_Y.Value = value.y;
				ValueTextBox_Z.Value = value.z;
			}
		}

		private NumberEditor[] numberEditors;

		public event Action ValueChanged;

		public Vector3Editor() {
			InitializeComponent();

			numberEditors = new NumberEditor[] {
				ValueTextBox_X,
				ValueTextBox_Y,
				ValueTextBox_Z,
			};
			foreach (var numberBox in numberEditors) {
				//numberBox.MinValue = attr.minValue;
				//numberBox.MaxValue = attr.maxValue;
				//numberBox.NumberType = attr.numberType;

				numberBox.ValueChanged += ValueTextBox_ValueChanged;
			}
		}

		private void ValueTextBox_ValueChanged() {
			ValueChanged?.Invoke();
		}
	}
}
