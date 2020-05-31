using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GKit {
	public static class AssemblyInfo {
		public static string Name {
			get {
				return Assembly.GetEntryAssembly().GetName().Name;
			}
		}
		public static string DLLName {
			get {
				return Assembly.GetExecutingAssembly().GetName().Name;
			}
		}
	}
}
