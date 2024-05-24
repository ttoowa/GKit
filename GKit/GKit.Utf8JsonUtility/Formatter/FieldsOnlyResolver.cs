using System;
using System.Collections.Concurrent;
using Utf8Json;

namespace GKit.Utf8JsonUtility;

public class FieldsOnlyResolver : IJsonFormatterResolver {
    public static readonly FieldsOnlyResolver Instance;
    
    private static readonly ConcurrentDictionary<Type, object> FormatterCache = new();
    
    static FieldsOnlyResolver() {
        Instance = new FieldsOnlyResolver();
    }
    
    private FieldsOnlyResolver() {
    }
    
    public IJsonFormatter<T> GetFormatter<T>() {
        return (IJsonFormatter<T>)FormatterCache.GetOrAdd(typeof(T), _ => new FieldsOnlyFormatter<T>());
    }
}