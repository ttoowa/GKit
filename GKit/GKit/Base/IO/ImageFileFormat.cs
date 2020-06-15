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
	public enum ImageFileFormat {
		bmp,
		jpg,
		gif,
		tiff,
		png,
		Unknown
	}
}
