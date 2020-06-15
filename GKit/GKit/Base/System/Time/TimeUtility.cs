using System;
using System.Globalization;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public static class TimeUtility {

		public static string GetNowTimeText(TimeFormat timeTextType) {
			return GetTimeText(timeTextType, DateTime.Now);
		}
		public static string GetTimeText(TimeFormat timeTextType, DateTime time) {
			return time.ToString(timeTextType.ToStringWithDesc(), CultureInfo.InvariantCulture);
		}

	}
}
