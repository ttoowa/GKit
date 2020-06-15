using System;
using System.Collections.Generic;
using System.Diagnostics;
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
{
	public static class ProcessUtility {
		public static bool Start(string path, string args = null) {
			try {
				Process process = new Process();
				process.StartInfo.FileName = path;
				if (!string.IsNullOrEmpty(args)) {
					process.StartInfo.Arguments = args;
				}
				process.Start();
				return true;
			} catch(Exception ex) {
				GDebug.Log(ex.ToString(), GLogLevel.Warnning);
				return false;
			}
		}
	}
}
