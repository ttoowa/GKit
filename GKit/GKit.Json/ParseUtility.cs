using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GKit.Json {
	public static class ParseUtility {
		public static T GetValue<T>(this JObject jObject, string propertyName) {
			T value;
			JToken token = jObject[propertyName];
			if (token != null) {
				value = token.ToObject<T>();
				return value;
			} else {
				return default(T);
			}
		}
		public static T GetValue<T>(this JObject jObject, string propertyName, T failedReturnValue) {
			T value;
			JToken token = jObject[propertyName];
			if (token != null) {
				value = token.ToObject<T>();
				return value;
			} else {
				return failedReturnValue;
			}
		}
	}
}
