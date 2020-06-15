using System;
using System.Collections.Generic;
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
.MultiThread {
	public enum ParallelPriolity {
		Low,
		Normal,
		High,
		Full,
	}
}
