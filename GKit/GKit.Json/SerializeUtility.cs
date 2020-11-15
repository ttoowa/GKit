using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace GKit.Json {
	public static class SerializeUtility {
		public delegate void FieldHandlerDelegate(object model, FieldInfo fieldInfo, ref bool skip);
		public delegate void FieldToJTokenDelegate(object model, FieldInfo fieldInfo, out JObject jField);

		public static void AddAttrFields<Attr>(this JObject jObject, object model, FieldToJTokenDelegate structHandler = null, FieldToJTokenDelegate classHandler = null) 
			where Attr:Attribute {
			FieldHandlerDelegate preHandler = (object handleModel, FieldInfo fieldInfo, ref bool skip) => {
				Attr editorAttribute = fieldInfo.GetCustomAttribute(typeof(Attr)) as Attr;

				if (editorAttribute == null)
					skip = true;
			};

			AddFields(jObject, model, preHandler, structHandler, classHandler);
		}
		public static void AddFields(this JObject jObject, object model, FieldHandlerDelegate preHandler = null, FieldToJTokenDelegate structHandler = null, FieldToJTokenDelegate classHandler = null) {

			FieldInfo[] fields = model.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (FieldInfo fieldInfo in fields) {
				bool preSkip = false;
				preHandler?.Invoke(model, fieldInfo, ref preSkip);
				if (preSkip)
					continue;

				object value = fieldInfo.GetValue(model);
				JToken jToken = null;

				if (value == null) {
					jToken = string.Empty;
				} else if (fieldInfo.FieldType.IsValueType && !fieldInfo.FieldType.IsEnum &&!fieldInfo.FieldType.IsPrimitive) {
					// Struct
					if (structHandler != null) {
						JObject jField = null;
						structHandler.Invoke(model, fieldInfo, out jField);
						jToken = jField;
					} else {
						JObject jValue = new JObject();
						AddFields(jValue, value);

						jToken = jValue;
					}
				} else if (fieldInfo.FieldType.IsClass && fieldInfo.FieldType != typeof(string)) {
					// Class
					if (classHandler != null) {
						JObject jField = null;
						classHandler.Invoke(model, fieldInfo, out jField);
						jToken = jField;
					}
				} else {
					jToken = value.ToString();
				}

				if(jToken != null) {
					jObject.Add(fieldInfo.Name, jToken);
				}

			}
		}

		public static void LoadAttrFields<Attr>(this object model, JObject jObject, FieldHandlerDelegate preHandler = null)
			where Attr : Attribute {
			FieldHandlerDelegate attrPreHandler = (object handleModel, FieldInfo fieldInfo, ref bool skip) => {
				Attr editorAttribute = fieldInfo.GetCustomAttribute(typeof(Attr)) as Attr;

				if (editorAttribute == null)
					skip = true;

				preHandler?.Invoke(handleModel, fieldInfo, ref skip);
			};

			LoadFields(model, jObject, attrPreHandler);
		}
		public static void LoadFields(this object model, JObject jObject, FieldHandlerDelegate preHandler = null) {

			FieldInfo[] fields = model.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (FieldInfo fieldInfo in fields) {
				bool preSkip = false;
				preHandler?.Invoke(model, fieldInfo, ref preSkip);
				if (preSkip)
					continue;

				if (!jObject.ContainsKey(fieldInfo.Name))
					continue;

				if (fieldInfo.FieldType.IsValueType && !fieldInfo.FieldType.IsEnum && !fieldInfo.FieldType.IsPrimitive) {
					// Struct
					JObject jField = jObject.GetValue(fieldInfo.Name).ToObject<JObject>();

					if (jField == null)
						continue;

					object field = Activator.CreateInstance(fieldInfo.FieldType);
					LoadFields(field, jField);

					fieldInfo.SetValue(model, field);
				} else if (fieldInfo.FieldType.IsClass && fieldInfo.FieldType != typeof(string)) {
					// Class
					JObject jField = jObject.GetValue(fieldInfo.Name).ToObject<JObject>();

					if (jField == null)
						continue;

					object field = Activator.CreateInstance(fieldInfo.FieldType);
					LoadFields(field, jField);

					fieldInfo.SetValue(model, field);
				} else {
					string stringValue = jObject.GetValue<string>(fieldInfo.Name);

					if(fieldInfo.FieldType.IsEnum) {
						fieldInfo.SetValue(model, Enum.Parse(fieldInfo.FieldType, stringValue));
					} else {
						fieldInfo.SetValue(model, Convert.ChangeType(stringValue, fieldInfo.FieldType));
					}
				}

			}
		}
	}
}
