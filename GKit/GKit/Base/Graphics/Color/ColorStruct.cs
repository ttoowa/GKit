#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Graphics {
	public struct HSV {
		public float hue;
		public float saturation;
		public float value;

		public HSV(float hue, float saturation, float value) {
			this.hue = hue;
			this.saturation = saturation;
			this.value = value;
		}
	}
}
