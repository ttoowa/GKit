using System.Net.Sockets;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Network {
	/// <summary>
	/// 패킷 헤더를 정의하는 프로토콜입니다.
	/// </summary>
	public class NetProtocol {
		public delegate int Bytes2HeaderDelegate(byte[] buffer);
		public delegate byte[] Header2BytesDelegate(int header);
		public delegate void SocketAsyncEventDelegate(object sender, SocketAsyncEventArgs e);
		public delegate bool SocketAsyncWorkDelegate(SocketAsyncEventArgs e);

		public int HeaderSize {
			get {
				return headerSize;
			}
		}
		public Bytes2HeaderDelegate Bytes2Header {
			get {
				return bytes2Header;
			}
		}
		public Header2BytesDelegate Header2Bytes {
			get {
				return header2Bytes;
			}
		}

		private int headerSize;
		private Bytes2HeaderDelegate bytes2Header;
		private Header2BytesDelegate header2Bytes;


		/// <summary>
		/// int사이즈의 헤더로 생성합니다.
		/// </summary>
		public NetProtocol() {
			headerSize = sizeof(int);
			bytes2Header = (byte[] buffer) => {
				return buffer.ToLocalInt32();
			};
			header2Bytes = (int header) => {
				return header.ToNetBytes();
			};
		}
		/// <param name="headerSize">헤더의 바이트 크기</param>
		/// <param name="bytes2Header">바이트를 데이터 길이에 해당하는 숫자로 변환하는 메서드</param>
		public NetProtocol(int headerSize, Bytes2HeaderDelegate bytes2Header, Header2BytesDelegate header2Bytes) {
			this.headerSize = headerSize;
			this.bytes2Header = bytes2Header;
			this.header2Bytes = header2Bytes;
		}
	}
}
