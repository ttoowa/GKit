using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Utf8Json.Resolvers;
using Utf8Json;
using Utf8Json.Internal;
using Utf8Json.Formatters;
using System.Reflection;

namespace GKit.Utf8JsonUtility;

public class FieldsOnlyFormatter<T> : IJsonFormatter<T> {
    private static readonly ConcurrentDictionary<Type, object> FormatterCache = new();
    
    public static IJsonFormatter<T> GetFormatter<T>() {
        return (IJsonFormatter<T>)FormatterCache.GetOrAdd(typeof(T), _ => new FieldsOnlyFormatter<T>());
    }
    
    public void Serialize(ref JsonWriter writer, T value, IJsonFormatterResolver formatterResolver) {
        if (value == null) {
            writer.WriteNull();
            return;
        }
        
        writer.WriteBeginObject();
        bool first = true;
        foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public)) {
            object fieldValue = field.GetValue(value);
            if (IsDefaultValue(field.FieldType, fieldValue)) continue;
            
            if (!first) writer.WriteValueSeparator();
            writer.WriteString(field.Name);
            writer.WriteNameSeparator();
            
            // primitive type 일 경우 Write, 아니면 Serialize
            
            JsonSerializer.NonGeneric.Serialize(field.FieldType, ref writer, fieldValue, formatterResolver);
            first = false;
        }
        
        writer.WriteEndObject();
    }
    
    public T Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) {
        if (reader.ReadIsNull()) return default;
        
        T obj = Activator.CreateInstance<T>();
        reader.ReadIsBeginObjectWithVerify();
        
        Dictionary<string, FieldInfo> fieldDict = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(f => f.Name, f => f);
        
        int count = 0;
        while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count)) {
            string propertyName = reader.ReadPropertyName();
            if (fieldDict.TryGetValue(propertyName, out FieldInfo field)) {
                object value = JsonSerializer.NonGeneric.Deserialize(field.FieldType, ref reader, formatterResolver);
                field.SetValue(obj, value);
            } else {
                reader.ReadNextBlock();
            }
        }
        
        return obj;
    }
    
    private bool IsDefaultValue(Type type, object value) {
        if (type.IsValueType) {
            return value.Equals(Activator.CreateInstance(type));
        }
        
        return value == null;
    }
}