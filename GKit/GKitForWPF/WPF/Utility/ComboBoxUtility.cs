using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GKitForWPF {
	public class GComboBoxItem {
		public string DisplayName {
			get; set;
		}
		public object Value {
			get; set;
		}

		public GComboBoxItem() {

		}
		public GComboBoxItem(string name, object value) {
			this.DisplayName = name;
			this.Value = value;
		}
	}

	public static class ComboBoxUtility {
		public static GComboBoxItem[] GetEnumItems(Type enumType) {
			Array enumValues = Enum.GetValues(enumType);
			GComboBoxItem[] items = new GComboBoxItem[enumValues.Length];
			for (int i = 0; i < enumValues.Length; ++i) {
				object enumValue = enumValues.GetValue(i);
				FieldInfo field = enumType.GetField(enumValue.ToString());
				EnumTextAttribute[] attrs = field.GetCustomAttributes<EnumTextAttribute>() as EnumTextAttribute[];

				GComboBoxItem item = new GComboBoxItem();
				item.Value = enumValue;
				if (attrs != null && attrs.Any()) {
					item.DisplayName = attrs[0].text;
				} else {
					item.DisplayName = enumValue.ToString();
				}
				items[i] = item;
			}
			return items;
		}
	}
}
