using System;
using System.ComponentModel;
using System.Reflection;

#if OnUnity
namespace GKitForUnity;
#elif OnWPF
namespace GKitForWPF;
#else
namespace GKit;
#endif

public class EnumTextAttribute : Attribute {
    public string text;
    
    public EnumTextAttribute(string text) {
        this.text = text;
    }
}

public static class EnumUtility {
    public static string ToStringWithDescAttr(this Enum enumValue) {
        Type type = enumValue.GetType();
        string defaultString = enumValue.ToString();
        MemberInfo[] memberInfos = type.GetMember(defaultString);
        if (memberInfos.Length > 0) {
            object[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            
            if (attrs.Length > 0) {
                for (int i = 0; i < attrs.Length; ++i) {
                    object attr = attrs[i];
                    if (attr is DescriptionAttribute) {
                        return ((DescriptionAttribute)attr).Description;
                    }
                }
            }
        }
        
        return defaultString;
    }
    
    public static string ToStringWithEnumTextAttr(this Enum enumValue) {
        Type type = enumValue.GetType();
        string defaultString = enumValue.ToString();
        MemberInfo[] memberInfos = type.GetMember(defaultString);
        if (memberInfos.Length > 0) {
            object[] attrs = memberInfos[0].GetCustomAttributes(typeof(EnumTextAttribute), false);
            
            if (attrs.Length > 0) {
                for (int i = 0; i < attrs.Length; ++i) {
                    object attr = attrs[i];
                    if (attr is EnumTextAttribute) {
                        return ((EnumTextAttribute)attr).text;
                    }
                }
            }
        }
        
        return defaultString;
    }
}