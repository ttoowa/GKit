using System;
using System.Collections.Generic;
using Utf8Json;
using Utf8Json.Resolvers;

namespace GKit.Utf8JsonUtility;

public class Utf8JsonFormatterResolver : IJsonFormatterResolver {
    public static Utf8JsonFormatterResolver Instance = new();
    
    private readonly Dictionary<Type, IJsonFormatter> formatterDict = new();
    
    private Utf8JsonFormatterResolver() {
    }
    
    public void AddFormatter<T>(IJsonFormatter<T> formatter) {
        formatterDict[typeof(T)] = formatter;
    }
    
    public IJsonFormatter<T> GetFormatter<T>() {
        if (formatterDict.TryGetValue(typeof(T), out IJsonFormatter? typeFormatter)) {
            return (IJsonFormatter<T>)typeFormatter;
        }
        
        return StandardResolver.ExcludeNull.GetFormatter<T>();
    }
}