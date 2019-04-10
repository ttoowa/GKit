using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKit {
	public enum TimeTextType {
		YYYY,
		YYYY_MM,
		YYYY_MM_DD,
		YYYY_MM_DD_HH,
		YYYY_MM_DD_HH_mm,
		YYYY_MM_DD_HH_mm_ss,
		HH,
		HH_mm,
		HH_mm_ss,
	}
	public static class TimeUtility {
		public static string GetNowText(TimeTextType type) {
			DateTime now = DateTime.Now;
			StringBuilder sb = new StringBuilder();
			switch(type) {
				case TimeTextType.YYYY:
				case TimeTextType.YYYY_MM:
				case TimeTextType.YYYY_MM_DD:
				case TimeTextType.YYYY_MM_DD_HH:
				case TimeTextType.YYYY_MM_DD_HH_mm:
				case TimeTextType.YYYY_MM_DD_HH_mm_ss:
					sb.Append(now.ToString("yyyy"));
					break;
			}
			switch(type) {
				case TimeTextType.YYYY_MM:
				case TimeTextType.YYYY_MM_DD:
				case TimeTextType.YYYY_MM_DD_HH:
				case TimeTextType.YYYY_MM_DD_HH_mm:
				case TimeTextType.YYYY_MM_DD_HH_mm_ss:
					sb.Append(now.ToString(".MM"));
					break;
			}
			switch (type) {
				case TimeTextType.YYYY_MM_DD:
				case TimeTextType.YYYY_MM_DD_HH:
				case TimeTextType.YYYY_MM_DD_HH_mm:
				case TimeTextType.YYYY_MM_DD_HH_mm_ss:
					sb.Append(now.ToString(".dd"));
					break;
			}
			switch (type) {
				case TimeTextType.HH:
				case TimeTextType.HH_mm:
				case TimeTextType.HH_mm_ss:
				case TimeTextType.YYYY_MM_DD_HH:
				case TimeTextType.YYYY_MM_DD_HH_mm:
				case TimeTextType.YYYY_MM_DD_HH_mm_ss:
					if(sb.Length > 0) {
						sb.Append(' ');
					}
					sb.Append(now.ToString("tt hh"));
					break;
			}
			switch(type) {
				case TimeTextType.HH_mm:
				case TimeTextType.HH_mm_ss:
				case TimeTextType.YYYY_MM_DD_HH_mm:
				case TimeTextType.YYYY_MM_DD_HH_mm_ss:
					sb.Append(now.ToString(":mm"));
					break;
			}
			switch (type) {
				case TimeTextType.HH_mm:
				case TimeTextType.HH_mm_ss:
				case TimeTextType.YYYY_MM_DD_HH_mm:
				case TimeTextType.YYYY_MM_DD_HH_mm_ss:
					sb.Append(now.ToString(":ss"));
					break;
			}
			return sb.ToString();
		}
	}
}
