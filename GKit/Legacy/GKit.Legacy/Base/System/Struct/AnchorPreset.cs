﻿#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public enum AnchorPreset {
		TopLeft,
		TopMid,
		TopRight,

		MidLeft,
		MidMid,
		MidRight,

		BotLeft,
		BotMid,
		BotRight,

		StretchLeft,
		StretchMid,
		StretchRight,

		TopStretch,
		MidStretch,
		BotStretch,

		StretchAll,
	}

	public enum AxisAnchor {
		Min,
		Mid,
		Max,
		Stretch,
	}


}

