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
{
	public class GWait {
		internal GTimeUnit unit;
		internal float time;

		public GWait(GTimeUnit unit, float time) {
			this.unit = unit;
			this.time = time;
		}
		public GWait(GTimeUnit unit, int frame) {
			this.unit = unit;
			this.time = frame + 0.00001f;
		}
	}
}
