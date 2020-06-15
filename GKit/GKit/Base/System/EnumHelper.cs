using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
    public static class EnumHelper
    {
        public static string ToStringWithDesc(this Enum enumValue)
        {
            Type type = enumValue.GetType();
            string defaultString = enumValue.ToString();
            MemberInfo[] memberInfos = type.GetMember(defaultString);
            if(memberInfos.Length > 0)
            {
                object[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if(attrs.Length > 0)
                {
                    for(int i=0; i<attrs.Length; ++i)
                    {
                        object attr = attrs[i];
                        if(attr is DescriptionAttribute)
                        {
                            return ((DescriptionAttribute)attr).Description;
                        }
                    }
                }
            }
            return defaultString;
        }
    }
}
