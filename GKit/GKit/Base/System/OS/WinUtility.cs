using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace GKit {
	public static class WinUtility {
		public class Impersonator : IDisposable {
			[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
			private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

			[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
			private extern static bool CloseHandle(IntPtr handle);

			private const int LOGON32_PROVIDER_DEFAULT = 0;
			private const int LOGON32_LOGON_INTERACTIVE = 2;

			public bool IsOpened {
				get; private set;
			}
			private string domainName;
			private string userName;
			private string pw;
			private SafeTokenHandle tockenHandle;
			private WindowsIdentity winId;
			private WindowsImpersonationContext impersonationContext;


			[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
			public Impersonator(string domainName, string userName, string pw) {
				this.domainName = domainName;
				this.userName = userName;
				this.pw = pw;
			}
			public void Dispose() {
				Close();
			}

			public void Open() {
				if (IsOpened)
					return;
				IsOpened = true;

				try {
					bool returnValue = LogonUser(userName, domainName, pw, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out tockenHandle);

					if (returnValue == false) {
						int lastError = Marshal.GetLastWin32Error();
						throw new System.ComponentModel.Win32Exception(lastError);
					}

					winId = new WindowsIdentity(tockenHandle.DangerousGetHandle());
					impersonationContext = winId.Impersonate();
				} catch (Exception ex) {
					Close();
					throw;
				}
			}
			public void Close() {
				if (!IsOpened)
					return;
				IsOpened = false;

				impersonationContext?.Dispose();
				winId?.Dispose();
				tockenHandle?.Dispose();
			}

			private sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid {
				private SafeTokenHandle()
					: base(true) { }

				[DllImport("kernel32.dll")]
				[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
				[SuppressUnmanagedCodeSecurity]
				[return: MarshalAs(UnmanagedType.Bool)]
				private static extern bool CloseHandle(IntPtr handle);

				protected override bool ReleaseHandle() {
					return CloseHandle(handle);
				}
			}
		}
	}
}
