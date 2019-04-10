using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bgoon.Net {
	public static class EndianConverter {
		public static byte[] Reverse(this byte[] data) {
			Array.Reverse(data);
			return data;
		}
		public static byte[] Reverse(this byte[] data, int startIndex) {
			data = data.Skip(startIndex).ToArray();
			Array.Reverse(data);
			return data;
		}
		public static byte[] Reverse(this byte[] data, int startIndex, int size) {
			data = data.Skip(startIndex).Take(size).ToArray();
			Array.Reverse(data);
			return data;
		}

		public static byte[] ToSingleByte(this int value) {
			return new byte[] {
				(byte)value
			};
		}
		public static byte[] ToNetBytes(this bool value) {
			return BitConverter.GetBytes(value);
		}
		public static byte[] ToNetBytes(this short value) {
			byte[] data = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian) {
				data.Reverse();
			}
			return data;
		}
		public static byte[] ToNetBytes(this int value) {
			byte[] data = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian) {
				data.Reverse();
			}
			return data;
		}
		public static byte[] ToNetBytes(this long value) {
			byte[] data = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian) {
				data.Reverse();
			}
			return data;
		}
		public static byte[] ToNetBytes(this ushort value) {
			byte[] data = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian) {
				data.Reverse();
			}
			return data;
		}
		public static byte[] ToNetBytes(this uint value) {
			byte[] data = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian) {
				data.Reverse();
			}
			return data;
		}
		public static byte[] ToNetBytes(this ulong value) {
			byte[] data = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian) {
				data.Reverse();
			}
			return data;
		}
		public static byte[] ToNetBytes(this float value) {
			byte[] data = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian) {
				data.Reverse();
			}
			return data;
		}
		public static byte[] ToNetBytes(this double value) {
			byte[] data = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian) {
				data.Reverse();
			}
			return data;
		}
		public static byte[] ToNetBytes(this string value, Encoding encoding) {
			byte[] data = encoding.GetBytes(value);
			return data;
		}
		public static byte[] ToNetBytesWithSize(this string value, Encoding encoding) {
			byte[] data = encoding.GetBytes(value);
			data = data.Length.ToNetBytes().Concat(data).ToArray();
			return data;
		}

		public static bool ToLocalBoolean(this byte[] data, int startIndex = 0) {
			return BitConverter.ToBoolean(data, startIndex);
		}
		public static short ToLocalInt16(this byte[] data, int startIndex = 0) {
			if (BitConverter.IsLittleEndian) {
				data = Reverse(data, startIndex, sizeof(short));
			} else {
				data = data.Skip(startIndex).ToArray();
			}

			return BitConverter.ToInt16(data, 0);
		}
		public static int ToLocalInt32(this byte[] data, int startIndex = 0) {
			if (BitConverter.IsLittleEndian) {
				data = Reverse(data, startIndex, sizeof(int));
			} else {
				data = data.Skip(startIndex).ToArray();
			}

			return BitConverter.ToInt32(data, 0);
		}
		public static long ToLocalInt64(this byte[] data, int startIndex = 0) {
			if (BitConverter.IsLittleEndian) {
				data = Reverse(data, startIndex, sizeof(long));
			} else {
				data = data.Skip(startIndex).ToArray();
			}

			return BitConverter.ToInt64(data, 0);
		}
		public static ushort ToLocalUInt16(this byte[] data, int startIndex = 0) {
			if (BitConverter.IsLittleEndian) {
				data = Reverse(data, startIndex, sizeof(ushort));
			} else {
				data = data.Skip(startIndex).ToArray();
			}

			return BitConverter.ToUInt16(data, 0);
		}
		public static uint ToLocalUInt32(this byte[] data, int startIndex = 0) {
			if (BitConverter.IsLittleEndian) {
				data = Reverse(data, startIndex, sizeof(uint));
			} else {
				data = data.Skip(startIndex).ToArray();
			}

			return BitConverter.ToUInt32(data, 0);
		}
		public static ulong ToLocalUInt64(this byte[] data, int startIndex = 0) {
			if (BitConverter.IsLittleEndian) {
				data = Reverse(data, startIndex, sizeof(ulong));
			} else {
				data = data.Skip(startIndex).ToArray();
			}

			return BitConverter.ToUInt64(data, 0);
		}
		public static float ToLocalFloat(this byte[] data, int startIndex = 0) {
			if (BitConverter.IsLittleEndian) {
				data = Reverse(data, startIndex, sizeof(float));
			} else {
				data = data.Skip(startIndex).ToArray();
			}

			return BitConverter.ToSingle(data, 0);
		}
		public static double ToLocalDouble(this byte[] data, int startIndex = 0) {
			if (BitConverter.IsLittleEndian) {
				data = Reverse(data, startIndex, sizeof(double));
			} else {
				data = data.Skip(startIndex).ToArray();
			}

			return BitConverter.ToDouble(data, 0);
		}
		public static string ToLocalStringWithSize(this byte[] data, Encoding encoding) {
			int length = data.ToLocalInt32();
			return encoding.GetString(data, sizeof(int), length);
		}
		public static string ToLocalStringWithSize(this byte[] data, Encoding encoding, out int size) {
			int length = data.ToLocalInt32();
			size = length;
			return encoding.GetString(data, sizeof(int), length);
		}
		public static string ToLocalString(this byte[] data, Encoding encoding) {
			return encoding.GetString(data, 0, data.Length);
		}
	}
}