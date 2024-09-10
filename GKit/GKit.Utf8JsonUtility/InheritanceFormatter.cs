using System;
using System.Collections;
using System.Collections.Generic;
using Utf8Json;

namespace GKit.Utf8JsonUtility;

public class InheritanceFormatter<EnumType, BaseModel> : IEnumerable, IJsonFormatter<BaseModel> where EnumType : Enum where BaseModel : class {
    public delegate EnumType GetEnumTypeDelegate(BaseModel value);
    
    private Dictionary<EnumType, Type> typeDict;
    private GetEnumTypeDelegate getEnumType;
    
    public InheritanceFormatter(GetEnumTypeDelegate getEnumType) {
        this.getEnumType = getEnumType;
        typeDict = new Dictionary<EnumType, Type>();
    }
    
    public void Serialize(ref JsonWriter writer, BaseModel value, IJsonFormatterResolver formatterResolver) {
        EnumType type = getEnumType(value);
        if (!typeDict.TryGetValue(type, out Type? typeValue)) {
            throw new Exception("Unknown Enum Type");
        }
        
        JsonSerializer.NonGeneric.Serialize(typeValue, ref writer, value, formatterResolver);
    }
    
    public BaseModel Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) {
        int offset = reader.GetCurrentOffsetUnsafe();
        BaseModel baseModel = JsonSerializer.Deserialize<BaseModel>(ref reader);
        EnumType type = getEnumType(baseModel);
        
        reader.AdvanceOffset(offset - reader.GetCurrentOffsetUnsafe());
        
        if (!typeDict.TryGetValue(type, out Type? typeValue)) {
            throw new Exception("Unknown Enum Type");
        }
        
        return JsonSerializer.NonGeneric.Deserialize(typeValue, ref reader, formatterResolver) as BaseModel;
    }
    
    public BaseModel CreateInstance(EnumType type) {
        if (!typeDict.TryGetValue(type, out Type? typeValue)) {
            throw new Exception("Unknown Enum Type");
        }
        
        return Activator.CreateInstance(typeValue) as BaseModel;
    }
    
    public IEnumerator GetEnumerator() {
        return typeDict.GetEnumerator();
    }
    
    public void Add(EnumType type, Type typeValue) {
        typeDict[type] = typeValue;
    }
    
    public void Remove(EnumType type) {
        typeDict.Remove(type);
    }
    
    public void Clear() {
        typeDict.Clear();
    }
    
    public bool Contains(EnumType type) {
        return typeDict.ContainsKey(type);
    }
    
    public Type GetType(EnumType type) {
        return typeDict[type];
    }
    
    public bool TryGetType(EnumType type, out Type typeValue) {
        return typeDict.TryGetValue(type, out typeValue);
    }
}