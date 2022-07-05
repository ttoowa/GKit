using System;
using System.Collections.Generic;
using Type = System.Type;

#if OnUnity
namespace GKitForUnity;
#elif OnWPF
namespace GKitForWPF;
#else
namespace GKit;
#endif

public static class TypeUtility {
    public static bool IsGenericList(this Type type) {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }
}