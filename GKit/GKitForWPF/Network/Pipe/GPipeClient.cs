using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GKit.Network {
	public class GPipeClient : IDisposable {
		private const string HandleHeader_In = "IN[";
		private const string HandleHeader_Out = "OUT[";
		private const string HandleTail = "]";
		public NamedPipeClientStream inPipe;
		public NamedPipeClientStream outPipe;
		public NetProtocol Protocol {
			get; private set;
		}
		public string ServerHandle {
			get; private set;
		}
		public bool IsConnected {
			get {
				return inPipe.IsConnected && outPipe.IsConnected;
			}
		}

		
		public GPipeClient(string serverHandle, NetProtocol protocol) {
			this.Protocol = protocol;

			ServerHandle = serverHandle;
			string inHandle = serverHandle.Substring(HandleHeader_In, HandleTail, false);
			string outHandle = serverHandle.Substring(HandleHeader_Out, HandleTail, false);

			inPipe = new NamedPipeClientStream(".", outHandle, PipeDirection.In, PipeOptions.None);
			outPipe = new NamedPipeClientStream(".", inHandle, PipeDirection.Out, PipeOptions.None);
		}

		public void Connect() {
			inPipe.Connect();
			outPipe.Connect();
		}
		public void SendLow(byte[] data) {
			outPipe.Write(data, 0, data.Length);
		}
		public void Send(byte[] data) {
			PacketBuilder builder = new PacketBuilder();
			builder.Append(Protocol.Header2Bytes(data.Length));
			builder.Append(data);
			SendLow(builder.ToArray());
		}
		public byte[] ReceiveLow(int length) {
			byte[] buffer = new byte[length];
			inPipe.Read(buffer, 0, length);
			return buffer;
		}
		public byte[] Receive() {
			byte[] headerData = ReceiveLow(Protocol.HeaderSize);
			return ReceiveLow(Protocol.Bytes2Header(headerData));
		}
		public void Dispose() {
			inPipe.Dispose();
			outPipe.Dispose();
		}
	}
}
