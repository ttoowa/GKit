using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgoon.Net {
	public static class PacketUtility {

		public static byte[] ToArray(this byte data) {
			return new byte[] { data };
		}
	}
}
