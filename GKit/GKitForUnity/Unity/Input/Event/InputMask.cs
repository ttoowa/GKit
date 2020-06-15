using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKitForUnity {
	internal static class InputMask {
		private static List<int> maskList = new List<int>();

		internal static void Clear() {
			maskList.Clear();
		}
		internal static bool Check(int layer) {
			return (layer == 0) || maskList.Contains(layer);
		}
		internal static void Mark(int layer) {
			maskList.Add(layer);
		}
	}
}
