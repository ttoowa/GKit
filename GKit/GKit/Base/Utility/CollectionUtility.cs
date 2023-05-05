using System;
using System.Collections.Generic;

#if OnUnity
namespace GKitForUnity;
#elif OnWPF
namespace GKitForWPF;
#else
namespace GKit;
#endif

public static class CollectionUtility {
    public static bool IsSameElements<T>(this IReadOnlyList<T> srcList, IReadOnlyList<T> targetList) {
        if (srcList.Count != targetList.Count)
            return false;

        for (int i = 0; i < srcList.Count; ++i) {
            if (!srcList[i].Equals(targetList[i])) return false;
        }

        return true;
    }
}