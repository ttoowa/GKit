using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Network {
	internal class GClientData {
		public Socket socket;
		public byte[] receivedHeaderBuffer;
		public SocketAsyncEventArgs receiveEventArgs;
		public SocketAsyncEventArgs sendEventArgs;
		public Queue<byte[]> sendQueue;
		public bool isSending;

		public GClientData(Socket socket, int headerSize) {
			this.socket = socket;
			receivedHeaderBuffer = new byte[headerSize];
			receiveEventArgs = new SocketAsyncEventArgs();
			sendEventArgs = new SocketAsyncEventArgs();
			sendQueue = new Queue<byte[]>();
		}


		public bool SendAsync() {
			return socket.SendAsync(sendEventArgs);
		}
	}
}
