#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.IO {
	public enum ImageFileFormat {
		bmp,
		jpg,
		gif,
		tiff,
		png,
		Unknown
	}
}
