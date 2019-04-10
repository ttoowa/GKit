using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKit.Network {
	public enum PacketBuildMode {
		OnlyData,
		ByteHeader_Data,
		ShortHeader_Data,
		IntHeader_Data,
		LongHeader_Data,
		UShortHeader_Data,
		UIntHeader_Data,
		ULongHeader_Data,
	}
	public class PacketBuilder {
		private Queue<byte> byteQueue;
		public int Count => byteQueue.Count;
		
		public PacketBuilder() {
			byteQueue = new Queue<byte>();
		}
		public void Clear() {
			byteQueue.Clear();
		}
		public void Append(byte data) {
			byteQueue.Enqueue(data);
		}
		public void Append(byte[] data) {
			for(int i=0; i<data.Length; ++i) {
				Append(data[i]);
			}
		}
		public byte[] ToArray() {
			return byteQueue.ToArray();
		}
		public byte[] ToArray(NetProtocol protocol) {
			byte[] header = protocol.Header2Bytes(Count);
			return header.Concat(byteQueue.ToArray()).ToArray();
		}
		public byte[] ToArray(PacketBuildMode mode) {
			byte[] header = null;
			switch (mode) {
				default:
				case PacketBuildMode.OnlyData:
					break;
				case PacketBuildMode.ByteHeader_Data:
					header = ((byte)Count).ToArray();
					break;
				case PacketBuildMode.ShortHeader_Data:
					header = ((short)Count).ToNetBytes();
					break;
				case PacketBuildMode.IntHeader_Data:
					header = ((int)Count).ToNetBytes();
					break;
				case PacketBuildMode.LongHeader_Data:
					header = ((long)Count).ToNetBytes();
					break;
				case PacketBuildMode.UShortHeader_Data:
					header = ((ushort)Count).ToNetBytes();
					break;
				case PacketBuildMode.UIntHeader_Data:
					header = ((uint)Count).ToNetBytes();
					break;
				case PacketBuildMode.ULongHeader_Data:
					header = ((ulong)Count).ToNetBytes();
					break;
			}
			byte[] dataArray = byteQueue.ToArray();
			if (header == null) {
				return dataArray;
			} else {
				return header.Concat(dataArray).ToArray();
			}
		}
	}
}
