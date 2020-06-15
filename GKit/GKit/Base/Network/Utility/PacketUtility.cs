using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Network {
	public static class PacketUtility {

		public static byte[] ToArray(this byte data) {
			return new byte[] { data };
		}
	}
}
