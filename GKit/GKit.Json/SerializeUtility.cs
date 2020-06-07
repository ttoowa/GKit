using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GKit.Json {
	public static class SerializeUtility {
		public static void AddAttrFields<Model, Attr>(this JObject jObject, Model model)
			where Attr : Attribute
			where Model : class {

			FieldInfo[] fields = typeof(Model).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (FieldInfo field in fields) {
				Attr editorAttribute = field.GetCustomAttribute(typeof(Attr)) as Attr;

				if (editorAttribute == null)
					continue;

				object value = field.GetValue(model);
				string stringValue;

				if (value == null) {
					stringValue = string.Empty;
				} else {
					stringValue = value.ToString();
				}

				jObject.Add(field.Name, stringValue);
			}
		}

		public static void LoadAttrFields<Model, Attr>(this Model model, JObject jObject)
			where Attr : Attribute	
			where Model : class {

			FieldInfo[] fields = typeof(Model).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (FieldInfo field in fields) {
				Attr editorAttribute = field.GetCustomAttribute(typeof(Attr)) as Attr;

				if (editorAttribute == null)
					continue;

				if (!jObject.ContainsKey(field.Name))
					continue;

				string stringValue = jObject.GetValue<string>(field.Name);
				
				field.SetValue(model, Convert.ChangeType(stringValue, field.FieldType));
			}
		}
	}
}
