using System;
using System.Collections.Generic;

#if OnUnity
namespace GKitForUnity;
#elif OnWPF
namespace GKitForWPF;
#else
namespace GKit;
#endif

public class InheritFactory<TType, TBase> : IEnumerable<KeyValuePair<TType, Type>> where TBase : class where TType : Enum  {
    // 파생 타입 Dict
    private static readonly Dictionary<TType, Type> inheritTypeDict = new();
    
    public InheritFactory(params KeyValuePair<TType, Type>[] pairs) {
        foreach (KeyValuePair<TType, Type> pair in pairs) {
            inheritTypeDict[pair.Key] = pair.Value;
        }
    }
    
    public TBase CreateInstance(TType type) {
        if (!inheritTypeDict.TryGetValue(type, out Type? typeValue)) {
            throw new Exception($"[InheritFactory.CreateInstance] Unknown Enum Type");
        }
        
        return Activator.CreateInstance(typeValue) as TBase;
    }
    
    public IEnumerator<KeyValuePair<TType, Type>> GetEnumerator() {
        return inheritTypeDict.GetEnumerator();
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return inheritTypeDict.GetEnumerator();
    }
    
    public void Add(TType type, Type typeValue) {
        inheritTypeDict[type] = typeValue;
    }
    
    public void Remove(TType type) {
        inheritTypeDict.Remove(type);
    }
    
    public void Clear() {
        inheritTypeDict.Clear();
    }
    
    public bool Contains(TType type) {
        return inheritTypeDict.ContainsKey(type);
    }
    
    public Type GetType(TType type) {
        return inheritTypeDict[type];
    }
    
    public bool TryGetType(TType type, out Type typeValue) {
        return inheritTypeDict.TryGetValue(type, out typeValue);
    }
}