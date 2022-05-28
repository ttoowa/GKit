﻿using System.Collections.Generic;

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
