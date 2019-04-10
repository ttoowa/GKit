using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKit {
	public unsafe static class NativeUtility {
		[DllImport("msvcrt.dll", EntryPoint = "memset", SetLastError = false)]
		public static extern void MemSet(void* dest, int value, int byteCount);
		[DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
		public static extern void CopyMemory(void* dest, void* src, uint count);

		public static void Clear(byte[] byteArray, byte value) {
			fixed(byte* bytePtr = byteArray) {
				MemSet(bytePtr, value, byteArray.Length);
			}
		}
	}
}
