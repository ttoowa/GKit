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
	public class PacketData {
		Queue<byte> dataQueue;

		public PacketData(byte[] data) {
			dataQueue = new Queue<byte>();

			for (int i = 0; i < data.Length; ++i) {
				dataQueue.Enqueue(data[i]);
			}
		}
		public byte[] Get(int count) {
			Queue<byte> bufferQueue = new Queue<byte>();

			for (int i = 0; i < count; ++i) {
				bufferQueue.Enqueue(dataQueue.Dequeue());
			}
			return bufferQueue.ToArray();
		}
	}
}
