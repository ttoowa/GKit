using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKit {
	public static class StringUtility {

		public static string Substring(this string text, string startSymbol, string endSymbol, bool includeSymbol = true) {
			if (text == null)
				return null;

			RangeInt range = new RangeInt(0, text.Length);
			bool foundStart = false;
			bool foundEnd = false;

			for (int i=0; i< text.Length; ++i) {
				if (!foundStart) {
					if (text.Length >= i + startSymbol.Length) {
						if (text.Substring(i, startSymbol.Length) == startSymbol) {
							foundStart = true;
							range.min = i;
						}
					}
				} else {
					if (text.Length >= i + endSymbol.Length) {
						if (text.Substring(i, endSymbol.Length) == endSymbol) {
							range.max = i+1;
							foundEnd = true;
							break;
						}
					}
				}
			}

			if(!includeSymbol) {
				if(foundStart) {
					range.min += startSymbol.Length;
					if(foundEnd) {
						range.max -= endSymbol.Length;
					}
				}
			}
			return text.Substring(range.min, range.Length);
		}
	}
}
