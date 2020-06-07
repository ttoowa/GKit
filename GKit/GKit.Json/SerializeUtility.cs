using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GKit.Json {
	public static class SerializeUtility {
		public static void AddAttributes<ModelType, AttrType>(this JObject jObject, ModelType model) where AttrType : Attribute {
			FieldInfo[] fields = typeof(ModelType).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (FieldInfo field in fields) {
				AttrType editorAttribute = field.GetCustomAttribute(typeof(AttrType)) as AttrType;

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
	}
}
