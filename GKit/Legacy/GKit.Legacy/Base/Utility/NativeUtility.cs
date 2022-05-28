﻿using System;
using System.Runtime.InteropServices;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public unsafe static class NativeUtility {
		[DllImport("msvcrt.dll", EntryPoint = "memset", SetLastError = false)]
		public static extern void MemSet(void* dest, int value, int byteCount);
		[DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
		public static extern void CopyMemory(void* dest, void* src, uint count);

		public static void Clear(byte[] byteArray, byte value) {
			fixed (byte* bytePtr = byteArray) {
				MemSet(bytePtr, value, byteArray.Length);
			}
		}
	}
}
