using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GKit.TextJsonUtility
{
    public class InheritanceConverter<EnumType, BaseModel> : JsonConverter<BaseModel>, IEnumerable where EnumType : Enum where BaseModel : class
    {
        public delegate EnumType GetEnumTypeDelegate(BaseModel value);

        private readonly Dictionary<EnumType, Type> typeDict;
        private readonly GetEnumTypeDelegate getEnumType;

        public InheritanceConverter(GetEnumTypeDelegate getEnumType)
        {
            this.getEnumType = getEnumType;
            this.typeDict = new Dictionary<EnumType, Type>();
        }

        public override void Write(Utf8JsonWriter writer, BaseModel value, JsonSerializerOptions options)
        {
            EnumType type = getEnumType(value);
            if (!typeDict.TryGetValue(type, out Type typeValue))
            {
                throw new Exception("Unknown Enum Type");
            }

            JsonSerializer.Serialize(writer, value, typeValue, options);
        }

        public override BaseModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            long originalPosition = reader.TokenStartIndex;
            BaseModel baseModel = (BaseModel)JsonSerializer.Deserialize(ref reader, typeof(BaseModel), options);
            EnumType type = getEnumType(baseModel);

            reader = new Utf8JsonReader(reader.ValueSpan.Slice((int)originalPosition));

            if (!typeDict.TryGetValue(type, out Type typeValue))
            {
                throw new Exception("Unknown Enum Type");
            }

            return (BaseModel)JsonSerializer.Deserialize(ref reader, typeValue, options);
        }

        public BaseModel CreateInstance(EnumType type)
        {
            if (!typeDict.TryGetValue(type, out Type typeValue))
            {
                throw new Exception("Unknown Enum Type");
            }

            return Activator.CreateInstance(typeValue) as BaseModel;
        }

        public IEnumerator GetEnumerator()
        {
            return typeDict.GetEnumerator();
        }

        public void Add(EnumType type, Type typeValue)
        {
            typeDict[type] = typeValue;
        }

        public void Remove(EnumType type)
        {
            typeDict.Remove(type);
        }

        public void Clear()
        {
            typeDict.Clear();
        }

        public bool Contains(EnumType type)
        {
            return typeDict.ContainsKey(type);
        }

        public Type GetType(EnumType type)
        {
            return typeDict[type];
        }

        public bool TryGetType(EnumType type, out Type typeValue)
        {
            return typeDict.TryGetValue(type, out typeValue);
        }
    }
}