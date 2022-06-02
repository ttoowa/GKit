using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace GKit.Json {
	public static class SerializeUtility {
		public delegate void FieldHandlerDelegate(object model, FieldInfo fieldInfo, ref bool skip);
		/// <returns>Returns whether or not handled. Default handling if false is returned.</returns>
		public delegate bool FieldToJTokenDelegate(object model, FieldInfo fieldInfo, out JObject field);
		/// <returns>Returns whether or not handled. Default handling if false is returned.</returns>
		public delegate bool JTokenToFieldDelegate(object model, FieldInfo fieldInfo, out object field);

		public static void AddAttrFields<Attr>(this JObject jObject, object model, FieldToJTokenDelegate structHandler = null, FieldToJTokenDelegate classHandler = null)
			where Attr : Attribute {
			FieldHandlerDelegate preHandler = (object handleModel, FieldInfo fieldInfo, ref bool skip) => {

				if (!fieldInfo.GetCustomAttributes<Attr>().Any())
					skip = true;
			};

			AddFields(jObject, model, preHandler, structHandler, classHandler);
		}
		public static void AddFields(this JObject jObject, object model, FieldHandlerDelegate preHandler = null, FieldToJTokenDelegate structHandler = null, FieldToJTokenDelegate classHandler = null) {

			FieldInfo[] fields = model.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(field => field.MetadataToken).ToArray(); ;

			foreach (FieldInfo fieldInfo in fields) {
				bool preSkip = false;
				preHandler?.Invoke(model, fieldInfo, ref preSkip);
				if (preSkip)
					continue;

				object value = fieldInfo.GetValue(model);
				JToken jToken = null;

				if (value == null) {
					jToken = string.Empty;
				} else if (fieldInfo.FieldType.IsValueType && !fieldInfo.FieldType.IsEnum && !fieldInfo.FieldType.IsPrimitive) {
					// Struct
					bool useDefaultHandler = true;
					if (structHandler != null) {
						JObject jField = null;
						useDefaultHandler = !structHandler.Invoke(model, fieldInfo, out jField);
						jToken = jField;
					}
					if (useDefaultHandler) {
						JObject jField = new JObject();
						jField.AddFields(value);

						jToken = jField;
					}
				} else if (fieldInfo.FieldType.IsClass && fieldInfo.FieldType != typeof(string)) {
					// Class
					bool useDefaultHandler = true;
					if (classHandler != null) {
						JObject jField = null;
						useDefaultHandler = !classHandler.Invoke(model, fieldInfo, out jField);
						jToken = jField;
					}
					if (useDefaultHandler) {
						JObject jField = new JObject();
						jField.AddFields(value, preHandler, structHandler, classHandler);

						jToken = jField;
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

		public static void LoadAttrFields<Attr>(this object model, JObject jObject, FieldHandlerDelegate preHandler = null, JTokenToFieldDelegate structHandler = null, JTokenToFieldDelegate classHandler = null)
			where Attr : Attribute {
			FieldHandlerDelegate attrPreHandler = (object handleModel, FieldInfo fieldInfo, ref bool skip) => {
				Attr editorAttribute = fieldInfo.GetCustomAttribute(typeof(Attr)) as Attr;

				if (editorAttribute == null)
					skip = true;

				preHandler?.Invoke(handleModel, fieldInfo, ref skip);
			};

			LoadFields(model, jObject, attrPreHandler);
		}
		public static void LoadFields(this object model, JObject jObject, FieldHandlerDelegate preHandler = null, JTokenToFieldDelegate structHandler = null, JTokenToFieldDelegate classHandler = null) {

			FieldInfo[] fields = model.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(field => field.MetadataToken).ToArray();

			foreach (FieldInfo fieldInfo in fields) {
				bool preSkip = false;
				preHandler?.Invoke(model, fieldInfo, ref preSkip);
				if (preSkip)
					continue;

				if (!jObject.ContainsKey(fieldInfo.Name))
					continue;

				if (fieldInfo.FieldType.IsValueType && !fieldInfo.FieldType.IsEnum && !fieldInfo.FieldType.IsPrimitive) {
					// Struct
					JObject jField = jObject.GetValue(fieldInfo.Name) as JObject;

					if (jField == null)
						continue;

					bool useDefaultHandler = true;
					object field = null;
					if (structHandler != null) {
						useDefaultHandler = !structHandler.Invoke(model, fieldInfo, out field);
					}
					if (useDefaultHandler) {
						field = Activator.CreateInstance(fieldInfo.FieldType);
						field.LoadFields(jField);
					}

					fieldInfo.SetValue(model, field);
				} else if (fieldInfo.FieldType.IsClass && fieldInfo.FieldType != typeof(string)) {
					// Class

					JObject jField = jObject.GetValue(fieldInfo.Name) as JObject;

					if (jField == null)
						continue;

					bool useDefaultHandler = true;
					object field = null;
					if (classHandler != null) {
						useDefaultHandler = !classHandler.Invoke(model, fieldInfo, out field);
					}
					if (useDefaultHandler) {
						field = Activator.CreateInstance(fieldInfo.FieldType);
						field.LoadFields(jField);
					}

					fieldInfo.SetValue(model, field);
				} else if (fieldInfo.FieldType == typeof(string) || fieldInfo.FieldType.IsEnum || fieldInfo.FieldType.IsPrimitive) {
					// Enum or String or Primitive
					string stringValue = jObject.GetValue<string>(fieldInfo.Name);

					if (fieldInfo.FieldType.IsEnum) {
						fieldInfo.SetValue(model, Enum.Parse(fieldInfo.FieldType, stringValue));
					} else {
						fieldInfo.SetValue(model, Convert.ChangeType(stringValue, fieldInfo.FieldType));
					}
				}

			}
		}
	}
}
