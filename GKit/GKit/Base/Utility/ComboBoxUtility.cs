using System;
using System.Linq;
using System.Reflection;

#if OnUnity
namespace GKitForUnity;
#elif OnWPF
namespace GKitForWPF;
#else
namespace GKit;
#endif

public class GComboBoxItem {
    public string DisplayName { get; set; }
    public object Value { get; set; }
    
    public GComboBoxItem() {
    }
    
    public GComboBoxItem(string name, object value) {
        DisplayName = name;
        Value = value;
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
            
            GComboBoxItem item = new();
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