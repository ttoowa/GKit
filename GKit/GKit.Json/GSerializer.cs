using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GKit.Json;

public delegate void FieldHandlerDelegate(object model, FieldInfo fieldInfo, ref bool skip);

/// <returns>Returns whether or not handled. Default handling if false is returned.</returns>
public delegate bool FieldToJTokenDelegate(object model, FieldInfo fieldInfo, out JObject field);

/// <returns>Returns whether or not handled. Default handling if false is returned.</returns>
public delegate bool JTokenToFieldDelegate(object model, FieldInfo fieldInfo, out object field);

public static class GSerializer {
    public static void SerializeFields<Attr>(this JObject jObject, object model, GSerializeSettings settings = null) where Attr : Attribute {
        if (settings == null) {
            settings = new GSerializeSettings();
        }

        settings.requiredAttributeType = typeof(Attr);

        SerializeFields(jObject, model, settings);
    }

    public static void SerializeFields(this JObject jObject, object model, GSerializeSettings settings = null) {
        var fields = model.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(field => field.MetadataToken).ToArray();

        foreach (var fieldInfo in fields) {
            if (settings?.requiredAttributeType != null) {
                if (!fieldInfo.GetCustomAttributes(settings.requiredAttributeType, true).Any()) {
                    continue;
                }
            }

            var preSkip = false;
            settings?.preHandler?.Invoke(model, fieldInfo, ref preSkip);
            if (preSkip) {
                continue;
            }

            var value = fieldInfo.GetValue(model);
            JToken jToken = null;

            if (value == null) {
                jToken = string.Empty;
            } else if (fieldInfo.FieldType.IsValueType && !fieldInfo.FieldType.IsEnum && !fieldInfo.FieldType.IsPrimitive) {
                // Struct
                var useDefaultHandler = true;
                if (settings?.memberHandler != null) {
                    JObject jField = null;
                    useDefaultHandler = !settings.memberHandler.Invoke(model, fieldInfo, out jField);
                    jToken = jField;
                }

                if (useDefaultHandler) {
                    var jField = new JObject();
                    jField.SerializeFields(value);

                    jToken = jField;
                }
            } else if (fieldInfo.FieldType.IsClass && fieldInfo.FieldType != typeof(string)) {
                // Class
                var useDefaultHandler = true;
                if (settings?.memberHandler != null) {
                    JObject jField = null;
                    useDefaultHandler = !settings.memberHandler.Invoke(model, fieldInfo, out jField);
                    jToken = jField;
                }

                if (useDefaultHandler) {
                    if (typeof(IEnumerable).IsAssignableFrom(fieldInfo.FieldType)) {
                        var jSettings = new JsonSerializerSettings();
                        if (settings?.requiredAttributeType != null) {
                            jSettings.ContractResolver = new RequireAttributeResolver(settings.requiredAttributeType);
                        }

                        var jString = JsonConvert.SerializeObject(fieldInfo.GetValue(model), jSettings);
                        jToken = JToken.Parse(jString);
                    } else {
                        var jField = new JObject();
                        jField.SerializeFields(value, settings);

                        jToken = jField;
                    }
                }
            } else if (fieldInfo.FieldType == typeof(string) || fieldInfo.FieldType.IsEnum || fieldInfo.FieldType.IsPrimitive) {
                // Enumm or String or Primitive
                jToken = value.ToString();
            }

            if (jToken != null) {
                jObject.Add(fieldInfo.Name, jToken);
            }
        }
    }

    public static void DeserializeFields<Attr>(this object model, JObject jObject, GDeserializeSettings settings = null) where Attr : Attribute {
        if (settings == null) {
            settings = new GDeserializeSettings();
        }

        settings.preHandler += (object handleModel, FieldInfo fieldInfo, ref bool skip) => {
            var editorAttribute = fieldInfo.GetCustomAttribute(typeof(Attr)) as Attr;

            if (editorAttribute == null) {
                skip = true;
            }
        };

        DeserializeFields(model, jObject, settings);
    }

    public static void DeserializeFields(this object model, JObject jObject, GDeserializeSettings settings = null) {
        if (model == null) {
            return;
        }

        var fields = model.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(field => field.MetadataToken).ToArray();

        foreach (var fieldInfo in fields) {
            var preSkip = false;
            settings?.preHandler?.Invoke(model, fieldInfo, ref preSkip);
            if (preSkip) {
                continue;
            }

            if (!jObject.ContainsKey(fieldInfo.Name)) {
                continue;
            }

            if (fieldInfo.FieldType.IsValueType && !fieldInfo.FieldType.IsEnum && !fieldInfo.FieldType.IsPrimitive) {
                // Struct
                var jField = jObject.GetValue(fieldInfo.Name) as JObject;

                if (jField == null) {
                    continue;
                }

                var useDefaultHandler = true;
                object field = null;
                if (settings?.memberHandler != null) {
                    useDefaultHandler = !settings.memberHandler.Invoke(model, fieldInfo, out field);
                }

                if (useDefaultHandler) {
                    field = Activator.CreateInstance(fieldInfo.FieldType);
                    field.DeserializeFields(jField);
                }

                fieldInfo.SetValue(model, field);
            } else if (fieldInfo.FieldType.IsClass && fieldInfo.FieldType != typeof(string)) {
                // Class
                var jFieldToken = jObject.GetValue(fieldInfo.Name);

                if (jFieldToken == null) {
                    continue;
                }

                var useDefaultHandler = true;
                object field = null;
                if (settings?.memberHandler != null) {
                    useDefaultHandler = !settings.memberHandler.Invoke(model, fieldInfo, out field);
                }

                if (useDefaultHandler) {
                    if (typeof(IEnumerable).IsAssignableFrom(fieldInfo.FieldType)) {
                        fieldInfo.SetValue(model, JsonConvert.DeserializeObject(jFieldToken.ToString(), fieldInfo.FieldType));
                    } else if (jFieldToken is JObject) {
                        field = Activator.CreateInstance(fieldInfo.FieldType);
                        field.DeserializeFields(jFieldToken as JObject);
                    }
                }

                if (field != null) {
                    fieldInfo.SetValue(model, field);
                }
            } else if (fieldInfo.FieldType == typeof(string) || fieldInfo.FieldType.IsEnum || fieldInfo.FieldType.IsPrimitive) {
                // Enum or String or Primitive
                var stringValue = jObject.GetValue<string>(fieldInfo.Name);

                if (fieldInfo.FieldType.IsEnum) {
                    fieldInfo.SetValue(model, Enum.Parse(fieldInfo.FieldType, stringValue));
                } else {
                    fieldInfo.SetValue(model, Convert.ChangeType(stringValue, fieldInfo.FieldType));
                }
            }
        }
    }

    private class RequireAttributeResolver : DefaultContractResolver {
        public readonly Type AttributeType;

        public RequireAttributeResolver(Type attributeType) {
            AttributeType = attributeType;
        }

        protected override List<MemberInfo> GetSerializableMembers(Type objectType) {
            var members = base.GetSerializableMembers(objectType);

            if (objectType.IsClass && objectType != typeof(string)) {
                members = members.Where(x => x.GetCustomAttributes(AttributeType, true).Any()).ToList();
            }

            return members;
        }
    }
}