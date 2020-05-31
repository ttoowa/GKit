using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GKit.Security;

namespace GKit.Network {
	public class GPipeServer : IDisposable {
		private const string HandleHeader_In = "IN[";
		private const string HandleHeader_Out = "OUT[";
		private const string HandleTail = "]";
		public NamedPipeServerStream inPipe;
		public NamedPipeServerStream outPipe;
		public NetProtocol Protocol {
			get; private set;
		}
		public string Handle {
			get; private set;
		}
		private string inHandle;
		private string outHandle;
		
		public GPipeServer(NetProtocol protocol) {
			this.Protocol = protocol;

			CreateHandle();

			inPipe = new NamedPipeServerStream(inHandle, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.None);
			outPipe = new NamedPipeServerStream(outHandle, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.None);
		}
		private void CreateHandle() {
			inHandle = Encrypt.GenerateHash(8);
			for (; ; ) {
				outHandle = Encrypt.GenerateHash(8);
				if (inHandle != outHandle) {
					break;
				}
			}
			Handle = HandleHeader_In + inHandle + HandleTail +
					HandleHeader_Out + outHandle + HandleTail;
		}

		public void WaitForConnection() {
			inPipe.WaitForConnection();
			outPipe.WaitForConnection();
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
